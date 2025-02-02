using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class PartyUIController : MonoBehaviour
    {
        [Header("UI Settings")] 
        [SerializeField] private Image[] partySprites;
        [SerializeField] private TMP_Text[] partyNames;
        [SerializeField] private TMP_Text[] partyEmpty;
        [SerializeField] private GameObject[] contentObjects;
        [Space(10)]
        [SerializeField] private Image[] healthBar;
        [SerializeField] private Image[] delayedHealthBar;
        [SerializeField] private TMP_Text[] healthText;
        [Space(5)]
        [SerializeField] private Image[] manaBar;
        [SerializeField] private Image[] delayedManaBar;
        [SerializeField] private TMP_Text[] manaText;

        [Header("Party Selected Settings")] 
        [SerializeField] private Image[] backgrounds;
        [SerializeField] private Image[] selectionLayer;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color deSelectedColor;
        private int currentSelectedMember = -1;

        public void LoadParty(SaveData saveData)
        {
            for(int i = 0; i < 6; i++)
            {
                Entity.Entity entity = saveData.entityStock[saveData.partySlots[i]];
                
                if (entity == null)
                {
                    // Set up "EMPTY"
                    contentObjects[i].SetActive(false);
                    partyEmpty[i].enabled = true;
                    continue;
                }
                
                contentObjects[i].SetActive(true);
                partyEmpty[i].enabled = false;

                float healthPercent = (float) entity.health / entity.GetMaxHP();
                float manaPercent = (float) entity.mana / entity.GetMaxMana();
                
                Sprite sprite = entity.sprites[0];
                SpriteUtils.SpriteUtils.ApplySpriteToImage(partySprites[i], sprite);
                
                partyNames[i].text = entity.entityName;
                healthBar[i].fillAmount = healthPercent;
                delayedHealthBar[i].fillAmount = healthPercent;
                manaBar[i].fillAmount = manaPercent;
                delayedManaBar[i].fillAmount = manaPercent;
                healthText[i].text = "" + entity.health;
                manaText[i].text = "" + entity.mana;
            }
        }

        public void SelectPartyMember(int rowID)
        {
            DeselectMembers();
            currentSelectedMember = rowID;
            backgrounds[rowID].color = selectedColor;
            selectionLayer[rowID].enabled = false;
        }

        public void DeselectMembers()
        {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].color = deSelectedColor;
                selectionLayer[i].enabled = true;
            }
        }
    }
}