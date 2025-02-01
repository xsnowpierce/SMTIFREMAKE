using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UltrawideBorders : MonoBehaviour
    {
        [SerializeField] private bool useBorders = true;
        [SerializeField] private Image leftBorder;
        [SerializeField] private Image rightBorder;

        public void SetBordersVisible(bool value)
        {
            if (!useBorders) return;
            
            if (value)
            {
                SizeCalculation();
            }
            else
            {
                leftBorder.enabled = false;
                rightBorder.enabled = false;
            }
        }

        private void SizeCalculation()
        {
            float aspect = Camera.main.aspect;
            float widescreenAspect = 1.77777778f;
            
            if (Approximately(aspect, widescreenAspect, 0.01f) || aspect <= widescreenAspect)
            {
                // borders are not necessary
                leftBorder.enabled = false;
                rightBorder.enabled = false;
            }
            else if (aspect > widescreenAspect)
            {
                // we must use borders!
                leftBorder.enabled = true;
                rightBorder.enabled = true;
                float wideScreenDifference = aspect - widescreenAspect;
                float size = (Screen.width * wideScreenDifference / aspect) / 2f;
                leftBorder.rectTransform.sizeDelta = new Vector2(size, 1f);
                rightBorder.rectTransform.sizeDelta = new Vector2(size, 1f);
            }
        }
        
        private bool Approximately(float a, float b, float threshold)
        {
            return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
        }
    }
}