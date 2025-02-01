using Database;
using Entity;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SpriteUtils
{
    public class BattleSpriteLoader : MonoBehaviour
    {
        [SerializeField] private Transform spritePlane;
        [SerializeField] private bool animateSprite;
        
        [Header("Colours")]
        [SerializeField] private Color hitEffectColour;

        [Header("Resistance Icon")] 
        [SerializeField] private Transform resistanceObject;
        [SerializeField] private Renderer resistanceRenderer;
        [SerializeField] private Sprite neutralIcon;
        
        [SerializeField] private DecalProjector projector;
        [SerializeField] private float reistanceIconHeight;
        
        private float animationTimer;
        private Databases database;
        private Sprite[] sprites;
        private float currentCountdown;
        private int currentSprite;
        private float spriteSizeDiv;
        private float spriteHeight;
        private Renderer rndr;
        
        private static readonly int IsBackRow = Shader.PropertyToID("isBackRow");
        private static readonly int EffectColour = Shader.PropertyToID("_Effect_Colour");
        
        private float hitEffectTimer;
        private bool isBackRow;
        private Entity.Entity currentEntity;

        private Texture2D[] calculatedSprites;
        
        
        
        private void Awake()
        {
            database = FindObjectOfType<Databases>();
            rndr = GetComponentInChildren<Renderer>();
        }

        public void LoadSprite(Entity.Entity entity, float spriteSize)
        {
            currentEntity = entity;
            sprites = entity.sprites;
            spriteSizeDiv = spriteSize;
            animationTimer = entity.spriteSpeed;
            this.spriteHeight = entity.spriteHeight;
            CalculateSprites();

            UpdateSprite(calculatedSprites[0]);
            
            Vector3 newSize = projector.size;
            newSize.x = (10 * spritePlane.localScale.x);
            projector.size = newSize;

            resistanceObject.transform.localPosition = new Vector3(0, reistanceIconHeight, -0.01f);
        }

        private void CalculateSprites()
        {
            calculatedSprites = new Texture2D[sprites.Length];

            for (int i = 0; i < calculatedSprites.Length; i++)
            {
                calculatedSprites[i] = SpriteUtils.CalculateSprite(sprites[i]);
            }
        }

        public void StartHitEffect(float duration)
        {
            hitEffectTimer = duration;
        }

        public void SetBackRow(bool value)
        {
            isBackRow = value;
        }

        private void Update()
        {
            if (hitEffectTimer > 0.0f)
                hitEffectTimer -= Time.deltaTime;
            else hitEffectTimer = 0f;
            
            if(sprites.Length > 0) 
                UpdateSprite(calculatedSprites[currentSprite]);
        }

        private void UpdateSprite(Texture2D croppedTexture)
        {
            AnimateSprite();
            
            spritePlane.localScale =
                new Vector3(croppedTexture.width / spriteSizeDiv, 1,
                    croppedTexture.height / spriteSizeDiv);

            rndr.material.mainTexture = croppedTexture;
            
            if (UseEffect())
            {
                rndr.material.SetColor(EffectColour, GetOverrideColour());
            }
            else
            {
                int backRow = 0;
                if (isBackRow) backRow = 1;
                rndr.material.SetInteger(IsBackRow, backRow);
                rndr.material.SetColor(EffectColour, GetOverrideColour());
            }

            // Update position to make it stay in the same spot
            float value = rndr.bounds.size.y / 2f;
            spritePlane.localPosition = new Vector3(0, value + spriteHeight, 0);
        }
        
        private void AnimateSprite()
        {
            if (animateSprite && sprites.Length > 1)
            {
                if (currentCountdown > 0f)
                    currentCountdown -= Time.deltaTime;
                
                else if (currentCountdown <= 0f)
                {
                    // Increment sprite
                    currentSprite++;
                    currentCountdown = animationTimer;
                    if (currentSprite >= sprites.Length)
                        currentSprite = 0;
                }
            }
        }

        private Color GetOverrideColour()
        {
            // Have hit effect at the top to override
            if (hitEffectTimer > 0.0f)
            {
                return hitEffectColour;
            }
            return Color.black;
        }

        private bool UseEffect()
        {
            if (hitEffectTimer > 0.0)
                return true;
            else return false;
        }

        public void ShowResistanceIcon(Element? type)
        {
            if (type == null)
                return;

            Element element = (Element) type;
            ResistanceType resistanceType = currentEntity.GetAffinityToElement(element);
            
            Material newMaterial = resistanceRenderer.material;
            newMaterial.mainTexture = resistanceType == ResistanceType.Normal ? neutralIcon.texture : database.GetResistanceSprite(resistanceType).texture;
            resistanceRenderer.material = newMaterial;

            resistanceObject.gameObject.SetActive(true);
        }

        public void HideResistanceIcon()
        {
            resistanceObject.gameObject.SetActive(false);
        }

        public void SetProjectorVisible(bool value)
        {
            projector.gameObject.SetActive(value);
        }
    }
}

