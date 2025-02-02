namespace Game.Battle
{
    public class DemonFightMenu : BattleMenuScript
    {
        protected override void InitializeButtonTexts()
        {
            BUTTON_TEXTS = new string[3];
            BUTTON_TEXTS[0] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_demon_attack");
            BUTTON_TEXTS[1] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_demon_return");
            BUTTON_TEXTS[2] = selectionUI.GetDatabase().GetTranslatedUIElement("battle_demon_guard");
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