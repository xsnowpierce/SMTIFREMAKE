using UnityEngine;

namespace Items.Scripts
{
    [CreateAssetMenu(fileName = "New Status Combat Item", menuName = "Item/Combat/Status")]
    public class StatusCombatItem : CombatItem
    {
        [Header("Status Settings")]
        public Status.Status statusToInflict;
        public int minEnemiesAffected;
        public int maxEnemiesAffected;
    }
}