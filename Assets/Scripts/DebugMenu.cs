using UnityEngine;
using UnityEngine.InputSystem;

namespace DebugMenu
{
    public class DebugMenu : MonoBehaviour
    {
        [SerializeField] private PlayerInput input;
        private bool IsMenuOpen;

        private void Awake()
        {
            input.actions["DebugMenu"].performed += ctx => DebugMenuButton();
        }
        
        private void Update()
        {
            if (IsMenuOpen)
                Time.timeScale = 0f;
            else Time.timeScale = 1f;
        }

        private void DebugMenuButton()
        {
            
        }
        
    }
}