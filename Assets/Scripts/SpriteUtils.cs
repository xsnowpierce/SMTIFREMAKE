using UnityEngine;
using UnityEngine.UI;

namespace SpriteUtils
{
    public static class SpriteUtils
    {
        public static float spriteSizeDiv = 75;
        
        public static Texture2D CalculateSprite(Sprite sprite)
        {
            Texture2D croppedTexture = new Texture2D( (int)sprite.rect.width, (int)sprite.rect.height );
            
            Color[] pixels = sprite.texture.GetPixels(  (int)sprite.rect.x, 
                (int)sprite.rect.y, 
                (int)sprite.rect.width, 
                (int)sprite.rect.height );
            
            croppedTexture.SetPixels( pixels );
            croppedTexture.anisoLevel = 0;
            croppedTexture.filterMode = FilterMode.Point;
            croppedTexture.Apply();
        
            return croppedTexture;
        }

        public static void ApplySpriteToImage(Image image, Sprite sprite)
        {
            Texture2D fixedTexture = CalculateSprite(sprite);
            image.rectTransform.localScale =
                new Vector3(fixedTexture.width / spriteSizeDiv, fixedTexture.height / spriteSizeDiv, 1);
            Sprite newSprite = Sprite.Create(fixedTexture, new Rect(0, 0, fixedTexture.width, fixedTexture.height), new Vector2(0.5f, 0.5f));
            image.sprite = newSprite;
        }
    }
}