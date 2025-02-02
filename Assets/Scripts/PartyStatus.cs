using System.Collections;
using Battle;
using Database;
using Entity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PartyStatus : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField] private RectTransform content;
        [SerializeField] private Image overlayImage;
        [SerializeField] private TMP_Text emptyText;
        [SerializeField] private TMP_Text entityName;
        [SerializeField] private TMP_Text entityRace;
        [SerializeField] private TMP_Text entityLevel;
        [SerializeField] private Image sprite;
        [SerializeField] private TMP_Text healthValue;
        [SerializeField] private TMP_Text manaValue;
        [SerializeField] private Image healthBar;
        [SerializeField] private Image healthBarSlow;
        [SerializeField] private Image manaBar;
        [SerializeField] private Image manaBarSlow;
        [SerializeField] private Image selectedImage;
        [SerializeField] private Image damageFlasher;
        [SerializeField] private float healthDecreaseSpeed = 0.4f;

        [SerializeField] private GameObject aliveStats;
        [SerializeField] private GameObject deadStats;
        
        private Entity.Entity currentEntity;
        private Animator animator;
        private bool isEmpty;
        private bool isDead;
        private BattleController battleController;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
        
        public void SetupEmpty()
        {
            content.gameObject.SetActive(false);
            isEmpty = true;
            emptyText.enabled = true;
            overlayImage.enabled = true;
            currentEntity = null;
        }
        
        public void SetupMember(Entity.Entity entity, Databases databases)
        {
            currentEntity = entity;
            content.gameObject.SetActive(true);
            overlayImage.enabled = false;
            emptyText.enabled = false;
            entityName.text = entity is not Protagonist ? databases.GetTranslatedName(entity.entityName) : entity.entityName;
            healthValue.text = entity.health.ToString();
            manaValue.text = entity.mana.ToString();
            entityRace.text = databases.GetTranslatedRace(entity.race);
            entityLevel.text = "Lv " + entity.level;
            healthBar.fillAmount = (entity.health * 1f / entity.GetMaxHP());
            healthBarSlow.fillAmount = (entity.health * 1f / entity.GetMaxHP());
            manaBar.fillAmount = (entity.mana * 1f / entity.GetMaxMana());
            manaBarSlow.fillAmount = (entity.mana * 1f / entity.GetMaxMana());
            deadStats.SetActive(false);
            SpriteUtils.SpriteUtils.ApplySpriteToImage(sprite, entity.sprites[0]);
        }

        public void SetDeadState(bool value)
        {
            isDead = value;
            if (value)
            {
                Deselected();
                // Hide hp and mana stuff and show "DEAD"
                aliveStats.SetActive(false);
                deadStats.SetActive(true);
            }
            else
            {
                // Show hp and mana stuff and hide "DEAD"
                aliveStats.SetActive(true);
                deadStats.SetActive(false);
            }
        }

        public void Selected(bool hasOverlay)
        {
            if (isEmpty || isDead) return;
            selectedImage.enabled = hasOverlay;
            overlayImage.enabled = false;
        }

        public void Deselected()
        {
            if (isEmpty) return;
            selectedImage.enabled = false;
            overlayImage.enabled = true;
        }

        public void PlayDamagedAnimation(Color damageColor, float flashSeconds, bool shakeEffect)
        {
            float newFillAmount = (currentEntity.health * 1f / currentEntity.GetMaxHP());
            UpdateValues();
            if (shakeEffect)
            {
                animator.Play("PartyDamage_01");
                damageFlasher.color = damageColor;
                damageFlasher.enabled = true;
                Invoke(nameof(HideDamageFlasher), flashSeconds);
            }
            StartCoroutine(decreaseHealthBar(newFillAmount));
        }

        public void PlayManaDepletion()
        {
            StartCoroutine(decreaseManaBar(currentEntity.mana * 1f / currentEntity.GetMaxMana()));
        }

        public void UpdateValues()
        {
            float newHealthFillAmount = (currentEntity.health * 1f / currentEntity.GetMaxHP());
            float newManaFillAmount = (currentEntity.mana * 1f / currentEntity.GetMaxMana());
            healthBar.fillAmount = newHealthFillAmount;
            manaBar.fillAmount = newManaFillAmount;
            healthValue.text = currentEntity.health.ToString();
            manaValue.text = currentEntity.mana.ToString();
        }

        private IEnumerator decreaseHealthBar(float targetAmount)
        {
            float amountToLose = healthBarSlow.fillAmount - targetAmount;

            float speed = amountToLose / healthDecreaseSpeed;

            while (healthBarSlow.fillAmount > targetAmount)
            {
                healthBarSlow.fillAmount -= speed * Time.deltaTime;
                yield return null;
            }
            healthBarSlow.fillAmount = targetAmount;
        }
        
        private IEnumerator decreaseManaBar(float targetAmount)
        {
            float amountToLose = manaBarSlow.fillAmount - targetAmount;

            float speed = amountToLose / healthDecreaseSpeed;

            while (manaBarSlow.fillAmount > targetAmount)
            {
                manaBarSlow.fillAmount -= speed * Time.deltaTime;
                yield return null;
            }
            manaBarSlow.fillAmount = targetAmount;
        }

        private void HideDamageFlasher()
        {
            damageFlasher.enabled = false;
        }

        public Entity.Entity GetCurrentEntity() => currentEntity;
    }
}