using Entity;
using UnityEngine;

namespace Items.Scripts
{
    [CreateAssetMenu(fileName = "New Damage Combat Item", menuName = "Item/Combat/Damaging Item")]
    public class DamageCombatItem : CombatItem
    {
        [Header("Damage Settings")]
        public Element damageElement;
        public bool damageAll;
        public bool damageFixedAmount;
        public float damageAmount;

        [Header("Status Settings")] 
        public bool chanceToInflict;
        public Status.Status statusEffect;
    }
}