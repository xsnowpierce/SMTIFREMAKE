using UnityEngine;

namespace Entity.Skills.Spells
{
    [CreateAssetMenu(fileName = "New Shield Skill", menuName = ("Skill/Spell/Shield"))]
    public class ShieldSpell : Spell
    {
        [Header("Shield")] 
        public bool instaKills;
        public bool physAttacks;
        public bool magicAttacks;
    }
}