using UnityEngine;

namespace Entity.Skills.Spells
{
    [CreateAssetMenu(fileName = "New Healing Spell", menuName = "Skill/Spell/Healing")]
    public class HealingSpell : Spell
    {
        [Header("Fixed Amount")] 
        public bool healsFixedAmount;
        public int fixedHealAmount;

        [Header("Percentage Amount")] 
        public bool healsPercentage;
        public int percentageHealed;

        [Header("Reviving Settings")] 
        public bool revives;
    }
}