using UnityEngine;

namespace Entity.Skills.Spells
{
    [CreateAssetMenu(fileName = "New Other Skill", menuName = "Skill/Spell/Other")]
    public class OtherSpell : Spell
    {
        public int hpCost;
        [Space(10)]
        public int power;
    }
}