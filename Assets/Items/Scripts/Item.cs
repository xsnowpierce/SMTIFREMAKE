using System;
using UnityEngine;

namespace Items
{
    public abstract class Item : ScriptableObject
    {
        [Serializable]
        public struct StatBonus
        {
            public int strengthBonus;
            public int magicBonus;
            public int intelligenceBonus;
            public int vitalityBonus;
            public int agilityBonus;
            public int luckBonus;
        }
        [Header("Item Options")] 
        public string itemKey;
        public int buyPrice, sellPrice;
    }
}