using System;
using Database;
using Entity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Battle.UI
{
    public class BattleChangeSelectedUI : MonoBehaviour
    {
        private Databases database;
        [Header("Items")]
        [SerializeField] private TMP_Text entityName, entityRace, entityLevel;
        [SerializeField] private TMP_Text hpCurrent, hpMax;
        [SerializeField] private TMP_Text manaCurrent, manaMax;
        [SerializeField] private Image hpImage, manaImage;
        [Space(10)]
        [SerializeField] private TMP_Text strVal;
        [SerializeField] private TMP_Text intVal, magVal, vitVal, agiVal, luckVal;
        [SerializeField] private Image strImage, intImage, magImage, vitImage, agiImage, luckImage;
        [SerializeField] private Image phyRes, gunRes, fireRes, iceRes, eleRes, winRes, expRes, curRes;

        private readonly int MAX_STAT_AMOUNT = 40;

        public void SelectEntity(Entity.Entity entity, Databases database)
        {
            entityName.text = entity is not Protagonist ? database.GetTranslatedName(entity.entityName) : entity.entityName;
            entityRace.text = database.GetTranslatedRace(entity.race);
            entityLevel.text = "" + entity.level;
            hpCurrent.text = "" + entity.health + "/";
            hpMax.text = "" + entity.GetMaxHP();
            hpImage.fillAmount = ((entity.health * 1f) / entity.GetMaxHP());
            manaImage.fillAmount = ((entity.mana * 1f) / entity.GetMaxMana());
            manaCurrent.text = "" + entity.mana + "/";
            manaMax.text = "" + entity.GetMaxMana();

            // TODO weaknesses here
            phyRes.sprite = database.GetResistanceSprite(entity.Resistances.physical);
            gunRes.sprite = database.GetResistanceSprite(entity.Resistances.gun);
            fireRes.sprite = database.GetResistanceSprite(entity.Resistances.fire);
            iceRes.sprite = database.GetResistanceSprite(entity.Resistances.ice);
            eleRes.sprite = database.GetResistanceSprite(entity.Resistances.electricity);
            winRes.sprite = database.GetResistanceSprite(entity.Resistances.force);
            expRes.sprite = database.GetResistanceSprite(entity.Resistances.expel);
            curRes.sprite = database.GetResistanceSprite(entity.Resistances.curse);
            
            strVal.text = entity.stats.strength + "";
            intVal.text = entity.stats.intelligence + "";
            magVal.text = entity.stats.magic + "";
            vitVal.text = entity.stats.vitality + "";
            agiVal.text = entity.stats.agility + "";
            luckVal.text = entity.stats.luck + "";

            strImage.fillAmount = (entity.stats.strength * 1f) / MAX_STAT_AMOUNT;
            intImage.fillAmount = (entity.stats.intelligence * 1f) / MAX_STAT_AMOUNT;
            magImage.fillAmount = (entity.stats.magic * 1f) / MAX_STAT_AMOUNT;
            vitImage.fillAmount = (entity.stats.vitality * 1f) / MAX_STAT_AMOUNT;
            agiImage.fillAmount = (entity.stats.agility * 1f) / MAX_STAT_AMOUNT;
            luckImage.fillAmount = (entity.stats.luck * 1f) / MAX_STAT_AMOUNT;
        }
    }
}