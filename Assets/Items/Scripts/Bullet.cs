using UnityEngine;

namespace Items.Scripts
{
    [CreateAssetMenu(fileName = "New Bullet Type", menuName = "Item/Combat/Bullet")]
    public class Bullet : Item
    {
        [Header("Bullet Settings")] 
        public int attackDamage;

        [Space(10)]
        public bool inflictsStatus;
        public Status.Status inflicts;
    }
}