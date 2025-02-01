using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Database;
using Entity;
using Entity.Skills;
using Entity.Skills.Physical;
using Entity.Skills.Spells;
using Game;
using Game.Audio;
using Game.Battle;
using Game.Battle.UI;
using SpriteUtils;
using UnityEngine;

namespace Battle
{
    public enum BattleType
    {
        RandomEncounter,
        Boss,
        Fiend
    }
    
    public enum BattlePhase
    {
        Empty,
        Talking,
        Attacking,
        Finished,
        LevelUp
    }

    public enum BattleMoveType
    {
        Unselected,
        Skill,
        COMP,
        Item,
        Move,
        Guard,
        Return
    }

    [Serializable]
    public struct BattleMove
    {
        public BattleMoveType moveType;
        
        [Header("Attack")]
        public Skill usingSkill;
        /// <summary>
        /// An array of ints for each ID of enemy effected
        /// </summary>
        public int[] enemiesEffected;

        // Don't need to have values for guard or return
    }

    public class BattleController : MonoBehaviour
    {
        [Serializable]
        public struct Enemy
        {
            public Transform demonObject;
            public Demon demonStats;
            public GameObject demonPlane;
            public BattleSpriteLoader spriteLoader;
            public int arrayID;
        }

        [SerializeField] private BattleTopBar battleTopBar;
        [SerializeField] private BattleSpawner spawner;
        [SerializeField] private BattleMoveUI moveUI;
        private SaveData saveData;
        [SerializeField] private BattleMusic battleMusic;
        [SerializeField] private SelectionUI selectionUI;
        private static readonly int MAX_ENEMIES_IN_ROW = 8;
        private BattlePhase currentBattlePhase = BattlePhase.Empty;
        private BattleMove[] battleMoves;
        [SerializeField] private BattlePartyStats battlePartyStats;
        [SerializeField] private BattleStats battleStats;
        [SerializeField] private PartyStatusUI partyStatusUI;
        [SerializeField] private BattleEnemySelection enemySelector;
        private Databases database;
        [SerializeField] private SFXController sfxController;
        [SerializeField] private BattleResults battleResults;

        [SerializeField] [Range(0f, 1f)] private float backRowAttackPercentage = 0.35f;
        [SerializeField] private float entitySelectionTime;
        [SerializeField] private Enemy[] enemies = new Enemy[MAX_ENEMIES_IN_ROW * 2];
        [SerializeField] private MoonPhaseSystem moonPhaseSystem;
        
        [Header("Floating Damage Settings")]
        [SerializeField] private GameObject floatingDamagePrefab;
        [SerializeField] private float floatingDamageKeepTime;
        [SerializeField] [Range(0f, 1f)] private float yellowDamageRange;
        [SerializeField] [Range(0f, 1f)] private float redDamageRange;
        [SerializeField] private Color whiteHealthColour;
        [SerializeField] private Color yellowHealthColour;
        [SerializeField] private Color redHealthColour;

        private List<Entity.Entity> enemiesKilled;
        [SerializeField] private BattleType battleType;
        
        // For selecting enemies
        private int[] lastEntitySelected;

        private void Awake()
        {
            database = FindObjectOfType<Databases>();
            saveData = FindObjectOfType<SaveData>();
            
            battleMoves = new BattleMove[6];
            enemiesKilled = new List<Entity.Entity>();
            
            battlePartyStats.Initialize(saveData.GetPartyMembers());
            if (enemies != null)
                LoadBattle(enemies, battleType);

            // TODO change this if we are getting ambushed
            ChangeBattlePhase(BattlePhase.Talking);
        }

        private void Start()
        {
            selectionUI.OpenMainMenu();
        }

