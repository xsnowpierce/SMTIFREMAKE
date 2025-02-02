using UnityEngine;

namespace Game.Pause
{
    public interface PauseMenuPanel
    {
        public void OnOpened();
        public void OnClosed();
        public void OnMovement(Vector2 movement);
        public bool OnCancel();
        public void OnSubmit();
        public void OnTabChange(int direction);
    }
}