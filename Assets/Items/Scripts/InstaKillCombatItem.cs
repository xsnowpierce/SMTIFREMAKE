using Entity;
using UnityEngine;

namespace Items.Scripts
{
    [CreateAssetMenu(fileName = "New Insta-Kill Combat Item", menuName = "Item/Combat/Insta-Kill")]
    public class InstaKillCombatItem : CombatItem
    {
        [Header("Insta-Kill Settings")] 
        public bool instaKills;
        public Element instaKillElement;
        public int minInstaKill;
        public int maxInstaKill;
    }
}