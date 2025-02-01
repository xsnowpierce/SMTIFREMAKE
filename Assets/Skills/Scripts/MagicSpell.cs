using UnityEngine;

namespace Entity.Skills.Spells
{
    [CreateAssetMenu(fileName = "New Magic Spell", menuName = "Skill/Spell/Magic")]
    public class MagicSpell : Spell
    {
        [Header("Spell Settings")] 
        public int attackPower;
        public Vector2 enemyHitRange;
        
        [Header("Status Infliction")] 
        public bool canInflict;
        public Status.Status statusInflicted;
    }
}