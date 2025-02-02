using UnityEngine;

namespace Game.Battle
{
    public interface BattleMenu
    {
        public void OnOpen(SelectionUI selectionUI);
        public void OnClose();
        public void Movement(Vector2 movement);
        public void OnAccept();
        public void OnCancel();
    }
}