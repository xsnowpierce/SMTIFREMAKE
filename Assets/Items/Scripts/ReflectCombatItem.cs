using UnityEngine;

namespace Items.Scripts
{
    [CreateAssetMenu(fileName = "New Reflect Combat Item", menuName = "Item/Combat/Reflect")]
    public class ReflectCombatItem : CombatItem
    {
        [Header("Reflect Settings")]
        public bool reflectsMagic;
        public bool reflectsPhysical;
        public int turnDuration;
    }
}