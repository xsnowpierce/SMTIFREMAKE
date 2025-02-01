using Game.Input;
using UnityEngine;

namespace Game.WorldMap
{
    [RequireComponent(typeof(CharacterController))]
    public class MapMover : MonoBehaviour
    {
        private CharacterController controller;
        private MapInteract interact;
        [SerializeField] private PlayerInputWrapper input;
        [SerializeField] private float movementSpeed;
        [SerializeField] private Vector3 gravity;

        private void OnEnable()
        {
            interact = GetComponent<MapInteract>();
            controller = GetComponent<CharacterController>();
            enabled = true;
        }

        private void Update()
        {
            Movement();
            Gravity();
        }

        private void Gravity()
        {
            controller.Move(gravity * Time.deltaTime);
        }

        private void Movement()
        {
            if (interact.IsInteracting()) return;
            Vector2 inp = input.GetMovement();
            Vector3 movement = new Vector3(inp.x, 0, inp.y);
            movement *= movementSpeed * Time.deltaTime;
            controller.Move(movement);
        }
    }
}