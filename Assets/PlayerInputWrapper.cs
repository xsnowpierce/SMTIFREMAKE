using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public class PlayerInputWrapper : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;

        public Vector2 GetMovement()
        {
            return playerInput.actions["Movement"].ReadValue<Vector2>();
        }

        public Vector2 GetFieldMovement()
        {
            Vector2 upDown = playerInput.actions["Movement"].ReadValue<Vector2>();
            Vector2 leftRight = playerInput.actions["StrafeMovement"].ReadValue<Vector2>();
            return new Vector2(leftRight.x, upDown.y);
        }

        public Vector2 GetRotation()
        {
            return new Vector2(playerInput.actions["Movement"].ReadValue<Vector2>().x, 0);
        }

        public InputAction GetMovementAction()
        {
            return playerInput.actions["Movement"];
        }

        public InputAction GetSprint()
        {
            return playerInput.actions["Sprint"];
        }

        public InputAction GetMapToggler()
        {
            return playerInput.actions["FullMap"];
        }

        public InputAction GetMenuToggler()
        {
            return playerInput.actions["Menu"];
        }

        public InputAction GetInteract()
        {
            return playerInput.actions["Interact"];
        }

        public InputAction GetCancel()
        {
            return playerInput.actions["MenuCancel"];
        }

        public InputAction GetMouse()
        {
            return playerInput.actions["Mouse"];
        }

        public InputAction GetMouseLeftButton()
        {
            return playerInput.actions["LeftMouse"];
        }

        public InputAction GetMwToggler()
        {
            return playerInput.actions["MwOff"];
        }

        public InputAction GetFastForwardToggler()
        {
            return playerInput.actions["FastForward"];
        }

        public PlayerInput GetPlayerInputScript() => playerInput;


    }
}