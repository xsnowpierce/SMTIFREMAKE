using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Canvas
{
    
    [RequireComponent(typeof(AspectRatioFitter))]
    [RequireComponent(typeof(RectTransform))]
    public class CanvasAspectScaler : MonoBehaviour
    {
        private AspectRatioFitter fitter;
        private RectTransform rect;
        private PlayerInput input;
        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            fitter = GetComponent<AspectRatioFitter>();
            input = FindObjectOfType<PlayerInput>();
            input.actions["Menu"].performed += _ => UpdateAspectRatio();
            UpdateAspectRatio();
        }

        public void UpdateAspectRatio()
        {
            float aspect = Camera.main.aspect;
            if (aspect >= 1.7f)
            {
                // 16:9 or greater, enable fitter
                fitter.enabled = true;
            }
            
            else
            {
                // lower than 16:9, disable fitter
                fitter.enabled = false;
                rect.offsetMax = Vector2.zero;
                rect.offsetMin = Vector2.zero;
            }
        }
    }
}