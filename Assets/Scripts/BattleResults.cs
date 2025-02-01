using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battle;
using Data;
using Database;
using Entity;
using Game.Audio;
using Game.Input;
using Game.UI;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Battle.UI
{
    public class BattleResults : MonoBehaviour
    {
        [SerializeField] private float xpIncreaseSpeed = 1f;
        
        [SerializeField] private GameObject disableObject;
        [SerializeField] private Animator sideBarAnimator;
        [SerializeField] private Animator fadeScreenAnimator;
        [SerializeField] private PlayerInputWrapper input;
        [SerializeField] private TMP_Text maccaGainText;
        [SerializeField] private TMP_Text magGainText;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Transform itemParent;
        [SerializeField] private AudioSource experienceSource;
        [SerializeField] private BattleLevelUp battleLevelUp;

        [Header("Party Stock")] 
        [SerializeField] private GameObject partyStatusPrefab;
        [SerializeField] private Transform partyStatusParent;
        [SerializeField] private List<XPPartyStatus> activeObjects;

        private SFXController sfxController;
        private BattleController battleController;

        private void Awake()
        {
            sfxController = FindObjectOfType<SFXController>();
        }
        
        public IEnumerator EndResults(SaveData data, List<Entity.Entity> defeatedEntities, Databases database, float sfxVolume, BattleController battleController)
        {
            this.battleController = battleController;
            int maccaGained = BattleFormuals.GetGainedMacca(defeatedEntities);
            int magGained = BattleFormuals.GetGainedMagnetite(defeatedEntities);
            List<Item> itemsDropped = new List<Item>();
            
            int[] partyXPGain = BattleFormuals.GetPartyXPGain(defeatedEntities, data);
            int[] stockXPGain = BattleFormuals.GetStockXPGain(defeatedEntities, data);

            maccaGainText.text = "+<color=#76d3f5>" + maccaGained;
            magGainText.text = "+<color=#76d3f5>" + magGained;

            experienceSource.volume = sfxVolume;
            
            fadeScreenAnimator.Play("ResultsFadeIn");

            foreach (Entity.Entity entity in defeatedEntities)
            {
                if (entity is Demon demon)
                {
                    foreach (Item dropped in demon.droppedItems)
                    {
                        float randomCheck = Random.Range(0f, 1f);
                        if (BattleFormuals.ITEM_DROP_RATE > randomCheck)
                        {
                            itemsDropped.Add(dropped);
                        }
                    } 
                }
            }

            foreach (Item item in itemsDropped)
            {
                GameObject newItem = Instantiate(itemPrefab, itemParent);
                BattleResultsItem resultsItem = newItem.GetComponent<BattleResultsItem>();
                resultsItem.SetItemInfo(item, database);
            }
            
            List<int> partyList = new List<int>();
            
            // Set up party stats and xp bars
            for (int i = 0; i < data.partySlots.Length; i++)
            {
                Entity.Entity entity = data.GetPartyMembers()[i];
                if (entity == null)
                {
                    activeObjects[i].SetupEmpty();
                    partyList.Add(-1);
                }
                else
                {
                    activeObjects[i].SetupMember(entity, database);
                    partyList.Add(data.partySlots[i]);
                }
            }
            
            for (int i = 0; i < data.entityStock.Length; i++)
            {
                bool inParty = false;
                foreach (int t in data.partySlots)
                {
                    if (i != t) continue;
                    
                    // Is in party
                    inParty = true;
                    break;
                }

                if (inParty) continue;
                
                Entity.Entity entity = data.entityStock[i];
                if(entity is Human) continue;
                GameObject newObject = Instantiate(partyStatusPrefab, partyStatusParent, true);
                XPPartyStatus partyStatus = newObject.GetComponent<XPPartyStatus>();
                activeObjects.Add(partyStatus);
                if (entity == null)
                    partyStatus.SetupEmpty();
                else partyStatus.SetupMember(entity, database);
                partyStatus.transform.localScale = new Vector3(0, 0, 0);
            }
            
            disableObject.SetActive(true);

            // TODO have appearing animation
            sideBarAnimator.Play("ResultsOpen");
            yield return new WaitForSeconds(0.25f);
            foreach (XPPartyStatus partyStatus in activeObjects)
            {
                partyStatus.PlayOpenAnimation();
            }

            yield return new WaitForSeconds(0.25f);
            
            // TODO have leveling animation

            for (int i = 0; i < data.partySlots.Length; i++)
            {
                if(partyList[i] == -1) continue;
                
                activeObjects[i].IncreaseXP(partyXPGain[i], xpIncreaseSpeed, this);
            }

            int added = 0;
            for (int i = 0; i < data.entityStock.Length; i++)
            {
                if(stockXPGain[i] == -1) continue;
                activeObjects[added + 6].IncreaseXP(stockXPGain[i], xpIncreaseSpeed, this);
                added++;
            }

            while (true)
            {
                bool isGainingXP = false;
                foreach (XPPartyStatus partyStatus in activeObjects)
                {
                    if (!partyStatus.GetIsIncreasingXP()) continue;
                    
                    isGainingXP = true;
                    break;
                }

                if (isGainingXP)
                {
                    // Keep looping sfx
                    experienceSource.loop = true;
                    if(!experienceSource.isPlaying) experienceSource.Play();
                    yield return null;
                }
                else
                {
                    // Break out of loop
                    experienceSource.loop = false;
                    break;
                }
            }

            bool acceptPressed = false;
            // TODO wait for player to press A
            while (!acceptPressed)
            {
                if (input.GetInteract().ReadValue<float>() != 0)
                {
                    acceptPressed = true;
                }
                yield return null;
            }
            
            // TODO level up(s)
            List<Entity.Entity> levelUpList = new List<Entity.Entity>();
            foreach (XPPartyStatus partyStatus in activeObjects)
            {
                if (partyStatus.GetHasLevelUp())
                    levelUpList.Add(partyStatus.GetEntity());
            }

            if (levelUpList.Count > 0)
            {
                battleController.ChangeBattlePhase(BattlePhase.LevelUp);
                
                // Get rid of current menu
                foreach (XPPartyStatus partyStatus in activeObjects)
                {
                    partyStatus.PlayCloseAnimation();
                }
                yield return new WaitForSeconds(0.25f);
                sideBarAnimator.Play("ResultsClose");
                yield return new WaitForSeconds(0.5f);

                foreach (Entity.Entity entity in levelUpList)
                {
                    yield return StartCoroutine(battleLevelUp.LevelUp(entity, database, input));
                }
            }

            // TODO fade out back to exploration scene
        }

        public void PlayLevelupSound()
        {
            sfxController.PlaySound("levelup_ping");
        }
    }
}