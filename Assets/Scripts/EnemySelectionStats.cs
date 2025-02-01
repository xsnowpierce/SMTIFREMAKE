using Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Battle.UI
{
    public class EnemySelectionStats : MonoBehaviour
    {
        private Databases database;

        [Header("UI Elements")] 
        [SerializeField] private GameObject activeObject;
        [SerializeField] private TMP_Text levelValue;
        [SerializeField] private TMP_Text raceName;
        [SerializeField] private TMP_Text demonName;
        [SerializeField] private TMP_Text hpValue;
        [SerializeField] private TMP_Text hpMaxValue;
        [SerializeField] private Image hpBarImage;
        [SerializeField] private Image physResistanceIcon;
        [SerializeField] private Image gunResistanceIcon;
        [SerializeField] private Image fireResistanceIcon;
        [SerializeField] private Image iceResistanceIcon;
        [SerializeField] private Image elecResistanceIcon;
        [SerializeField] private Image forceResistanceIcon;
        [SerializeField] private Image expelResistanceIcon;
        [SerializeField] private Image curseResistanceIcon;

        private Entity.Entity currentEntity;
        
        private void Awake()
        {
            database = FindObjectOfType<Databases>();
            SetVisible(false);
        }

        public void SetVisible(bool value)
        {
            activeObject.SetActive(value);
            if (!value) currentEntity = null;
        }
        
        public void LoadEntity(Entity.Entity entity)
        {
            currentEntity = entity;
            levelValue.text = entity.level.ToString();
            raceName.text = database.GetTranslatedRace(entity.race);
            demonName.text = database.GetTranslatedName(entity.entityName);
            hpValue.text = entity.health + "/";
            hpMaxValue.text = entity.GetMaxHP().ToString();
            hpBarImage.fillAmount = (entity.health * 1f / entity.GetMaxHP());
            
            physResistanceIcon.sprite = database.GetResistanceSprite(entity.Resistances.physical);
            gunResistanceIcon.sprite = database.GetResistanceSprite(entity.Resistances.gun);
            fireResistanceIcon.sprite = database.GetResistanceSprite(entity.Resistances.fire);
            iceResistanceIcon.sprite = database.GetResistanceSprite(entity.Resistances.ice);
            elecResistanceIcon.sprite = database.GetResistanceSprite(entity.Resistances.electricity);
            forceResistanceIcon.sprite = database.GetResistanceSprite(entity.Resistances.force);
            expelResistanceIcon.sprite = database.GetResistanceSprite(entity.Resistances.expel);
            curseResistanceIcon.sprite = database.GetResistanceSprite(entity.Resistances.curse);
        }
    }
}