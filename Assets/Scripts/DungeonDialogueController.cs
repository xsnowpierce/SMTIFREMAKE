using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Database;
using Game.Level;
using SpriteUtils;
using UnityEngine;
using Story = Ink.Runtime.Story;

namespace Dialogue
{
    [Serializable]
    public struct PossibleDialogue
    {
        public TextAsset inkJsonAsset;
        public string musicID;
        public string requiredFlag;
        [Header("Dialogue Tile Settings")] public Vector3[] requiredRotationsForTile;
        public Vector3 afterwardsRotation;
    }
    public class DungeonDialogueController : DialogueController
    {
        private List<GameObject> actors;
        private PossibleDialogue? chosenDialogue;

        [Header("Sprite Settings")] [SerializeField]
        private GameObject spritePrefab;

        [SerializeField] private Material spriteMaterial;
        [SerializeField] private float spriteSizeDiv = 150;
        private Vector2 spriteSizeMult = new(1, 1);

        protected override void Awake()
        {
            base.Awake();
            chosenDialogue = null;
            actors = new List<GameObject>();
        }

        public PossibleDialogue? GetChosenDialogue() => chosenDialogue;
        
        private void LoadDialogue(TextAsset inkFile, Vector3 spriteSpawnPoint)
        {
            currentStory = new Story(inkFile.text);
            RegisterDialogueCommands();
            currentStory.BindExternalFunction("create_actor",
                (int actorID, int spriteID, float spritePosition) =>
                {
                    CreateActor(actorID, spriteID, spritePosition, spriteSpawnPoint);
                });
            currentStory.Continue();
        }

        public void DetermineAndLoadDialogue(PossibleDialogue[] dialogues, Vector3 spriteSpawnPoint)
        {
            currentStory = null;
            PossibleDialogue? chosen = null;

            for (int i = 0; i < dialogues.Length; i++)
            {
                if (string.IsNullOrEmpty(dialogues[i].requiredFlag) ||
                    saveData.playerFlags.Contains(dialogues[i].requiredFlag))
                {
                    chosen = dialogues[i];
                }
            }

            if (chosen != null)
            {
                chosenDialogue = chosen;
                if (chosen.Value.inkJsonAsset == null)
                {
                    Debug.LogError("Room had dialogue, but the file was null.");
                    return;
                }

                // Notify the music player of a possible change
                MusicController musicController = FindObjectOfType<MusicController>();

                if (!string.IsNullOrEmpty(chosen.Value.musicID))
                {
                    if (chosen.Value.musicID.Equals("none"))
                        musicController.EndSong();
                    else
                        musicController.ChangeToSong(musicController.GetCurrentMusicPack()
                            .GetMusicFromString(chosen.Value.musicID));
                }

                LoadDialogue(chosen.Value.inkJsonAsset, spriteSpawnPoint);
                return;
            }

            Debug.LogError("No dialogue was found. Loading default message.");
        }
        
        protected override void RegisterDialogueCommands()
        {
            base.RegisterDialogueCommands();
            currentStory.BindExternalFunction("set_sprite_size_mult",
                (float x, float y) => { SetSpriteSizeMult(x, y); });
            currentStory.BindExternalFunction("change_sprite",
                (int actorNumber, int spriteID) => { ChangeSprite(actorNumber, spriteID); });
        }

        public void OpenStairsDialogue(bool upStairs)
        {
            string messageKey;
            if (upStairs)
                messageKey = databases.GetTranslatedText("upstairs_dialogue");
            else messageKey = databases.GetTranslatedText("downstairs_dialogue");

            string yesChoice = databases.GetTranslatedText("response_common_yes");
            string noChoice = databases.GetTranslatedText("response_common_no");
            StartCoroutine(StairsDialogue(messageKey, yesChoice, noChoice));
        }

        public void KillActors()
        {
            foreach (GameObject obj in actors)
            {
                Destroy(obj);
            }

            actors = new List<GameObject>();
        }

        private IEnumerator StairsDialogue(string message, string yes, string no)
        {
            dialogue.SetDialogueText("", message);
            dialogueOpened = true;
            yield return StartCoroutine(dialogue.ShowDialogueBox(true));

            // The player has to make a choice
            waitingForChoice = true;

            // Open the dialogue responses box
            string[] responses = new string[2];
            responses[0] = yes;
            responses[1] = no;
            yield return dialogue.OpenResponses(responses);

            while (waitingForChoice)
            {
                // Wait for the player to make their choice
                if (dialogue.IsInteractPressed())
                    waitingForChoice = false;
                yield return null;
            }

            choiceMade = dialogue.GetCurrentSelected();

            if (choiceMade < 0)
            {
                Debug.LogError("Player's response was less than 0, resetting to 0.");
                choiceMade = 0;
            }

            // Player has made their choice, continue
            yield return StartCoroutine(dialogue.CloseBox());
            dialogueOpened = false;

            yield return null;
        }


        private void SetSpriteSizeMult(float x, float y)
        {
            spriteSizeMult = new Vector2(x, y);
        }

        private void CreateActor(int actorID, int spriteID, float spritePosition, Vector3 spriteSpawnPoint)
        {
            Sprite sprite = databases.GetSprite(spriteID);
            Vector3 localSpritePosition = new Vector3(spritePosition, 0, 0);

            GameObject obj = Instantiate(spritePrefab, Vector3.zero, Quaternion.identity);
            obj.transform.localPosition = (spriteSpawnPoint + localSpritePosition);
            obj.GetComponent<SpriteLoader>().LoadSprite(sprite, spriteMaterial, spriteSizeDiv);

            if (actorID <= actors.Count) actors.Insert(actorID, obj);
            else actors.Add(obj);
            obj.transform.localScale = new Vector3(spriteSizeMult.x, spriteSizeMult.y, 1);
            spriteSizeMult = new Vector2(1, 1);
        }

        private void ChangeSprite(int actorNumber, int spriteID)
        {
            if (actors.Count < (actorNumber - 1))
            {
                throw new ArgumentOutOfRangeException();
            }
            
            Debug.LogError("Changing sprites is not implemented yet");
        }
    }
}