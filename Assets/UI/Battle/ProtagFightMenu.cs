using System;

namespace Game.Battle
{
    public class ProtagFightMenu : BattleMenuScript
    {

        protected override void InitializeButtonTexts()
        {
            BUTTON_TEXTS = new string[5];
            BUTTON_TEXTS[0] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_protag_attack");
            BUTTON_TEXTS[1] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_protag_summon");
            BUTTON_TEXTS[2] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_protag_item");
            BUTTON_TEXTS[3] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_protag_move");
            BUTTON_TEXTS[4] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_protag_guard");
        }

        public override void OnOpened()
        {
            
        }

        public override void OnAccept()
        {
            PlaySound("ui_select");
        }

        public override void OnCancel()
        {
            PlaySound("ui_cancel");
        }
    }
}