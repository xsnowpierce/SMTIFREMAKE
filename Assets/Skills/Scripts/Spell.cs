using UnityEngine;

namespace Entity.Skills.Spells
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "Skill/Spell")]
    public abstract class Spell : Skill
    {
        [Header("Spell Options")]
        public int mpCost;
    }
}