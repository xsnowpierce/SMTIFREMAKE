using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "New Gem", menuName = "Item/Gem")]
    public class Gem : Item
    {
        [Header("Max Hold Bonus")] 
        public StatBonus statBonus;
    }
}