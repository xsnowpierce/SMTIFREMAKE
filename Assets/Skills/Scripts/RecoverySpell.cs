using UnityEngine;

namespace Entity.Skills.Spells
{
    [CreateAssetMenu(fileName = "New Recovery Spell", menuName = "Skill/Spell/Recovery")]
    public class RecoverySpell : Spell
    {
        [Header("Status Settings")]
        public bool healAllStatuses;
        public Status.Status cureStatus;
    }
}