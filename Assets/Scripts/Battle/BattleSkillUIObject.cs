using Database;
using Entity;
using Entity.Skills;
using Entity.Skills.Physical;
using Entity.Skills.Spells;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Battle
{
    public class BattleSkillUIObject : MonoBehaviour
    {
        [SerializeField] private Image skillBackground;
        [SerializeField] private Image skillIcon;
        [SerializeField] private TMP_Text skillName;
        [SerializeField] private GameObject skillCostParent;
        [SerializeField] private TMP_Text skillCost;
        [SerializeField] private TMP_Text skillCostType;
        [SerializeField] private Image skillCostBackground;

        [SerializeField] private Color UnselectedColour;
        [SerializeField] private Color SelectedColour;
        [SerializeField] private Color MpColour;
        [SerializeField] private Color HpColour;

        private Skill currentSkill; 
        private bool isSelected;
        private bool isSelectable;
        private string skillDescription;

        public void SetupMelee(bool isGun)
        {
            skillName.text = "Melee";
            skillCostParent.SetActive(false);
        }
        
        public void SetupSkill(Skill skill, Databases database, Entity.Entity entityStats)
        {
            currentSkill = skill;
            skillName.text = database.GetTranslatedSkill(skill.skillKey);
            
            // TODO set skill icon

            if (skill is Spell spell)
            {
                skillCost.text = spell.mpCost.ToString();
                skillCostType.text = "MP";
                skillCostType.color = MpColour;
                skillIcon.sprite = database.GetElementSprite(spell.element);
            }
            else if (skill is PhysicalSkill physicalSkill)
            {
                if (!physicalSkill.hideSkillCostOnSelection)
                {
                    float percentage = (physicalSkill.hpCostPercentage / 100f);
                    int skillCostAmount = Mathf.CeilToInt(entityStats.GetMaxHP() * percentage);
                    skillCost.text = skillCostAmount.ToString();
                    skillCostType.text = "HP";
                    skillCostType.color = HpColour;
                }
                else
                {
                    skillCost.text = "";
                    skillCostType.text = "";
                    skillCostType.color = new Color(0, 0, 0, 0);
                    skillCostBackground.enabled = false;
                }
                skillIcon.sprite = database.GetElementSprite(Element.Physical);
            }

            if (!CheckIfCanUse(skill, entityStats))
            {
                isSelectable = false;
                // TODO make the skill grey or something on the menu
            }
        }

        private bool CheckIfCanUse(Skill skill, Entity.Entity entityStats)
        {
            if (skill is Spell spell)
            {
                if (entityStats.mana < spell.mpCost)
                    return false;
                else return true;
            }

            if (skill is PhysicalSkill physicalSkill)
            {
                int skillCostAmount = entityStats.GetMaxHP() * (physicalSkill.hpCostPercentage / 100);
                if (entityStats.health <= skillCostAmount)
                    return false;
                else return true;
            }

            return false;
        }

        public void SelectSkill()
        {
            skillBackground.color = SelectedColour;
            isSelected = true;
        }

        public void DeselectSkill()
        {
            skillBackground.color = UnselectedColour;
            isSelected = false;
        }

        public string GetSkillDescription() => skillDescription;
        public Skill GetSkill() => currentSkill;
        public Image GetImageBackground() => skillBackground;
        public bool GetIsSelectable() => isSelectable;
    }
}