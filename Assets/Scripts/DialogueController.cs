using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Database;
using Ink.Runtime;
using Items;
using UnityEngine;

namespace Dialogue
{
    public abstract class DialogueController : MonoBehaviour
    {
        [SerializeField] protected DialogueUIController dialogue;
        protected Databases databases;
        protected Story currentStory;
        protected SaveData saveData;
        protected bool dialogueOpened;
        protected bool waitingForChoice;
        protected int choiceMade = -1;
        
        protected virtual void Awake()
        {
            if (dialogue == null)
            {
                Debug.LogError("Dialogue UI was not found. Disabling dialogue.");
                this.enabled = false;
            }
            saveData = FindObjectOfType<SaveData>();
            databases = FindObjectOfType<Databases>();
            dialogue.CloseBox();
        }
        
        protected void LoadDialogue(TextAsset inkFile)
        {
            currentStory = new Story(inkFile.text);
            RegisterDialogueCommands();
            currentStory.Continue();
        }

        public IEnumerator ReadString(string speakerName, string text)
        {
            dialogue.SetDialogueText(speakerName, text);
            dialogueOpened = true;
            yield return StartCoroutine(dialogue.ShowDialogueBox(false));
            
            while (!dialogue.IsInteractPressed())
            {
                yield return null;
            }
            
            yield return StartCoroutine(dialogue.CloseBox());
            dialogueOpened = false;
            yield return null;
        }

        protected virtual void RegisterDialogueCommands()
        {
            currentStory.BindExternalFunction("give_item",
                (int itemID, int amount) => { GivePlayerItem(itemID, amount); });
            currentStory.BindExternalFunction("give_demon", (int demonID) => { GivePlayerDemon(demonID); });
            currentStory.BindExternalFunction("add_party_member", (int memberID) => { AddPartyMember(memberID); });
            currentStory.BindExternalFunction("add_player_flag", (string flagName) => { AddPlayerFlag(flagName); });
            currentStory.BindExternalFunction("add_nova_points", (int amount) => { AddNovaPoints(amount); });
            currentStory.BindExternalFunction("set_player_gender", (int gender) => { SetPlayerGender(gender); });
            currentStory.BindExternalFunction("heal_party", HealParty);
            currentStory.BindExternalFunction("save_game", SaveGame);
        }
        
        protected string ParseString(string stringToParse)
        {
            if (String.IsNullOrEmpty(stringToParse))
            {
                Debug.LogError("Message was empty.");
                return "";
            }
            string playerName = saveData.playerName;
            return stringToParse.Replace("{PLAYERNAME}", playerName);
        }

        protected string GetStringFromDictionary(string key)
        {
            string it = databases.GetTranslatedText(key.Trim().ToLower());
            if (string.IsNullOrEmpty(it))
            {
                Debug.LogError("Did not get anything back from database. Is the key added to the database?");
                return "error";
            }
            return ParseString(it);
        }

        protected string GetNameFromDictionary(string key)
        {
            string it = databases.GetTranslatedName(key.Trim().ToLower());
            if (string.IsNullOrEmpty(it))
            {
                Debug.LogError("Did not get anything back from database. Is the key added to the database?");
                return "error";
            }
            return ParseString(it);
        }
        
        /// <summary>
        /// Shows the currently loaded dialogue called by LoadDialogue,
        /// and allows for player input.
        /// </summary>
        public void OpenDialogue()
        {
            StartCoroutine(ReadDialogue());
        }

