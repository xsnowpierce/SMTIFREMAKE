using Database;
using Entity;
using Items;
using TMPro;
using UnityEngine;

namespace Game.Battle.UI
{
    public class BattleLevelupEquipment : MonoBehaviour
    {
        [Header("Names")]
        [SerializeField] private TMP_Text swordName;
        [SerializeField] private TMP_Text gunName;
        [SerializeField] private TMP_Text helmetName;
        [SerializeField] private TMP_Text chestName;
        [SerializeField] private TMP_Text gloveName;
        [SerializeField] private TMP_Text shoeName;
        
        [Header("Weapons")]
        [SerializeField] private TMP_Text swordDamageValue;
        [SerializeField] private TMP_Text swordAccValue;
        
        [SerializeField] private TMP_Text gunDamageValue;
        [SerializeField] private TMP_Text gunAccValue;
        
        [Header("Stat Boosts")]
        [SerializeField] private TMP_Text helmetStatBoost;
        [SerializeField] private TMP_Text chestStatBoost;
        [SerializeField] private TMP_Text gloveStatBoost;
        [SerializeField] private TMP_Text shoeStatBoost;
        
        [Header("Defense Boosts")]
        [SerializeField] private TMP_Text helmetDefenseBoost;
        [SerializeField] private TMP_Text chestDefenseBoost;
        [SerializeField] private TMP_Text gloveDefenseBoost;
        [SerializeField] private TMP_Text shoeDefenseBoost;

        public void LoadInformation(Human human, Databases database)
        {
            if (human.equipment.meleeWeapon != null)
            {
                swordName.text = database.GetTranslatedItem(human.equipment.meleeWeapon.itemKey);
                swordDamageValue.text = "<color=#76d3f5>" + human.equipment.meleeWeapon.attackDamage;
                swordAccValue.text = "<color=#76d3f5>" + human.equipment.meleeWeapon.hit;
            }
            else
            {
                swordName.text = database.GetTranslatedText("inventory_no_equip");
                swordDamageValue.text = "--";
                swordAccValue.text = "--";
            }

            if (human.equipment.firearmWeapon != null)
            {
                gunName.text = database.GetTranslatedItem(human.equipment.firearmWeapon.itemKey);
                gunDamageValue.text = "<color=#76d3f5>" + human.equipment.firearmWeapon.attackDamage;
                gunAccValue.text = "<color=#76d3f5>" + human.equipment.firearmWeapon.hit;
            }
            else
            {
                gunName.text = database.GetTranslatedText("inventory_no_equip");
                gunDamageValue.text = "--";
                gunAccValue.text = "--";
            }

            if (human.equipment.helmet != null)
            {
                helmetName.text = database.GetTranslatedItem(human.equipment.helmet.itemKey);
                helmetStatBoost.text = GetParsedBonuses(human.equipment.helmet);
                helmetDefenseBoost.text = "+<color=#76d3f5>" + human.equipment.helmet.defense;
            }
            else
            {
                helmetName.text = database.GetTranslatedText("inventory_no_equip");
                helmetStatBoost.text = "--";
                helmetDefenseBoost.text = "--";
            }

            if (human.equipment.body != null)
            {
                chestName.text = database.GetTranslatedItem(human.equipment.body.itemKey);
                chestStatBoost.text = GetParsedBonuses(human.equipment.helmet);
                chestDefenseBoost.text = "+<color=#76d3f5>" + human.equipment.body.defense;
            }
            else
            {
                chestName.text = database.GetTranslatedText("inventory_no_equip");
                chestStatBoost.text = "--";
                chestDefenseBoost.text = "--";
            }

            if (human.equipment.arms != null)
            {
                gloveName.text = database.GetTranslatedItem(human.equipment.arms.itemKey);
                gloveStatBoost.text = GetParsedBonuses(human.equipment.helmet);
                gloveDefenseBoost.text = "+<color=#76d3f5>" + human.equipment.arms.defense;
            }
            else
            {
                gloveName.text = database.GetTranslatedText("inventory_no_equip");
                gloveStatBoost.text = "--";
                gloveDefenseBoost.text = "--";
            }

            if (human.equipment.legs != null)
            {
                shoeName.text = database.GetTranslatedItem(human.equipment.legs.itemKey);
                shoeStatBoost.text = GetParsedBonuses(human.equipment.helmet);
                shoeDefenseBoost.text = "+<color=#76d3f5>" + human.equipment.legs.defense;
            }
            else
            {
                shoeName.text = database.GetTranslatedText("inventory_no_equip");
                shoeStatBoost.text = "--";
                shoeDefenseBoost.text = "--";
            }
        }

        private string GetParsedBonuses(Armor armor)
        {
            string returnString = "";
            if (armor.bonuses.strengthBonus > 0)
                returnString += "STR: +<color=#76d3f5>" + armor.bonuses.strengthBonus + "</color>, ";
            if (armor.bonuses.intelligenceBonus > 0)
                returnString += "INT: +<color=#76d3f5>" + armor.bonuses.intelligenceBonus + "</color>, ";
            if (armor.bonuses.magicBonus > 0)
                returnString += "MAG: +<color=#76d3f5>" + armor.bonuses.magicBonus + "</color>, ";
            if (armor.bonuses.vitalityBonus > 0)
                returnString += "VIT: +<color=#76d3f5>" + armor.bonuses.vitalityBonus + "</color>, ";
            if (armor.bonuses.agilityBonus > 0)
                returnString += "AGL: +<color=#76d3f5>" + armor.bonuses.agilityBonus + "</color>, ";
            if (armor.bonuses.luckBonus > 0)
                returnString += "LCK: +<color=#76d3f5>" + armor.bonuses.luckBonus + "</color>";

            return returnString;
        }
    }
}