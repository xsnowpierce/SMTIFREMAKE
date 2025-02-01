using System;
using Data;
using Database;
using Items;
using Items.Scripts;
using TMPro;
using UnityEngine;

namespace Game.Pause.Inventory
{
    public class InventoryUI : MonoBehaviour, PauseMenuPanel
    {
        public enum ItemType
        {
            Expendable, Gem, Valuable
        }
        
        [SerializeField] private RectTransform itemParent;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Color selectedColour;
        [SerializeField] private Color normalColour;
        [SerializeField] private TMP_Text itemDescription;
        [SerializeField] private TMP_Text noItemText;
        private SaveData data;
        private InventoryItemUI[] items;
        private int itemSelected;

        public void OnOpened()
        {
            ReadItemList(ItemType.Expendable);
            itemDescription.text = "";
            if (items.Length == 0 || items == null)
            {
                noItemText.enabled = true;
            }
            else
            {
                noItemText.enabled = false;
                if (itemSelected > items.Length)
                   itemSelected = items.Length;
                SelectItem(itemSelected); 
            }
        }

        public void OnMovement(Vector2 movement)
        {
            if (items.Length == 0 || items == null)
                return;
            int y = (int) movement.y;
            int newSelect;
            if (itemSelected + y > items.Length - 1)
            {
                // Clamp to top
                newSelect = 0;
            }
            else
            {
                // Select normally
                newSelect = y + itemSelected;
            }

            if (newSelect < 0)
                newSelect = items.Length - 1;
            
            // Process selection
            SelectItem(newSelect);
        }

        public bool OnCancel()
        {
            // RETURN FALSE IF WE ARE EXITING MENU
            // RETURN TRUE IF WE ARE STILL IN MENU
            OnClosed();
            return false;
        }

        public void OnSubmit()
        {
            
        }

        public void OnClosed()
        {
            DestroyAllItems();
            items = Array.Empty<InventoryItemUI>();
        }

        public void OnTabChange(int direction)
        {
            
        }

        private void SelectItem(int listNumber)
        {
            if (listNumber < 0)
                listNumber = 0;
            if (listNumber > items.Length - 1)
                listNumber = items.Length - 1;
            
            items[itemSelected].GetBackgroundImage().color = normalColour;
            items[listNumber].GetBackgroundImage().color = selectedColour;
            itemSelected = listNumber;
            //itemDescription.text = FindObjectOfType<Databases>().GetTranslatedItemDescription(items[itemSelected].GetItem().item.itemKey);
            itemDescription.text = items[itemSelected].GetItem().item.itemKey + "_desc";
        }
        private void ReadItemList(ItemType type)
        {
            data = FindObjectOfType<SaveData>();
            items = new InventoryItemUI[data.inventory.Count];

            for (int i = 0; i < data.inventory.Count; i++)
            {
                InventoryItem item = data.inventory[i];
                
                // DO ITEM TYPECHECK
                if (type == ItemType.Expendable)
                {
                    if (item.item.GetType() == typeof(Bullet) 
                        || item.item.GetType() == typeof(FirearmWeapon) 
                        || item.item.GetType() == typeof(MeleeWeapon)
                        || item.item.GetType() == typeof(Gem))
                        continue;
                }
                else if (type == ItemType.Gem)
                {
                    if (item.item.GetType() != typeof(Gem))
                        continue;
                }
                else
                {
                    // ADD KEY ITEM SUPPORT HERE
                }

                GameObject newItem = Instantiate(itemPrefab, itemParent);
                InventoryItemUI inventoryItemUI = newItem.GetComponent<InventoryItemUI>();
                
                inventoryItemUI.LoadItem(item);
                items[i] = inventoryItemUI;
            }
        }
        private void DestroyAllItems()
        {
            foreach (RectTransform child in itemParent)
            {
                Destroy(child.gameObject);
            }
        }
        public InventoryItem GetItemAt(int number)
        {
            return items[number].GetItem();
        }
    }
}