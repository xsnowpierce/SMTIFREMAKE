using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Database
{
    [CreateAssetMenu(fileName = "New Item Database", menuName = "Database/Item")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<Item> itemList;

        public List<Item> GetItemDatabase() => itemList;

        public Item GetItem(int index)
        {
            return itemList[index];
        }
    }
}