        private IEnumerator ReadDialogue()
        {
            bool firstRun = true;
            waitingForChoice = false;
            string currentCharacter = "";

            // Shouldn't happen, but might!
            if (currentStory == null)
            {
                currentStory = null;
                dialogue.SetDialogueText("system", GetStringFromDictionary("nobody_here"));
                dialogueOpened = true;
                yield return StartCoroutine(dialogue.ShowDialogueBox(false));
                while (!dialogue.IsInteractPressed())
                {
                    yield return null;
                }

                yield return StartCoroutine(dialogue.CloseBox());
                dialogueOpened = false;
                yield break;
            }

            do
            {
                if (currentStory.canContinue && !firstRun)
                    currentStory.Continue();

                // Do item giving code
                List<string> itemKeys = new List<string>();
                itemKeys.AddRange(currentStory.currentTags.FindAll(s => s.Contains("giveitem =")));
                itemKeys.AddRange(currentStory.currentTags.FindAll(s => s.Contains("finditem =")));
                if (itemKeys.Count > 0)
                {
                    foreach (string key in itemKeys)
                    {
                        string newkey;
                        string message;
                        if (key.Contains("giveitem"))
                        {
                            newkey = key.Replace("giveitem =", "").Trim();
                            message = databases.GetTranslatedText("give_item");
                        }
                        else
                        {
                            newkey = key.Replace("finditem =", "").Trim();
                            message = databases.GetTranslatedText("find_item");
                        }

                        newkey = newkey.Replace(" ", "");
                        
                        string itemKey = newkey.Split(',')[0];
                        string itemAmount = newkey.Split(',')[1];

                        Item item = databases.GetItem(Int32.Parse(itemKey));

                        string itemName = databases.GetTranslatedItem(item.itemKey);
                        message = message.Replace("{ITEM}", itemName).Replace("{AMOUNT}", itemAmount);
                        
                        dialogue.SetDialogueText("system", message);

                        dialogueOpened = true;
                        yield return StartCoroutine(dialogue.ShowDialogueBox(false));
                        while (!dialogue.IsInteractPressed())
                        {
                            yield return null;
                        }

                        // Add item to inventory
                        GivePlayerItem(Int32.Parse(itemKey), Int32.Parse(itemAmount));
                    }
                }

                string characterName =
                    currentStory.currentTags.FirstOrDefault(s => s.Contains("name = "))?.Substring(7);

                if (!string.IsNullOrEmpty(characterName))
                {
                    if (characterName.ToLower() != "system")
                        characterName = GetNameFromDictionary(characterName);

                    currentCharacter = characterName;
                }

                if (string.IsNullOrEmpty(currentStory.currentText) ||
                    string.IsNullOrWhiteSpace(currentStory.currentText))
                {
                    continue;
                }

                string dialogueText = GetStringFromDictionary(currentStory.currentText);
                dialogue.SetDialogueText(currentCharacter, dialogueText);
                dialogueOpened = true;
                yield return StartCoroutine(dialogue.ShowDialogueBox(currentStory.currentChoices is { Count: > 0 }));

                // The player has now read the entire message, and can press a button to continue the dialogue

                if (currentStory.currentChoices is { Count: > 0 }) // Make sure the list is not null or empty
                {
                    // The player has to make a choice
                    waitingForChoice = true;

                    // Open the dialogue responses box
                    string[] choices = new string[currentStory.currentChoices.Count];
                    for (int i = 0; i < currentStory.currentChoices.Count; i++)
                    {
                        Choice choice = currentStory.currentChoices[i];
                        choices[i] = GetStringFromDictionary(choice.text);
                    }

                    yield return dialogue.OpenResponses(choices);

                    while (waitingForChoice)
                    {
                        // Wait for the player to make their choice
                        if (dialogue.IsInteractPressed())
                            waitingForChoice = false;
                        yield return null;
                    }

                    // Player has made their choice, continue
                    int playerChoice = dialogue.GetCurrentSelected();
                    dialogue.CloseResponses();
                    currentStory.ChooseChoiceIndex(playerChoice);
                    //currentStory.Continue();
                    continue;
                }

                if (firstRun)
                    firstRun = false;

                // WAIT FOR PLAYER TO PRESS SPACE TO CONTINUE DIALOGUE
                while (!dialogue.IsInteractPressed())
                {
                    yield return null;
                }
                
                // Player pressed space, continue dialogue if we can
                
                // Play dialogue submit sound
                    
            } while (currentStory.canContinue);

            yield return StartCoroutine(dialogue.CloseBox());
            dialogueOpened = false;
            //spriteSizeMult = new Vector2(1,1);
            yield return null;
        }
        
        protected void SaveGame()
        {
        }
        
        protected void GivePlayerItem(int itemID, int amount)
        {
            Item addItem = FindObjectOfType<Databases>().GetItem(itemID);
            InventoryItem item = saveData.inventory.SingleOrDefault(item => item.item == addItem);
            if (item.item == null)
            {
                // Don't have the item!
                InventoryItem invItem = new InventoryItem()
                {
                    item = addItem, amount = amount
                };
                saveData.inventory.Add(invItem);
            }
            else
            {
                // Have the item, add to it
                InventoryItem singleOrDefault =
                    saveData.inventory.SingleOrDefault(findItem => findItem.item == addItem);
                singleOrDefault.amount += amount;
            }
        }
        
        protected void GivePlayerDemon(int demonID)
        {
        }

        protected void AddPartyMember(int entityID)
        {
            string code = "";
            saveData.AddPartyMember(entityID, out code);
            if(code == "successful") return;
            else if (code == "party_full")
            {
                Debug.Log("Party was full when trying to add a new entity.");
            }
        }

        protected void AddPlayerFlag(string flagName)
        {
            if (!saveData.playerFlags.Contains(flagName))
                saveData.playerFlags.Add(flagName);
        }

        protected void HealParty()
        {
        }

        protected void AddNovaPoints(int amount)
        {
            saveData.novaPoints += amount;
        }

        protected void SetPlayerGender(int gender)
        {
            if (gender == 0)
                saveData.playerGender = PlayerGender.Male;
            else saveData.playerGender = PlayerGender.Female;
        }
        
        public bool IsDialogueOpened() => dialogueOpened;

        public void MakeChoice(int choiceMade)
        {
            this.choiceMade = choiceMade;
            waitingForChoice = false;
        }

        public int GetChoiceMade()
        {
            int choice = choiceMade;
            choiceMade = -1;
            return choice;
        }
    }
}