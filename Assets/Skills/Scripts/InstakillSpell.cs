using UnityEngine;

namespace Entity.Skills.Spells
{
    [CreateAssetMenu(fileName = "New Insta-kill Spell", menuName = "Skill/Spell/Insta-kill")]
    public class InstakillSpell : Spell
    {
        [Header("Insta-kill Stats")] 
        [Range(0, 1f)] public float killChance;
    }
}