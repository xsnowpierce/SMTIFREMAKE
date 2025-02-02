using System;
using System.Collections;
using System.Collections.Generic;
using Database;
using Entity;
using Entity.Skills;
using Entity.Skills.Physical;
using Entity.Skills.Spells;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Battle
{
    public class BattleSkillMenu : BattleMenuScript
    {
        [SerializeField] private RectTransform skillParent;
        [SerializeField] private GameObject skillObjectPrefab;
        [SerializeField] private float distanceBetweenSkills;
        [SerializeField] private float defaultHeightStart = -122f;

        private BattleSkillUIObject[] objects;
        private bool skillsLoaded = false;
        private Skill pickedSkill;
        private Databases database;
        private bool hasSkills = false;
        
        public void LoadSkillMenu(Entity.Entity entity, Databases database)
        {
            this.database = database;
            float currentHeight = defaultHeightStart;
            
            // Make a new skill list without the enemy only skills
            List<Skill> skills = new List<Skill>();
            
            // Add Melee and Gun skills to this
            if (entity is Human human)
            {
                if (human.equipment.meleeWeapon != null)
                {
                    PhysicalSkill meleeSkill = ScriptableObject.CreateInstance<PhysicalSkill>();
                    meleeSkill.element = Element.Physical;
                    meleeSkill.attackPower = human.equipment.meleeWeapon.attackDamage;
                    meleeSkill.inflictsStatus = human.equipment.meleeWeapon.inflictsStatus;
                    meleeSkill.statusInflicted = human.equipment.meleeWeapon.inflictStatus;
                    meleeSkill.targetType = human.equipment.meleeWeapon.selectionType;
                    meleeSkill.skillKey = "generic_melee";
                    meleeSkill.hideSkillCostOnSelection = true;
                    skills.Add(meleeSkill);
                }
                if (human.equipment.firearmWeapon != null && human.equipment.bullet != null)
                {
                    PhysicalSkill gunSkill = ScriptableObject.CreateInstance<PhysicalSkill>();
                    gunSkill.element = Element.Gun;
                    gunSkill.attackPower = human.equipment.firearmWeapon.attackDamage + human.equipment.bullet.attackDamage;
                    gunSkill.inflictsStatus = human.equipment.bullet.inflictsStatus;
                    gunSkill.statusInflicted = human.equipment.bullet.inflicts;
                    gunSkill.targetType = human.equipment.firearmWeapon.selectionType;
                    gunSkill.skillKey = "generic_gun";
                    gunSkill.hideSkillCostOnSelection = true;
                    skills.Add(gunSkill);
                }

                if (human.currentGuardian != null)
                {
                    foreach (Skill skill in human.currentGuardian.skills)
                    {
                        if(!skill.isEnemyOnly)
                            skills.Add(skill);
                    }
                }
            }else if (entity is Demon demon)
            {
                PhysicalSkill meleeSkill = ScriptableObject.CreateInstance<PhysicalSkill>();
                meleeSkill.element = Element.Physical;
                meleeSkill.attackPower = demon.GetAttackPower();
                meleeSkill.targetType = demon.meleeAttackType;
                meleeSkill.skillKey = "generic_melee";
                meleeSkill.hideSkillCostOnSelection = true;
                skills.Add(meleeSkill);
            }
            
            for (int i = 0; i < entity.skills.Length; i++)
            {
                if (entity.skills[i].isEnemyOnly)
                    continue;
                skills.Add(entity.skills[i]);
            }
            Skill[] usableSkills = skills.ToArray();

            if (usableSkills.Length == 0)
            {
                // User has no usable skills
                Debug.Log("User opened skills and has no skills.");
                hasSkills = false;
                isOpened = false;
                return;
            }
            else hasSkills = true;

            objects = new BattleSkillUIObject[usableSkills.Length];
            selectedImage = new Image[usableSkills.Length];
            BUTTON_TEXTS = new string[usableSkills.Length];
            
            // Create a object for each skill the entity has
            for (int i = 0; i < usableSkills.Length; i++)
            {
                GameObject newSkill = Instantiate(skillObjectPrefab);
                BattleSkillUIObject battleSkillUIObject = newSkill.GetComponent<BattleSkillUIObject>();
                RectTransform skillObjectTransform = newSkill.GetComponent<RectTransform>();
                selectedImage[i] = battleSkillUIObject.GetImageBackground();
                BUTTON_TEXTS[i] = battleSkillUIObject.GetSkillDescription();
                
                skillObjectTransform.SetParent(skillParent);
                skillObjectTransform.localScale = new Vector3(1, 1, 1);
                //skillObjectTransform.offsetMax = new Vector2(0, 100);
                //skillObjectTransform.offsetMin = new Vector2();
                
                skillObjectTransform.anchoredPosition = new Vector2(0, currentHeight);
                battleSkillUIObject.SetupSkill(usableSkills[i], selectionUI.GetDatabase(), entity);
                SetupButtonText(i, usableSkills[i]);

                float newDistance = skillObjectTransform.rect.height + distanceBetweenSkills;
                currentHeight -= newDistance;
                objects[i] = battleSkillUIObject;
            }
            // Done adding them
            skillsLoaded = true;
            SelectCurrent();
        }

        public Skill GetSkillAtPosition(int id)
        {
            return objects[id].GetSkill();
        }

        public int GetNumberOfObjects()
        {
            return objects.Length;
        }

        public int GetSelectedNum()
        {
            return currentSelected;
        }

        protected override void InitializeButtonTexts()
        {
        }

        private void SetupButtonText(int position, Skill skill)
        {
            string elementName = "";
            int power = 0;
            string attackType = "";
            switch (skill.targetType)
            {
                case BattleEnemySelection.SelectionType.Single:
                    attackType = "Single";
                    break;
                case BattleEnemySelection.SelectionType.AllRow:
                    attackType = "Entire Row";
                    break;
                case BattleEnemySelection.SelectionType.AllParty:
                    attackType = "All Foes";
                    break;

            }
            if (skill is Spell spell)
            {
                elementName = database.GetTranslatedSkill(spell.GetElementKey());
                if (spell is MagicSpell magicSpell)
                {
                    power = magicSpell.attackPower;
                }
            }
            else
            {
                if (skill is PhysicalSkill physicalSkill)
                {
                    power = physicalSkill.attackPower;
                    elementName = database.GetTranslatedSkill("element_physical");
                }
            }
            
            
            string result = elementName + "\t" + "<b><color=#FF0000>POW</color></b> " + power + "\t" + attackType + "\n" + "> " + database.GetTranslatedSkill("desc_" + skill.skillKey);
            
            BUTTON_TEXTS[position] = result;
        }

        public override void OnOpened()
        {
            pickedSkill = null;
        }

        public override void OnAccept()
        {
            pickedSkill = objects[currentSelected].GetSkill();
            PlaySound("ui_select");
            OnClose();
        }

        public Skill GetSelectedSkill() => pickedSkill;

        public override void OnClose()
        {
            if (hasSkills)
            {
                ResetMenu();
            }
            base.OnClose();
        }

        public override void OnCancel()
        {
            PlaySound("ui_cancel");
            ResetMenu();
        }

        private void ResetMenu()
        {
            foreach(BattleSkillUIObject obj in objects)
            {
                Destroy(obj.gameObject);
            }

            objects = Array.Empty<BattleSkillUIObject>();
            selectedImage = Array.Empty<Image>();
            skillsLoaded = false;
        }

        public void ResetSkill()
        {
            pickedSkill = null;
        }

        protected override void SelectCurrent()
        {
            if (!skillsLoaded) return;
            
            base.SelectCurrent();
        }
    }
}