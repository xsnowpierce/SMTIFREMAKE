using UnityEngine;

namespace Items.Scripts
{
    [CreateAssetMenu(fileName = "New Status Boost Combat Item", menuName = "Item/Combat/Status Boost")]
    public class StatusBoostCombatItem : CombatItem
    {
        [Header("Battle Status Settings")] 
        public bool affectsAll;
        public bool raisesAttack;
        public bool raisesDefense;
        public bool raisesMagic;
        public bool raisesHitRate;
    }
}