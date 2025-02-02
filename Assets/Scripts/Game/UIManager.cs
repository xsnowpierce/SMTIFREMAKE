using UnityEngine;

namespace Game
{
    
    public class UIManager : MonoBehaviour
    {
        private Animator uiAnimator;
        private bool canOpenPartyStatus = true;
        private bool canOpenPauseMenu = true;

        private void Awake()
        {
            uiAnimator = GetComponent<Animator>();
        }
        
        public void ShowUI()
        {
            uiAnimator.Play("Pull In", 0, 0.0f);
        }

        public void HideUI()
        {
            uiAnimator.Play("Pull Out", 0, 0.0f);
        }

        public bool CanOpenPartyStatus() => canOpenPartyStatus;
        public void SetCanOpenPartyStatus(bool value) => canOpenPartyStatus = value;
        
        public bool CanOpenPauseMenu() => canOpenPauseMenu;
        public void SetCanOpenPauseMenu(bool value) => canOpenPauseMenu = value;

    }
}