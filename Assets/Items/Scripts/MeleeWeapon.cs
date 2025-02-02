using Entity;
using UnityEngine;

namespace Items.Scripts
{
    [CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Item/Combat/Melee")]
    public class MeleeWeapon : Weapon
    {
        public Element damageElement = Element.Physical;
        public bool isCursed;
        public StatBonus bonuses;

        [Header("Status Settings")] 
        public bool inflictsStatus;
        public Status.Status inflictStatus;
    }
}