        public void LoadBattle(Enemy[] enemyList, BattleType battleType)
        {
            this.battleType = battleType;
            // Setup UI
            moonPhaseSystem.SetMoonPhase(saveData.moonPhase);
            battleStats.SetStatus(saveData.macca, saveData.magnetite);

            Enemy[] newEnemyList = new Enemy[enemyList.Length];

            if (enemyList.Length > (MAX_ENEMIES_IN_ROW * 2f))
            {
                Debug.LogError("Tried to load more enemies than allowed in a row.");
                return;
            }

            for (int i = 0; i < enemyList.Length; i++)
            {
                if (enemyList[i].demonStats == null) continue;
                
                Demon demon = Instantiate(enemyList[i].demonStats);
                demon.name = demon.entityName;
                demon.health = demon.GetMaxHP();
                demon.mana = demon.GetMaxMana();
                newEnemyList[i].demonStats = demon;
            }

            enemies = newEnemyList;
            
            spawner.LoadEnemies(newEnemyList, this);
        }

        public Enemy[] GetEnemies() => enemies;

        public void ChangeBattlePhase(BattlePhase phase)
        {
            if (phase != currentBattlePhase)
            {
                currentBattlePhase = phase;
                battleMusic.PlayMusic(phase, battleType);
            }
        }

        public void SetPartyBattleMove(BattleMove move, int partyID)
        {
            battleMoves[partyID] = move;
        }

        private struct Ordered
        {
            public bool isPlayerParty;
            public bool isEnemyBackRow;
            public int partyID;
            public int agilityStat;
        }

        public void StartActions()
        {
            for (int i = 0; i < battleMoves.Length; i++)
            {
                if (saveData.partySlots[i] == -1)
                    continue;

                // Current entity is real 
            }
            
            // Create list for sorting
            List<Ordered> entity = new List<Ordered>();

            // ADD ALL ENTITIES IN BATTLE TO LIST
            for (int i = 0; i < enemies.Length; i++)
            {
                Demon enemy = enemies[i].demonStats;
                if (enemy == null) continue;
                Ordered enemyOrdered = new Ordered()
                {
                    isEnemyBackRow = i >= MAX_ENEMIES_IN_ROW / 2,
                    isPlayerParty = false,
                    partyID = i,
                    agilityStat = enemy.stats.agility
                };
                entity.Add(enemyOrdered);
            }

            List<Ordered> prioritizedList = new List<Ordered>();
            
            for (int i = 0; i < saveData.partySlots.Length; i++)
            {
                if (saveData.partySlots[i] == -1 || saveData.entityStock[saveData.partySlots[i]].health <= 0)
                    continue;
                
                Entity.Entity ent = saveData.entityStock[saveData.partySlots[i]];
                if (ent == null) continue;
                // Check if the player is guarding, using an item, or anything else that shouldn't be 
                // affected by agility and instead applied at turn start
                
                if (battleMoves[i].moveType == BattleMoveType.Guard)
                {
                    // Player is guarding!
                    battlePartyStats.stats[i].isGuarding = true;
                    continue;
                }

                // Player is not doing such
                Ordered partyOrdered = new Ordered()
                {
                    isEnemyBackRow = false,
                    isPlayerParty = true,
                    partyID = i,
                    agilityStat = ent.stats.agility
                };
                
                // Check prioritization
                if (battleMoves[i].moveType == BattleMoveType.COMP)
                {
                    // Add to prioritized list
                    prioritizedList.Add(partyOrdered);
                    continue;
                }
                
                // Don't prioritize
                entity.Add(partyOrdered);
            }
            
            // SORT THE LIST BY AGILITY
            List<Ordered> orderedList = entity.OrderByDescending(stat => stat.agilityStat).ToList();
            string text = "";
            foreach (Ordered ordered in orderedList)
            {
                text += "" + ordered.agilityStat + ", ";
            }
            StartCoroutine(ProcessTurn(prioritizedList, orderedList));
        }

        private IEnumerator ProcessTurn(List<Ordered> prioritized, List<Ordered> orderedList)
        {
            // Players that are guarding were not added to the list
            // Players that are using the COMP should be prioritized
            // Items should not be
            // Demons returning?
            // Attacks obviously shouldn't be

            foreach (Ordered entity in prioritized)
            {
                // Has to be a player, probably
                // Do these first!
                yield return StartCoroutine(DoBattleMove(entity));
            }

            foreach (Ordered entity in orderedList)
            {
                yield return StartCoroutine(DoBattleMove(entity));
            }

            // Turns are done, re-open move menu for the player
            SetBattleUIPhase(false, false, true, true);
            selectionUI.OpenFight();
            // Reset all player moves
            for (int i = 0; i < battleMoves.Length; i++)
            {
                battleMoves[i] = new BattleMove();
            }
            partyStatusUI.HideAllMemberMoves();
            partyStatusUI.UnHideAllPartyMembers();
        }

