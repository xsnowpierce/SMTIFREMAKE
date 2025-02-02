using Game.Battle;
using UnityEngine;

namespace Entity.Skills
{
    public abstract class Skill : ScriptableObject
    {
        [Header("Name")]
        public string skillKey;
        public bool isEnemyOnly;
        public BattleEnemySelection.SelectionType targetType;
        public bool hideSkillCostOnSelection;
        public Element element = Element.Physical;

        public string GetElementKey()
        {
            switch (element)
            {
                case Element.Physical:
                    return "element_physical";
                case Element.Gun:
                    return "element_gun";
                case Element.Fire:
                    return "element_fire";
                case Element.Ice:
                    return "element_ice";
                case Element.Electric:
                    return "element_electric";
                case Element.Force:
                    return "element_force";
                case Element.Nerve:
                    return "element_nerve";
                case Element.Expel:
                    return "element_expel";
                case Element.Curse:
                    return "element_curse";
                case Element.Almighty:
                    return "element_almighty";
                case Element.Recovery:
                    return "element_recovery";
                case Element.Support:
                    return "element_support";
                case Element.Other:
                    return "element_other";
                default:
                    return "idk lol";
            }
        }
    }
}