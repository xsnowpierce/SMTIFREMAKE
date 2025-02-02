using Data;
using Game.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Battle
{
    public abstract class BattleMenuScript : MonoBehaviour, BattleMenu
    {
        protected int currentSelected;
        protected int nextSelect;
        public bool isOpened { get; protected set; }
        protected SaveData data;
        protected SelectionUI selectionUI;
        protected string[] BUTTON_TEXTS;
        private SFXController soundController;
        
        [SerializeField] protected Image[] selectedImage;
        [SerializeField] protected Color selectedColour;
        [SerializeField] protected Color normalColour;

        protected virtual void Awake()
        {
            soundController = FindObjectOfType<SFXController>();
        }

        protected void PlaySound(string soundID)
        {
            soundController.PlaySound(soundID);
        }

        protected abstract void InitializeButtonTexts();
        
        public virtual void OnOpen(SelectionUI selectionUI)
        {
            //
            this.selectionUI = selectionUI;
            InitializeButtonTexts();
            data = FindObjectOfType<SaveData>();
            isOpened = true;
            OnOpened();
            SelectCurrent();
        }

        public virtual void OnClose()
        {
            //
            isOpened = false;
        }
        
        public virtual void Movement(Vector2 movement)
        {
            if (!isOpened) return;
            switch (movement.y)
            {
                case 0:
                    return;
                // Moving up
                case > 0 when currentSelected == 0:
                    // Wrap to bottom
                    nextSelect = selectedImage.Length - 1;
                    break;
                case > 0:
                    // Go up
                    nextSelect = currentSelected - 1;
                    break;
                default:
                {
                    // Moving down
                    if (currentSelected == selectedImage.Length - 1)
                    {
                        // Wrap to top
                        nextSelect = 0;
                    }
                    else
                    {
                        // Move down
                        nextSelect = currentSelected + 1;
                    }
                    break;
                }
            }
            PlaySound("ui_move");
            SelectCurrent();
        }

        protected virtual void SelectCurrent()
        {
            if (currentSelected >= selectedImage.Length)
            {
                currentSelected = 0;
                nextSelect = 0;
            }
            selectedImage[currentSelected].color = normalColour;
            selectedImage[nextSelect].color = selectedColour;
            currentSelected = nextSelect;
            selectionUI.SetTopBarText(BUTTON_TEXTS[currentSelected]);
        }
        
        public abstract void OnOpened();
        public abstract void OnAccept();
        public abstract void OnCancel();
        public int GetSelectedID() => currentSelected;
    }
}