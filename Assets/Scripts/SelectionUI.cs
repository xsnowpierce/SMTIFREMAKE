using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using Data;
using Database;
using Dialogue;
using Entity;
using Entity.Skills;
using Entity.Skills.Physical;
using Entity.Skills.Spells;
using Game.Audio;
using Game.Battle.UI;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Battle 
{
    public class SelectionUI : MonoBehaviour
    {
        [SerializeField] private PlayerInputWrapper input;
        private Databases databases;
        private SaveData saveData;
        
        private bool selectionOpened;
        private int currentMenu = 0;
        [SerializeField] private BattleMenuScript[] menus;
        [SerializeField] private BattleTopBar topBar;
        [SerializeField] private Animator cameraAnimator;

        [SerializeField] private BattleController battleController;
        [SerializeField] private PartyStatusUI partyStatus;
        [SerializeField] private BattleConfirmMenu confirmMenu;
        [SerializeField] private BattleChangeMenu changeMenu;
        [SerializeField] private SFXController sfxController;
        [SerializeField] private BattleDialogueController battleDialogue;
        
        [Header("Move Icons")] 
        [SerializeField] private Sprite guardIcon;
        
        private bool doneSelectingThisMember;
        private bool acceptPressed;
        private bool cancelPressed;
        private int currentPartySelectionID;
        private bool isSelectingEnemy;
        private bool moveMenuOpened;
        private Skill selectedSkill;
        private Coroutine currentBattleSelectionCoroutine;
        private float inputCooldown = 0.03f;
        private float currentInputCooldown;
        
        
        private void Awake()
        {
            databases = FindObjectOfType<Databases>();
            saveData = FindObjectOfType<SaveData>();
            input.GetMovementAction().performed += Movement;
            input.GetInteract().performed += _ => Interact();
            input.GetCancel().performed += _ => Cancel();
        }

        public void OpenMainMenu()
        {
            selectionOpened = true;
            menus[0].gameObject.SetActive(true);
            menus[0].OnOpen(this);
        }

        private void Movement(InputAction.CallbackContext ctx)
        {
            if (!selectionOpened || currentInputCooldown > 0.0f) return;
            Vector2 movement = ctx.ReadValue<Vector2>();
            
            if (confirmMenu.IsOpened())
            {
                if (movement.x != 0f)
                    confirmMenu.Movement();
            }
            else if (moveMenuOpened)
            {
                changeMenu.Movement(movement);
            }
            else
                menus[currentMenu].Movement(movement);

            currentInputCooldown = inputCooldown;
        }

        private void Update()
        {
            if (currentInputCooldown > 0.0f)
            {
                currentInputCooldown -= Time.deltaTime;
            }
        }

        private void Interact()
        {
            if (!selectionOpened) return;
            if (confirmMenu.IsOpened())
            {
                confirmMenu.Selection();
            }

            else if (moveMenuOpened)
            {
                changeMenu.Select();
            }
            else
            {
                acceptPressed = true;
                menus[currentMenu].OnAccept();
            }
        }

        private void Cancel()
        {
            if (moveMenuOpened)
            {
                changeMenu.Cancel();
            }
            else
            {
                if (!selectionOpened) return;
            
                if (currentMenu > 0)
                {
                    // Player pressed back inside of a menu that isn't the main menu
                    cancelPressed = true;
                }
            
                menus[currentMenu].OnCancel();
            }
        }

        public void OpenFight()
        {
            menus[currentMenu].OnClose();

            // Fight menu
            StartCoroutine(FightMenu());
        }

        private IEnumerator partyFightMenu(int partyMember)
        {
            doneSelectingThisMember = false;
            acceptPressed = false;
            
            battleController.SetBattleUIPhase(false, false, true, true);
            if (partyMember < 0) partyMember = 0;
                Entity.Entity entity = saveData.entityStock[saveData.partySlots[partyMember]];
                
                // Visibly select icon of attacking
                partyStatus.SelectPartyMember(partyMember);

                // Open menu relevant to player
                if (entity is null){
                    doneSelectingThisMember = true;
                    yield break;
                }

                if (entity is Protagonist protag)
                {
                    acceptPressed = false;
                    while (true)
                    {
                        ChangeMenu(1);
                        while (!acceptPressed)
                        {
                            yield return null;
                        }

                        acceptPressed = false;
                        // What did we press?
                        int selected = menus[currentMenu].GetSelectedID();
                        if (selected == 0)
                        {
                            // Opened skill menu
                            if(!CanOpenSkillMenu(protag))
                                continue;
                            
                            yield return StartCoroutine(SkillMenu(protag, partyMember, 1));
                            break;
                        }
                        else if (selected == 1)
                        {
                            // Opened comp menu
                        }
                        else if (selected == 2)
                        {
                            // Opened item menu
                        }
                        else if (selected == 3)
                        {
                            // Opened move menu
                            yield return StartCoroutine(OpenChangeMenu(false, 0));
                        }
                        else if (selected == 4)
                        {
                            // Opened guard menu
                            yield return StartCoroutine(GuardChoice(partyMember));
                            break;
                        }
                    }
                }
                else if (entity is Human human)
                {
                    
                    while (true)
                    {
                        ChangeMenu(2);
                        while (!acceptPressed)
                        {
                            yield return null;
                        }

                        acceptPressed = false;
                        // What did we press?
                        int selected = menus[currentMenu].GetSelectedID();
                        if (selected == 0)
                        {
                            // Opened skill menu
                            if (!CanOpenSkillMenu(human))
                                continue;
                            
                            yield return StartCoroutine(SkillMenu(human, partyMember, 2));
                            break;
                        }
                        else if (selected == 1)
                        {
                            // Opened item menu
                        }
                        else if (selected == 2)
                        {
                            // Opened move menu
                            yield return StartCoroutine(OpenChangeMenu(false, 0));
                        }
                        else if (selected == 3)
                        {
                            // Opened guard menu
                            yield return StartCoroutine(GuardChoice(partyMember));
                            break;
                        }
                    }
                }
                else if (entity is Demon demon)
                {
                    while (true)
                    {
                        ChangeMenu(3);
                        while (!acceptPressed)
                        {
                            yield return null;
                        }

                        acceptPressed = false;
                        // What did we press?
                        int selected = menus[currentMenu].GetSelectedID();
                        if (selected == 0)
                        {
                            // Opened Skill menu
                            if (!CanOpenSkillMenu(demon))
                                continue;
                            
                            yield return StartCoroutine(SkillMenu(demon, partyMember, 3));
                            break;
                        }
                        else if (selected == 1)
                        {
                            // Opened move menu
                            yield return StartCoroutine(OpenChangeMenu(true, partyMember));
                        }
                        else if (selected == 2)
                        {
                            // Opened guard
                            yield return StartCoroutine(GuardChoice(partyMember));
                            break;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Player was something else!?!!?!? Type: " + entity.GetType());
                    yield break;
                }

                // Analyze should be it's own button, usable by any party member
                // Attack button will merge Attack/Sword, Gun, Extra, and Magic buttons
                battleController.SetBattleUIPhase(false, false, false, true);
                doneSelectingThisMember = true;
        }

        private IEnumerator OpenChangeMenu(bool canSelectFirstDemon, int forcedFirstSelect)
        {
            moveMenuOpened = true;
            yield return StartCoroutine(changeMenu.OpenMenu(canSelectFirstDemon, forcedFirstSelect));
            moveMenuOpened = false;
        }

        private bool CanOpenSkillMenu(Entity.Entity entity)
        {
            if (entity is Human human)
            {
                if (human.equipment.meleeWeapon == null &&
                    (human.equipment.firearmWeapon == null || human.equipment.bullet == null) && (human.currentGuardian == null || human.currentGuardian.skills.Length == 0))
                {
                    Debug.Log("Player tried opening skills menu without any skills and/or equipment.");
                    return false;
                }
            }
            else
            {
                if (entity.skills.Length == 0)
                {
                    Debug.Log("Player tried opening skills menu without any skills");
                    return false;
                }
            }

            return true;
        }

        private IEnumerator SkillMenu(Entity.Entity entity, int partyMember, int fightMenuID)
        {
            // Make sure we can even open the skill menu in the first place
            
            // Opened skill menu
            if (!CanOpenSkillMenu(entity))
            {
                yield break;
            }
            
            while (true)
            {
                yield return StartCoroutine(SelectSkill(entity, partyMember));
                // Process skill
                
                selectedSkill = null;
                if (selectedSkill == null)
                {
                    ChangeMenu(fightMenuID);
                }else
                    CloseMenu();
                yield break;
            }
        }

        private IEnumerator GuardChoice(int partyMember)
        {
            BattleMove move = new BattleMove
            {
                moveType = BattleMoveType.Guard
            };
            battleController.SetPartyBattleMove(move, partyMember);
            partyStatus.SetPartyMemberMove(partyMember, guardIcon, databases.GetTranslatedSkill("generic_defend"));
            yield break;
        }

        public void OpenComp()
        {
            StartCoroutine(CompChoice());
        }
        
        private IEnumerator CompChoice()
        {
            if (!saveData.hasComp)
            {
                battleController.SetBattleUIPhase(false, false, false, false);
                
                menus[currentMenu].gameObject.SetActive(false);
                yield return StartCoroutine(battleDialogue.ReadString("system", databases.GetTranslatedText("battle_no_comp")));
                menus[currentMenu].gameObject.SetActive(true);
                
                battleController.SetBattleUIPhase(true, true, false, true);
                yield break;
            }
            
            // TODO add comp functionality here

            yield return null;
        }

        public void OpenTalk()
        {
            StartCoroutine(TalkChoice());
        }

        private IEnumerator TalkChoice()
        {
            if (!saveData.hasComp)
            {
                battleController.SetBattleUIPhase(false, false, false, false);
                
                menus[currentMenu].gameObject.SetActive(false);
                yield return StartCoroutine(battleDialogue.ReadString("system", databases.GetTranslatedText("battle_no_comp")));
                menus[currentMenu].gameObject.SetActive(true);
                
                battleController.SetBattleUIPhase(true, true, false, true);
                yield break;
            }
            
            // TODO add comp functionality here
            yield return null;
        }

        private IEnumerator FightMenu()
        {
            bool hasSelectedAll = false;

            while (!hasSelectedAll)
            {
                Coroutine coroutine = null;
                while (currentPartySelectionID < saveData.partySlots.Length)
                {
                    if (saveData.partySlots[currentPartySelectionID] == -1 || saveData.entityStock[saveData.partySlots[currentPartySelectionID]].health <= 0)
                    {
                        currentPartySelectionID++;
                        continue;
                    }
                    doneSelectingThisMember = false;
                    cancelPressed = false;
                    if(coroutine != null) StopCoroutine(coroutine);
                     coroutine = StartCoroutine(partyFightMenu(currentPartySelectionID));
                    while (true)
                    {
                        if (doneSelectingThisMember || cancelPressed) break;
                        yield return null;
                    }
                    if (cancelPressed)
                    {
                        if (currentMenu > 3)
                        {
                            // User exited out of sub-menu
                            // (Skill select, item select, etc)
                            // Bring them to the main menu for the current entity instead
                            
                            if(coroutine != null) StopCoroutine(coroutine);
                            coroutine = StartCoroutine(partyFightMenu(currentPartySelectionID));
                        }
                        else
                        {
                            if(coroutine != null) StopCoroutine(coroutine);
                        
                            // Either way we're cancelling this entity's choice, so hide the bar for it
                            partyStatus.HideMemberMove(currentPartySelectionID);
                        
                            // Get a list of all party members in order of party ID while keeping their spot
                            List<int> partyList = new List<int>();

                            for (int i = 0; i < saveData.partySlots.Length; i++)
                            {
                                if(saveData.partySlots[i] != -1 && saveData.entityStock[saveData.partySlots[i]].health > 0)
                                    partyList.Add(i);
                            }
                        
                            // Find the ID of the user before the current one
                            for (int i = 0; i < partyList.Count; i++)
                            {
                                // Find current ID
                                if (partyList[i] == currentPartySelectionID)
                                {
                                    if (i > 0)
                                    {
                                        // We have someone behind
                                        currentPartySelectionID = partyList[i - 1];
                                        battleController.ResetMove(currentPartySelectionID);
                                    }
                                    else
                                    {
                                        // This is the top, exit back to main menu
                                        battleController.ResetMove(currentPartySelectionID);
                                        partyStatus.DeselectAllMembers();
                                        partyStatus.UnHideAllPartyMembers();
                                        currentPartySelectionID = 0;
                                        battleController.SetBattleUIPhase(true, true, false, true);
                                        ChangeMenu(0);
                                        cancelPressed = false;
                                        acceptPressed = false;
                                        yield break;
                                    }
                                }
                            }
                        }
                    }
                    else currentPartySelectionID++;
                }
            
                // All party members moves have been selected 
                // Hide selection menu
                partyStatus.DeselectAllMembers();
                CloseMenu();
                // Hide top bar
                battleController.SetBattleUIPhase(false, false ,false, true);
                confirmMenu.OpenMenu();
                while (confirmMenu.IsOpened())
                {
                    yield return null;
                }
                // Player must have confirmed move
                if (confirmMenu.GetSelected() == 0)
                {
                    // Battle stuff here
                    battleController.ChangeBattlePhase(BattlePhase.Attacking);
                    partyStatus.UnHideAllPartyMembers();
                    battleController.StartActions();
                    currentPartySelectionID = 0;
                    hasSelectedAll = true;
                }
                else 
                {
                    // Re-choose for the last party member
                    acceptPressed = false;
                    // Reset moves of all infront of this number
                    battleController.ResetMove(saveData.partySlots.Length - 1);
                    
                    // Keep subtracting currentPartySelectionID until there is a valid member
                    while (true)
                    {
                        if (currentPartySelectionID >= saveData.partySlots.Length || saveData.partySlots[currentPartySelectionID] == -1)
                        {
                            currentPartySelectionID--;
                            continue;
                        }
                        else break;
                    }
                }
            }
        }

        private IEnumerator SelectSkill(Entity.Entity entity, int partyID)
        {
            ChangeMenu(4);
            if (menus[4] is BattleSkillMenu skillMenu)
            {
                skillMenu.LoadSkillMenu(entity, databases);
                Skill lastSelectedSkill = null;
                while (skillMenu.isOpened)
                {
                    if (skillMenu.GetSelectedNum() >= 0 && skillMenu.GetNumberOfObjects() > 0)
                    {
                        Skill currentSelectedSkill = skillMenu.GetSkillAtPosition(skillMenu.GetSelectedNum());
                    
                        if (currentSelectedSkill != lastSelectedSkill)
                        {
                            switch (currentSelectedSkill)
                            {
                                // Changed selected skill
                                // Show affinity icons on all demons
                                case PhysicalSkill:
                                    //Debug.Log("Physical skill");
                                    battleController.ShowAllEnemyAffinities(Element.Physical);
                                    break;
                                case Spell spell when spell.element != Element.Support && spell.element != Element.Recovery:
                                    //Debug.Log("Spell selected");
                                    battleController.ShowAllEnemyAffinities(spell.element);
                                    break;
                                case Spell:
                                    // Hide all affinities
                                    battleController.HideAllEnemyAffinities();
                                    break;
                            }

                            lastSelectedSkill = currentSelectedSkill;
                        }
                    }
                    yield return null;
                }
                
                // Hide all entity affinity icons
                battleController.HideAllEnemyAffinities();
                selectedSkill = skillMenu.GetSelectedSkill();
                skillMenu.ResetSkill();
                
                // Select an enemy to use it on
                if (selectedSkill == null)
                {
                    
                }
                else
                {
                    CloseMenu();
                    //BattleEnemySelection enemySelection = menus[5] as BattleEnemySelection;
                    ChangeMenu(5);
                    // Skill is single target only
                    isSelectingEnemy = true;
                    Element? element = null;
                    if (selectedSkill is PhysicalSkill)
                        element = Element.Physical;
                    else if (selectedSkill is Spell sp)
                        element = sp.element;
                    switch (selectedSkill.targetType)
                    {
                        case BattleEnemySelection.SelectionType.Single:
                            yield return battleController.SelectEnemy(BattleEnemySelection.SelectionType.Single, element);
                            break;
                        case BattleEnemySelection.SelectionType.AllRow:
                            yield return battleController.SelectEnemy(BattleEnemySelection.SelectionType.AllRow, element);
                            break;
                        case BattleEnemySelection.SelectionType.AllParty:
                            yield return battleController.SelectEnemy(BattleEnemySelection.SelectionType.AllParty, element);
                            break;
                    }
                    isSelectingEnemy = false;
                    CloseMenu();
                    int[] enemiesSelected = battleController.GetLastSelectedEntities();
                    BattleMove move = new BattleMove()
                    {
                        moveType = BattleMoveType.Skill,
                        usingSkill = selectedSkill,
                        enemiesEffected = enemiesSelected
                    };
                    battleController.SetPartyBattleMove(move, partyID);
                    Sprite icon;
                    if (selectedSkill is Spell spell)
                        icon = databases.GetElementSprite(spell.element);
                    else icon = databases.GetElementSprite(Element.Physical);
                    partyStatus.SetPartyMemberMove(partyID, icon, databases.GetTranslatedSkill(selectedSkill.skillKey));
                    battleController.ResetLastEntitySelection();
                }
            }
            else
            {
                Debug.LogError("Wasn't skill menu");
            }
            yield return null;
        }

        /// <summary>
        /// Opens the menu of a specific panel
        /// 0 = Main Menu
        /// 1 = Protagonist Menu
        /// 2 = Human Party Menu
        /// 3 = Demon Menu
        /// 4 = Skill Menu
        /// 5 = Enemy Selection Menu
        /// </summary>
        /// <param name="menuID">The menu to change to.</param>
        private void ChangeMenu(int menuID)
        {
            menus[currentMenu].OnClose();
            menus[currentMenu].gameObject.SetActive(false);
            
            menus[menuID].gameObject.SetActive(true);
            menus[menuID].OnOpen(this);
            currentMenu = menuID;
        }
        
        private void CloseMenu()
        {
            menus[currentMenu].OnClose();
            menus[currentMenu].gameObject.SetActive(false);
            currentMenu = 0;
        }

        public void SetTopBarText(string text)
        {
            topBar.SetBarText(text);
        }

        public Databases GetDatabase() => databases;
    }
}