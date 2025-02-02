using Data;
using Entity;
using UnityEngine;

namespace Items
{
    
    [CreateAssetMenu(fileName = "New Armor", menuName = "Item/Combat/Armor")]
    public class Armor : Item
    {
        public enum ArmorType
        {
            Helmet,
            Body,
            Arms,
            Leg
        }

        [Header("Armor Settings")]
        public ArmorType armorType;
        public int defense;
        public int evasion;
        public PlayerGender gender = PlayerGender.Both;
        public StatBonus bonuses;
        public ResistanceType resists;
    }
}