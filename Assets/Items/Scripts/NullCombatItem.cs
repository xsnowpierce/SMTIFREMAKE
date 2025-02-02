using Entity;
using UnityEngine;

namespace Items.Scripts
{
    [CreateAssetMenu(fileName = "New Null Combat Item", menuName = "Item/Combat/Null")]
    public class NullCombatItem : CombatItem
    {
        [Header("Null Settings")]
        public Element[] elementsNulled;
        public bool blocksEnergyDrain;
    }
}