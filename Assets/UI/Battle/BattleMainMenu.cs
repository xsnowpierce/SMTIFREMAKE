using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Battle
{
    public class BattleMainMenu : BattleMenuScript
    {
        
        protected override void InitializeButtonTexts()
        {
            BUTTON_TEXTS = new string[5];
            BUTTON_TEXTS[0] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_main_fight");
            BUTTON_TEXTS[1] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_main_talk");
            BUTTON_TEXTS[2] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_main_comp");
            BUTTON_TEXTS[3] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_main_escape");
            BUTTON_TEXTS[4] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_main_auto");
        }
        
        public override void OnOpened()
        {
            
        }
        
        public override void OnCancel()
        {
            // Go to top and play cancelled sound
            if (currentSelected != 0)
            {
                PlaySound("ui_cancel");
                nextSelect = 0;
                SelectCurrent();
            }
        }

        public override void OnAccept()
        {
            if (!isOpened || !gameObject.activeSelf) return;
            
            PlaySound("ui_select");

            switch (currentSelected)
            {
                case 0:
                    // Fight!
                    selectionUI.OpenFight();
                    return;
                case 1:
                    // Talk!
                    selectionUI.OpenTalk();
                    break;
                case 2:
                    // Comp!
                    selectionUI.OpenComp();
                    break;
                case 3:
                    // Escape!
                    break;
                case 4:
                    // Auto battle!
                    break;
            }
        }

        
    }
}