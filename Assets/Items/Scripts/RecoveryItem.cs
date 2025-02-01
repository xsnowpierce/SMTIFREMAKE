using UnityEngine;

namespace Items.Scripts
{
    [CreateAssetMenu(fileName = "New Recovery Item", menuName = "Item/Recovery")]
    public class RecoveryItem : Item
    {
        [Header("HP Options")] 
        public bool givesHealth;
        public bool givesFixedHealthAmount;
        public float healAmount;

        [Header("MP Options")] 
        public bool givesMana;
        public bool givesFixedManaAmount;
        public float manaAmount;

        [Header("Status Options")] 
        public bool curesStatus;
        public bool cureAll;
        public Status.Status[] statuses;
    }
}