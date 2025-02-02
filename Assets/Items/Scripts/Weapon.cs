using Game.Battle;
using UnityEngine;

namespace Items.Scripts
{
    public abstract class Weapon : Item
    {
        public enum WeaponGender
        {
            Male, Female, Both
        }

        [Header("Weapon Settings")]
        public int attackDamage;
        public int hit;
        public BattleEnemySelection.SelectionType selectionType;
        public int minTargets, maxTargets;
        public WeaponGender weaponGender = WeaponGender.Both;
        
        
    }
}