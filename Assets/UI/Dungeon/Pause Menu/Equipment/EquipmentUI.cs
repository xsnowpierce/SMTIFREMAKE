using UnityEngine;

namespace Game.Pause.Equipment
{
    public class EquipmentUI : MonoBehaviour, PauseMenuPanel
    {
        public void OnOpened()
        {
        }

        public void OnClosed()
        {
        }

        public void OnMovement(Vector2 movement)
        {
        }

        public bool OnCancel()
        {
            return false;
        }

        public void OnSubmit()
        {
        }

        public void OnTabChange(int direction)
        {
        }
    }
}