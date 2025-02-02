using UnityEngine;

namespace Entity.Skills.Spells
{
    [CreateAssetMenu(fileName = "New Buff Spell", menuName = "Skill/Spell/Stat Buff")]
    public class BuffSpell : Spell
    {
        public bool allyBuff;
        
        [Header("Modify Value")] 
        public int attack;
        public int defense;
        public int hitEvade;
        public int magicPower;
        public int buffs;
    }
}