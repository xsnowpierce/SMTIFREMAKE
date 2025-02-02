using Game.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Battle.UI
{
    public class BattleConfirmMenu : MonoBehaviour
    {
        private bool isOpened;
        private int selectedID;
        [SerializeField] private Image[] selectionImages;
        [SerializeField] private GameObject boxObject;
        private SFXController sfxController;

        private void Awake()
        {
            sfxController = FindObjectOfType<SFXController>();
            CloseMenu();
        }
        
        public void OpenMenu()
        {
            boxObject.SetActive(true);
            selectedID = 0;
            selectionImages[0].enabled = true;
            selectionImages[1].enabled = false;
            isOpened = true;
            
            // TODO add an opening animation here, and delay input until finished
        }

        public void CloseMenu()
        {
            // TODO add a closing animation here, and delay "isOpened = false" until finished
            boxObject.SetActive(false);
            isOpened = false;
        }

        public void Movement()
        {
            if (!isOpened)
            {
                Debug.LogError("Movement was passed on to the Confirm Menu, but it wasn't opened.");
                return;
            }
            sfxController.PlaySound("ui_move");
            int newSelection = 0;
            if (selectedID == 0) newSelection = 1;
            SetSelection(newSelection);
        }

        public void Selection()
        {
            if (!isOpened)
            {
                Debug.LogError("Movement was passed on to the Confirm Menu, but it wasn't opened.");
                return;
            }
            sfxController.PlaySound(selectedID == 0 ? "ui_select" : "ui_cancel");
            CloseMenu();
        }

        private void SetSelection(int id)
        {
            int lastID = selectedID;
            selectedID = id;

            selectionImages[lastID].enabled = false;
            selectionImages[selectedID].enabled = true;
        }

        public int GetSelected() => selectedID;
        public bool IsOpened() => isOpened;
    }
}