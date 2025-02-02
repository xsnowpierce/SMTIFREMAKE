using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(Image))]
    public class GifAnimator : MonoBehaviour
    {
        private Image _image;
        [SerializeField] private Sprite[] frames;
        [SerializeField] private float frameDelay;
        private int currentFrame;
        private float timer;
        
        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer > frameDelay)
            {
                currentFrame++;
                timer = 0.0f;
            }

            if (currentFrame >= frames.Length)
            {
                currentFrame = 0;
            }

            _image.sprite = frames[currentFrame];
        }

        public void LoadNewSprites(Sprite[] sprites, float delay)
        {
            frames = sprites;
            frameDelay = delay;
        }
    }
}
