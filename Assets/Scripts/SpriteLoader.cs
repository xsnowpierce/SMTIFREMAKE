using UnityEngine;

namespace SpriteUtils
{
    public class SpriteLoader : MonoBehaviour
    {

        [SerializeField] private Transform spritePlane;
        
        public void LoadSprite(Sprite sprite, Material spriteMaterial, float spriteSizeDiv)
        {
            Renderer component = GetComponentInChildren<Renderer>();
            
            // Set the size of our sprite plane accordingly
            Texture2D croppedTexture = new Texture2D( (int)sprite.rect.width, (int)sprite.rect.height );
            Color[] pixels = sprite.texture.GetPixels(  (int)sprite.textureRect.x, 
                (int)sprite.textureRect.y, 
                (int)sprite.textureRect.width, 
                (int)sprite.textureRect.height );
            croppedTexture.SetPixels( pixels );
            croppedTexture.anisoLevel = 0;
            croppedTexture.filterMode = FilterMode.Point;
            croppedTexture.Apply();
        
            spritePlane.localScale =
                new Vector3(croppedTexture.width / spriteSizeDiv, 1,
                    croppedTexture.height / spriteSizeDiv);
        
            Material mat = new Material(spriteMaterial)
            {
                mainTexture = croppedTexture
            };
            component.material = mat;

            // Set the position of our plane so that the sprite is not swimming in the ground
            // Get half of the Z value and increase the planes position by that

            float value = component.bounds.size.y / 2f;
            spritePlane.position += new Vector3(0, value, 0);
        }
    }
}