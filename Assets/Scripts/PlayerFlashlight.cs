using Game.Collision;
using UnityEngine;

namespace Game.Movement
{
    public class PlayerFlashlight : MonoBehaviour
    {
        [SerializeField] private Light flashlight;

        public void ToggleFlashlight(bool value)
        {
            flashlight.enabled = value;
        }
    }
}