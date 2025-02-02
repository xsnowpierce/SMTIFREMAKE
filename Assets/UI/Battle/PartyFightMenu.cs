namespace Game.Battle
{
    public class PartyFightMenu : BattleMenuScript
    {
        protected override void InitializeButtonTexts()
        {
            BUTTON_TEXTS = new string[4];
            BUTTON_TEXTS[0] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_human_attack");
            BUTTON_TEXTS[1] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_human_item");
            BUTTON_TEXTS[2] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_human_move");
            BUTTON_TEXTS[3] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_human_guard");
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