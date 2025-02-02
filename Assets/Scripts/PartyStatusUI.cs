using Data;
using Database;
using Game.Battle.UI;
using Game.UI;
using UnityEngine;

namespace Game.Battle
{
    public class PartyStatusUI : MonoBehaviour
    {
        private SaveData data;
        [SerializeField] private GameObject disableObject;
        [SerializeField] private float damageFlashSeconds;
        [SerializeField] private PartyStatus[] statuses;
        private PartyMoveBar[] moveBars;
        private Databases database;
        [SerializeField] [Range(0f, 1f)] private float whiteDamageRange;
        [SerializeField] [Range(0f, 1f)] private float yellowDamageRange;
        [SerializeField] [Range(0f, 1f)] private float redDamageRange;
        [SerializeField] private Color whiteHealthColour;
        [SerializeField] private Color yellowHealthColour;
        [SerializeField] private Color redHealthColour;

        private void Awake()
        {
            database = FindObjectOfType<Databases>();
            data = FindObjectOfType<SaveData>();
        }

        private void Start()
        {
            SetupPartyUI();
        }

        private void SetupPartyUI()
        {
            moveBars = new PartyMoveBar[statuses.Length];
            for (int i = 0; i < data.partySlots.Length; i++)
            {
                if (data.partySlots[i] < 0)
                    statuses[i].SetupEmpty();
                else
                {
                    if (data.entityStock[data.partySlots[i]] != null)
                    {
                        Entity.Entity entity = data.entityStock[data.partySlots[i]];
                        if (entity.health > entity.GetMaxHP())
                            entity.health = entity.GetMaxHP();
                        if (entity.mana > entity.GetMaxMana())
                            entity.mana = entity.GetMaxMana();
                        statuses[i].SetupMember(entity, database);
                        moveBars[i] = statuses[i].GetComponent<PartyMoveBar>();
                    }
                    else
                    {
                        statuses[i].SetupEmpty();
                    }
                }
            }
        }

        public void SetPartyUIVisible(bool value)
        {
            disableObject.SetActive(value);
        }

        public void SelectPartyMember(int memberID)
        {
            DeselectAllMembers();
            statuses[memberID].Selected(true);
        }

        public void UnHideAllPartyMembers()
        {
            foreach (PartyStatus status in statuses)
            {
                status.Selected(false);
            }
        }

        public void DeselectAllMembers()
        {
            foreach (PartyStatus status in statuses)
            {
                status.Deselected();
            }
        }

        public void UpdateEntityInSlot(int memberID, Entity.Entity newEntity)
        {
            if (newEntity == null)
            {
                statuses[memberID].SetupEmpty();
            }else
                statuses[memberID].SetupMember(newEntity, database);
        }

        public void SetDead(int memberID, bool isDead)
        {
            statuses[memberID].SetDeadState(isDead);
        }
        
        public void DealDamage(int memberID, Entity.Entity entity, bool shakeEffect)
        {
            // Determine colour to damage with
            float hpPercent = entity.health * 1f / entity.GetMaxHP();
            Color damageColour;
            if (hpPercent < redDamageRange)
                damageColour = redHealthColour;
            else if (hpPercent < yellowDamageRange)
                damageColour = yellowHealthColour;
            else damageColour = whiteHealthColour;

            // Shake spot
            statuses[memberID].PlayDamagedAnimation(damageColour, damageFlashSeconds, shakeEffect);
            // Deal damage and do animation for it
        }

        public void LoseMana(int memberID)
        {
            statuses[memberID].PlayManaDepletion();
        }

        public void UpdateStatPanel(int memberID)
        {
            statuses[memberID].UpdateValues();
        }

        public void SetPartyMemberMove(int memberID, Sprite icon, string name)
        {
            moveBars[memberID].ShowMoveAction(icon, name);
        }

        public void HideMemberMove(int memberID)
        {
            if(moveBars[memberID] != null)
                moveBars[memberID].HideMoveAction();
        }
        
        public void HideAllMemberMoves()
        {
            for (int i = 0; i < statuses.Length; i++)
            {
                if(moveBars[i] != null)
                    moveBars[i].HideMoveAction();
            }
        }
    }
}