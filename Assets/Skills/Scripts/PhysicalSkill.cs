using Entity;
using Entity.Skills;
using Game.Battle;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entity.Skills.Physical
{
    [CreateAssetMenu(fileName = "New Physical Skill", menuName = "Skill/Physical Skill")]
    public class PhysicalSkill : Skill
    {
        [Header("Physical Skill Settings")]
        public int hpCostPercentage;
        public int attackPower;
        public bool hasInstaKillChance;
        public Element element;

        [Header("Status Infliction")] 
        public bool inflictsStatus;
        public Status.Status statusInflicted;
    }
}