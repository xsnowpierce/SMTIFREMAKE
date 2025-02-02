using UnityEngine;

namespace Entity.Skills.Spells
{
    [CreateAssetMenu(fileName = "New Status Skill", menuName = "Skill/Spell/Status")]
    public class StatusSpell : Spell
    {
        [Header("Status Settings")] 
        public Status.Status inflicts;
        [Range(0, 1f)] public float percentageHit;
    }
}