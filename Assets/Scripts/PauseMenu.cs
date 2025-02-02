using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Pause 
{
    public class PauseMenu : MonoBehaviour
    {
        private PlayerInput input;
        [SerializeField] private UIManager uiManager;
        private bool isPauseOpened;
        private float lastTimescale;
        [SerializeField] private Camera camMain;
        [SerializeField] private Camera pauseCam;
        [SerializeField] private GameObject disableObject;
        private int inMenu = -1;
        private int sideBarSelected = 0;
        [Header("Sidebar")] 
        public Image[] sidebarBackgrounds;
        public Color selectedColour;
        public Color normalColour;
        [Header("Menus")] 
        [SerializeField] private GameObject[] menuPanels;
        private PauseMenuPanel currentPauseMenuPanel;


        private void Awake()
        {
            SelectSideBar(sideBarSelected);
            disableObject.SetActive(false);
            input = FindObjectOfType<PlayerInput>();
            input.actions["Movement"].performed += Movement;
            input.actions["Menu"].performed += ctx => MenuButton();
            input.actions["MenuCancel"].performed += ctx => CancelButtonPressed();
            input.actions["Interact"].performed += ctx => SubmitButtonPressed();
            input.actions["MenuTabChange"].performed += TabChange;
        }

        private void TabChange(InputAction.CallbackContext ctx)
        {
            if (!isPauseOpened) return;
            if (inMenu > -1)
            {
                currentPauseMenuPanel.OnTabChange(ctx.ReadValue<int>());
            }
        }

        private void MenuButton()
        {
            if (!isPauseOpened && Time.timeScale > 0 && uiManager.CanOpenPauseMenu())
            {
                lastTimescale = Time.timeScale;
                Time.timeScale = 0;
                isPauseOpened = true;
                disableObject.SetActive(true);
            }
            else if (isPauseOpened && Time.timeScale == 0)
            {
                ClosePauseMenu();
            }
        }
        
        private void CancelButtonPressed()
        {
            if (!isPauseOpened) return;
            
            if(inMenu == -1)
                ClosePauseMenu();
            
            else
            {
                // Send cancel button to the current panel
                // If the panel returns true, it still remains
                // if it returns false, it is being exited out of
                if (currentPauseMenuPanel.OnCancel()) return;
                
                menuPanels[inMenu].SetActive(false);
                currentPauseMenuPanel = null;
                inMenu = -1;
            }
        }

        private void SubmitButtonPressed()
        {
            if (!isPauseOpened) return;

            if (inMenu == -1)
                // Open a menu at sidebarselected
                OpenMenu(sideBarSelected);
            else
                currentPauseMenuPanel.OnSubmit();
        }

        private void OpenMenu(int menu)
        {
            sideBarSelected = menu;
            inMenu = menu;
            camMain.enabled = false;
            pauseCam.enabled = true;
            menuPanels[menu].SetActive(true);
            currentPauseMenuPanel = menuPanels[menu].GetComponent<PauseMenuPanel>();
            currentPauseMenuPanel.OnOpened();
        }

        private void Movement(InputAction.CallbackContext ctx)
        {
            if (!isPauseOpened) return;
            
            Vector2 moveValue = new Vector2();
            if (ctx.ReadValue<Vector2>().y > 0)
                moveValue.y = 1;
            else if (ctx.ReadValue<Vector2>().y < 0)
                moveValue.y = -1;
            if (ctx.ReadValue<Vector2>().x > 0)
                moveValue.x = 1;
            else if (ctx.ReadValue<Vector2>().x < 0)
                moveValue.x = -1;
            
            if (inMenu == -1)
                // Move sidebar up and down
                SelectSideBar((int) -moveValue.y);
            else currentPauseMenuPanel.OnMovement(-moveValue);
        }

        private void SelectSideBar(int id)
        {
            int newSelect = 0;
            if(sideBarSelected + id > sidebarBackgrounds.Length - 1)
            {
                // Clamp to top
                newSelect = 0;
            }
            else
            {
                // Move normally
                newSelect = id + sideBarSelected;
            }

            if (newSelect < 0)
                newSelect = sidebarBackgrounds.Length - 1;
            
            // Add selection
            sidebarBackgrounds[sideBarSelected].color = normalColour;
            sidebarBackgrounds[newSelect].color = selectedColour;
            sideBarSelected = newSelect;
        }

        private void ClosePauseMenu()
        {
            if (inMenu > -1)
            {
                currentPauseMenuPanel.OnClosed();
                menuPanels[inMenu].SetActive(false);
            }

            pauseCam.enabled = false;
            camMain.enabled = true;
            currentPauseMenuPanel = null;
            Time.timeScale = lastTimescale;
            isPauseOpened = false;
            disableObject.SetActive(false);
            inMenu = -1;
        }
    }
}