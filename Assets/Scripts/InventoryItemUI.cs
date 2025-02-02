using Data;
using Database;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pause.Inventory 
{
    public class InventoryItemUI : MonoBehaviour
    {
        private InventoryItem currentItem;

        [Header("UI Objects")] 
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemQuantity;

        public void LoadItem(InventoryItem item)
        {
            currentItem = item;
            itemName.text = FindObjectOfType<Databases>().GetTranslatedItem(item.item.itemKey);
            itemQuantity.text = item.amount.ToString();
        }

        public InventoryItem GetItem() => currentItem;

        public Image GetBackgroundImage() => backgroundImage;
    }
}