        private IEnumerator DoBattleMove(Ordered entityOrdered)
        {
            // This is the specific entity's move. Nothing more than that. This will be called at least twice a battle
            // depending on how many allies and enemies there are.
            
            string moveBarName;
            
            if (entityOrdered.isPlayerParty)
            {
                if (entityOrdered.partyID == -1 || saveData.partySlots[entityOrdered.partyID] == -1)
                    yield break;
                
                Entity.Entity entity = saveData.entityStock[saveData.partySlots[entityOrdered.partyID]];
                
                // Check if entity is alive
                if (entity.health <= 0)
                    yield break;
                
                
                // Party member
                
                
                // Check to see what the player is doing. They might be using an item or something
                if (battleMoves[entityOrdered.partyID].moveType != BattleMoveType.Skill)
                {
                    
                    // Select party member on the UI, gray out all rest
                    partyStatusUI.SelectPartyMember(entityOrdered.partyID);
                    
                    switch (battleMoves[entityOrdered.partyID].moveType)
                    {
                        case BattleMoveType.Item:
                            // Show move bar with item name and icon
                            moveBarName = entityOrdered.partyID + " used an item!";
                            yield return StartCoroutine(moveUI.StartMove(moveBarName, true));
                        
                            // TODO check if the item is a combat item
                        
                            // Do some animation+sound for the item being used
                        
                            // Apply values
                        
                            yield break;
                        case BattleMoveType.Move:
                            // Show move bar with attacks
                            // Show move bar with appropriate move data
                            moveBarName = entityOrdered.partyID + " attacks!";
                            yield return StartCoroutine(moveUI.StartMove(moveBarName, true));
                            break;
                        case BattleMoveType.Return:
                            moveBarName = entityOrdered.partyID + " was returned!";
                            yield return StartCoroutine(moveUI.StartMove(moveBarName, true));
                            break;
                        case BattleMoveType.COMP:
                            break;
                        default:
                            Debug.LogError("Action was not accounted for.");
                            break;
                    }
                }
                else
                {
                    // Check to make sure there are entities left alive that we are targeting
                    int aliveAttackingEnemies = 0;
                    if (battleMoves[entityOrdered.partyID].enemiesEffected == null)
                    {
                        yield break;
                    }
                    foreach (int enemy in battleMoves[entityOrdered.partyID].enemiesEffected)
                    {
                        if (enemies[enemy].demonObject != null)
                        {
                            aliveAttackingEnemies++;
                        }
                    }

                    if (aliveAttackingEnemies == 0)
                    {
                        // No targets alive, skip skill
                        partyStatusUI.SelectPartyMember(entityOrdered.partyID);
                        yield return StartCoroutine(moveUI.StartMove(database.GetTranslatedText("battle_no_target"), true));
                        yield return StartCoroutine(moveUI.EndMoveAnimation());
                        partyStatusUI.HideMemberMove(entityOrdered.partyID);
                        partyStatusUI.UnHideAllPartyMembers();
                        yield break;
                    }
                    
                    // Select party member on the UI, gray out all rest
                    partyStatusUI.SelectPartyMember(entityOrdered.partyID);

                    Skill usingSkill = battleMoves[entityOrdered.partyID].usingSkill;
                    string entityName = "";
                    entityName = entity is Protagonist ? saveData.playerName : database.GetTranslatedName(entity.entityName);
                    moveBarName = "";
                    
                    // Make sure the player has enough HP/MP to execute the skill
                    bool canUseSkill = true;
                    if (usingSkill is PhysicalSkill physical)
                    {
                        float amountRequired = Mathf.CeilToInt((entity.GetMaxHP() * (physical.hpCostPercentage / 100f)));
                        if (entity.health - amountRequired <= 0)
                        {
                            canUseSkill = false;
                            moveBarName = entityName + " has insufficient HP.";
                        }
                    }
                    else if (usingSkill is Spell usingSpell)
                    {
                        if (entity.mana - usingSpell.mpCost < 0)
                        {
                            canUseSkill = false;
                            moveBarName = entityName + " has insufficient MP.";
                        }
                    }

                    if (!canUseSkill)
                    {
                        // Can't use skill
                        yield return StartCoroutine(moveUI.StartMove(moveBarName, true));

                        yield return new WaitForSeconds(0.2f);
                        
                        // Hide move bar
                        yield return StartCoroutine(moveUI.EndMoveAnimation());
                
                        // Remove the skill from the UI
                        partyStatusUI.HideMemberMove(entityOrdered.partyID);
                        partyStatusUI.UnHideAllPartyMembers();
                        yield break;
                    }
                    
                    
                    // Player is performing a skill
                    moveBarName = entityName + " used " + database.GetTranslatedSkill(usingSkill.skillKey) + "!";
                    
                    // Take away HP/MP depending on skill cost
                    if (usingSkill is PhysicalSkill physicalSkill)
                    {
                        entity.health -= Mathf.CeilToInt((entity.GetMaxHP() * (physicalSkill.hpCostPercentage / 100f)));
                        partyStatusUI.DealDamage(entityOrdered.partyID, entity, false);
                    }else if (usingSkill is Spell spell)
                    {
                        entity.mana -= (spell.mpCost);
                        partyStatusUI.UpdateStatPanel(entityOrdered.partyID);
                    }
                    partyStatusUI.LoseMana(entityOrdered.partyID);
                    yield return StartCoroutine(moveUI.StartMove(moveBarName, true));
                    // TODO ATTACK ANIMATION STUFF

                    bool isKillingAnEnemy = false;
                    foreach (int enemy in battleMoves[entityOrdered.partyID].enemiesEffected)
                    {
                        if (enemies[enemy].demonObject == null)
                        {
                            // Demon was dead
                            //Debug.Log("Tried attacking a demon that was dead.");
                            continue;
                        }

                        // TODO Do code for null, repel, absorb here
                        switch (enemies[enemy].demonStats
                                .GetAffinityToElement(battleMoves[entityOrdered.partyID].usingSkill.element))
                        {
                            case ResistanceType.Null:
                            {
                                GameObject floatingDamage = Instantiate(floatingDamagePrefab);
                                FloatingDamageNumber floatingDamageNumber = floatingDamage.GetComponent<FloatingDamageNumber>();
                                floatingDamageNumber.InitializeText(database.GetTranslatedText("battle_affinity_null"), whiteHealthColour, floatingDamageKeepTime);
                                Vector3 spawnPosition = enemies[enemy].demonPlane.transform.position;
                                spawnPosition.z -= 0.1f;
                                // Determine Y position
                                floatingDamage.transform.position = spawnPosition;
                                break;
                            }
                            case ResistanceType.Absorb:
                            {
                                
                                break;
                            }
                            case ResistanceType.Repel:
                            {
                                
                                break;
                            }
                            default:
                            {
                                // Calculate damage
                                int damageTaken = BattleFormuals.CalculateDamage(
                                    saveData.GetEntityFromStockID(saveData.partySlots[entityOrdered.partyID]),
                                    enemies[enemy].demonStats, battleMoves[entityOrdered.partyID].usingSkill, false);
                                
                                enemies[enemy].demonStats.health -= damageTaken;
                        
                                // Do visuals
                                Animator animator = enemies[enemy].demonObject.GetComponent<Animator>();
                                animator.Play("EnemyHit_01");
                                GameObject floatingDamage = Instantiate(floatingDamagePrefab);
                                
                                // Play sound
                                sfxController.PlaySound("enemy_hit");
                                
                                FloatingDamageNumber floatingDamageNumber = floatingDamage.GetComponent<FloatingDamageNumber>();
                        
                                // Find colour to use for the floating damage indicator
                                Color useColour;
                                // Use the percentage of health AFTER the damage has been dealt
                                float healthPercentage =
                                    (enemies[enemy].demonStats.health * 1f / enemies[enemy].demonStats.GetMaxHP());
                                if (healthPercentage < redDamageRange)
                                    useColour = redHealthColour;
                                else if (healthPercentage < yellowDamageRange)
                                    useColour = yellowHealthColour;
                                else useColour = whiteHealthColour;
                                floatingDamageNumber.InitializeNumber(damageTaken, useColour, floatingDamageKeepTime);
                                Vector3 spawnPosition = enemies[enemy].demonPlane.transform.position;
                                spawnPosition.z -= 0.1f;
                                // Determine Y position
                                floatingDamage.transform.position = spawnPosition;
                                enemies[enemy].demonObject.GetComponent<BattleSpriteLoader>().StartHitEffect(.25f);
                                break;
                            }
                        }

                        // Check if entity is dead and kill it if needed
                        if (enemies[enemy].demonStats.health <= 0)
                        {
                            isKillingAnEnemy = true;
                        }
                    }
                    
                    // Check if entity is dead and kill it if needed
                    if (isKillingAnEnemy)
                    {
                        yield return new WaitForSeconds(0.2f);
                        foreach (int enemy in battleMoves[entityOrdered.partyID].enemiesEffected)
                        {
                            if (enemies[enemy].demonObject != null && enemies[enemy].demonStats.health <= 0)
                            {
                                // Kill entity
                                KillEnemy(enemy);
                            }
                        }
                        sfxController.PlaySound("enemy_death");
                        
                        // Check for end of round
                        yield return StartCoroutine(CheckEndConditions());
                    }
                }
                
                // Hide move bar
                yield return StartCoroutine(moveUI.EndMoveAnimation());
                
                // Remove the skill from the UI
                partyStatusUI.HideMemberMove(entityOrdered.partyID);
                partyStatusUI.UnHideAllPartyMembers();
            }
            else
            {
                // Enemy
                Demon demon = enemies[entityOrdered.partyID].demonStats;
                
                // Check if demon is still alive
                if (demon == null || demon.health <= 0)
                {
                    yield break;
                }
                
                Transform demonSprite = spawner.GetTransform(entityOrdered.partyID);
                
                // TODO maybe focus on enemy somehow? spotlight? idk
                // "Deselect" all other enemies
                float selectionNumber = 1f;
                while (selectionNumber > 0f)
                {
                    // Change value
                    selectionNumber -= entitySelectionTime * Time.deltaTime;
                    // Update on all entities
                    foreach (BattleSpriteLoader sprite in spawner.GetEnemyRenderers())
                    {
                        if (sprite == null) continue;
                        if(sprite.transform == demonSprite) continue;
                    }
                    yield return null;
                }
                foreach (BattleSpriteLoader sprite in spawner.GetEnemyRenderers())
                {
                    if (sprite == null) continue;
                    if(sprite.transform == demonSprite) continue;
                }
                
                // Use AI to pick skill and enem(ies) to use it on
                Skill demonSkill = demon.aiType.DecideAttack(demon, demon.health, entityOrdered.isEnemyBackRow);
                // Enemy IDs of who is getting attacked
                Entity.Entity[] partySlots = saveData.GetPartyMembers();
                if (partySlots == null)
                {
                    Debug.LogError("Party slots were null.");
                    yield break;
                }

                int[] attackedEntities = { };
                if (demonSkill == null)
                {
                    // Skill came back null, so attack normally instead
                    PhysicalSkill meleeSkill = ScriptableObject.CreateInstance<PhysicalSkill>();
                    meleeSkill.element = Element.Physical;
                    meleeSkill.attackPower = demon.GetAttackPower();
                    meleeSkill.hpCostPercentage = 0;
                    meleeSkill.targetType = demon.meleeAttackType;
                    demonSkill = meleeSkill;
                    
                    attackedEntities = demon.aiType.DecideAttacking(partySlots, demon.meleeAttackType, backRowAttackPercentage, true);
                    moveBarName = (database.GetTranslatedName(demon.entityName) + " attacks!");
                } 
                else
                {
                    if (demonSkill.targetType == BattleEnemySelection.SelectionType.None)
                    {
                        moveBarName = database.GetTranslatedName(demon.entityName) + " used " + database.GetTranslatedSkill(demonSkill.skillKey) + "!";
                    }else
                    {
                        attackedEntities = demon.aiType.DecideAttacking(partySlots, demonSkill.targetType, backRowAttackPercentage, false);
                        moveBarName = database.GetTranslatedName(demon.entityName) + " used " + database.GetTranslatedSkill(demonSkill.skillKey) + "!";
                    }
                }
                
                // attackedEntities array is the array of numbers of which playerIDs are being attacked.
                
                // Show move bar
                yield return StartCoroutine(moveUI.StartMove(moveBarName, false));
                
                // TODO Battle animation

                if (attackedEntities.Length > 0)
                {
                    foreach (int party in attackedEntities)
                    {
                        Entity.Entity attackedParty = saveData.entityStock[saveData.partySlots[party]];
                        
                        // Check if we are taking damage
                        if (demonSkill is PhysicalSkill or MagicSpell)
                        {
                            ResistanceType resistanceType = ResistanceType.Normal;
                            switch (demonSkill)
                            {
                                case PhysicalSkill:
                                    resistanceType = attackedParty.GetAffinityToElement(Element.Physical);
                                    break;
                                case MagicSpell magicSpell:
                                    resistanceType = attackedParty.GetAffinityToElement(magicSpell.element);
                                    break;
                            }

                            switch (resistanceType)
                            {
                                case ResistanceType.Absorb:
                                {
                                    Debug.Log("Got hit and absorbed the attack, but the code doesn't exist yet");
                                    break;
                                }
                                case ResistanceType.Null:
                                {
                                    Debug.Log("Nulled attack but no visuals yet.");
                                    break;
                                }
                                case ResistanceType.Repel:
                                {
                                    Debug.Log("Got hit and repelled the attack, but the code doesn't exist yet");
                                    break;
                                }
                                default:
                                {
                                    int damageTaken = BattleFormuals.CalculateDamage(demon, attackedParty, demonSkill, 
                                        battleMoves[party].moveType == BattleMoveType.Guard);
                            
                                    // Lose party member health but make sure it doesn't go below 0
                                    attackedParty.health = Mathf.Max(attackedParty.health - damageTaken, 0);

                                    // Do damage animation
                                    partyStatusUI.DealDamage(party, attackedParty, true);
                                    sfxController.PlaySound("party_hit");
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // TODO add conditions for things like debuffs etc
                        }
                    }
                    
                    // Check if the party members are dead
                    foreach (int party in attackedEntities)
                    {
                        Entity.Entity attackedParty = saveData.entityStock[saveData.partySlots[party]];
                        if (attackedParty.health <= 0)
                        {
                            // Party member is dead
                            if (attackedParty is Demon)
                            {
                                // Remove from party entirely
                                saveData.partySlots[party] = -1;
                                partyStatusUI.UpdateEntityInSlot(party, null);
                            }
                            else
                            {
                                // Is human, keep them in their spot but make them dead
                                partyStatusUI.SetDead(party, true);
                                yield return StartCoroutine(CheckEndConditions());
                            }
                            partyStatusUI.HideMemberMove(party);
                        }
                    }
                    // TODO Check to see if the party needs rearranging
                    // also TODO check if the player is in a game over state
                }

                // Hide move bar
                yield return StartCoroutine(moveUI.EndMoveAnimation());
            
                // Un-"deselect" all enemies
                selectionNumber = 0f;
                while (selectionNumber < 1f)
                {
                    // Change value
                    selectionNumber += entitySelectionTime * Time.deltaTime;
                    // Update on all entities
                    foreach (BattleSpriteLoader sprite in spawner.GetEnemyRenderers())
                    {
                        if (sprite == null) continue;
                    }
                    yield return null;
                }
                foreach (BattleSpriteLoader sprite in spawner.GetEnemyRenderers())
                {
                    if (sprite == null) continue;
                }
                
            }
            yield return null;
        }

        private IEnumerator CheckEndConditions()
        {
            // Check if there are still party members alive
            int partyMembersAlive = 0;
            foreach (int id in saveData.partySlots)
            {
                // Check for -1 values (the slot is empty)
                if (id == -1) continue;
                
                Entity.Entity entity = saveData.GetEntityFromStockID(id);
                if (entity != null || entity.health > 0)
                {
                    // Entity is alive
                    partyMembersAlive++;
                }
            }

            if (partyMembersAlive <= 0)
            {
                // BATTLE HAS BEEN LOST
                yield return StartCoroutine(LostBattle());
            }
            
            // Check if there are still enemies alive
            bool stillEnemiesAlive = false;
            foreach (Enemy enemy in enemies)
            {
                if (enemy.demonObject == null) continue;
                
                stillEnemiesAlive = true;
                break;
            }

            if (!stillEnemiesAlive)
            {
                // BATTLE HAS BEEN WON
                yield return StartCoroutine(WonBattle());
            }
            yield return null;
        }

        private IEnumerator LostBattle()
        {
            Debug.Log("Battle has been lost.");
            yield return null;
        }

        private IEnumerator WonBattle()
        {
            SetBattleUIPhase(false, false, false, false);
            ChangeBattlePhase(BattlePhase.Finished);
            yield return StartCoroutine(battleResults.EndResults(saveData, enemiesKilled, database, sfxController.GetVolume(), this));
            yield return null;
        }

        public void KillEnemy(int enemyID)
        {
            enemiesKilled.Add(enemies[enemyID].demonStats);
            Destroy(enemies[enemyID].demonObject.gameObject);
            enemies[enemyID] = new Enemy();
        }
        
        public void SetBattleUIPhase(bool moonVisible, bool currencyVisible, bool helpBarVisible, bool partyVisible)
        {
            battleStats.SetMoonVisible(moonVisible);
            battleStats.SetCurrencyVisible(currencyVisible);
            battleTopBar.SetBarActive(helpBarVisible);
            partyStatusUI.SetPartyUIVisible(partyVisible);
        }
        
        public void ResetMove(int partyID)
        {
            partyStatusUI.HideMemberMove(partyID);
            battleMoves[partyID] = new BattleMove();
        }

        public IEnumerator SelectEnemy(BattleEnemySelection.SelectionType selectionType, Element? element)
        {
            // Set up UI to select
            SetBattleUIPhase(false, false, true, true);
            battleTopBar.SetBarText("Select a target.");
            
            // Selection code
            yield return StartCoroutine(enemySelector.StartSelection(enemies, selectionType, 0, element));
            
            sfxController.PlaySound("ui_select");
            
            // Apply values after finish
            lastEntitySelected = enemySelector.GetEntitiesSelected();
        }

        public int[] GetLastSelectedEntities()
        {
            return lastEntitySelected;
        }

        public void ResetLastEntitySelection()
        {
            lastEntitySelected = Array.Empty<int>();
            enemySelector.ResetSelection();
        }

        public bool IsInFrontRow(int enemyID)
        {
            return enemyID < MAX_ENEMIES_IN_ROW;
        }

        public void SetEnemyPosition(int enemyID, Transform demonObject, Demon demonStats,
            GameObject demonPlane)
        {
            enemies[enemyID].demonObject = demonObject;
            enemies[enemyID].demonStats = demonStats;
            enemies[enemyID].demonPlane = demonPlane;
        }

        public void ShowEnemyAffinity(Enemy enemy, Element affinity)
        {
            if (enemy.spriteLoader != null)
            {
                enemy.spriteLoader.ShowResistanceIcon(affinity);
            }
        }

        public void HideEnemyAffinity(Enemy enemy)
        {
            if (enemy.spriteLoader != null)
            {
                enemy.spriteLoader.HideResistanceIcon();
            }
        }

        public void ShowAllEnemyAffinities(Element affinity)
        {
            foreach (Enemy enemy in enemies)
            {
                ShowEnemyAffinity(enemy, affinity);
            }
        }

        public void HideAllEnemyAffinities()
        {
            foreach (Enemy enemy in enemies)
            {
                HideEnemyAffinity(enemy);
            }
        }

        public int GetMaxEnemiesInRow() => MAX_ENEMIES_IN_ROW;
    }
}