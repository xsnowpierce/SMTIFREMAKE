using System.Collections;
using Game.Collision;
using Game.Events;
using Game.Events.EventTypes;
using Game.Minimap;
using UnityEngine;

namespace Game.Movement
{
    [RequireComponent(typeof(Movement))]
    public class PlayerRotation : MonoBehaviour
    {
        // Un-editable values
        private Movement _movement;
        private bool isRotating;

        [SerializeField] private float walkingSpeed = 0.26f;
        [SerializeField] private float runningSpeed = 0.2f;
        private float currentSpeed;
        
        // EVENT VARIABLES
        [Header("Event Variables")]
        [SerializeField] private IntEvent lookEvent;
        [SerializeField] private PlayerInteract playerInteract;

        private void Awake()
        {
            isRotating = false;
            _movement = GetComponent<Movement>();
        }
        
        public void RotatePlayer(float direction)
        {
            // Digitize the rotation input
            // Make the value passed always 1 or -1, even if it really is 0.4 or -0.0000001
            if (direction > 0) direction = 1;
            else if (direction < 0) direction = -1;
            // cast the float direction into an int
            int newdir = (int) direction;

            // if we are still having an input, then start the turning
            if (direction != 0)
                StartCoroutine(Turn(newdir, _movement.GetCurrentSpeed(), false));
        }

        public void FullRotate(int lastDirection)
        {
            StartCoroutine(Turn(lastDirection, _movement.GetCurrentSpeed(), true));
        }
        
        private IEnumerator Turn(int direction, MovementSpeed speed, bool fullRotation)
        {
            if (isRotating || _movement.IsPlayerMoving()) yield break;
            isRotating = true;

            if (speed == MovementSpeed.FAST)
                currentSpeed = runningSpeed;
            else currentSpeed = walkingSpeed;
            
            float time = 0;
            
            Vector3 startRotation = transform.rotation.eulerAngles;
            Vector3 targetRotation = startRotation;
            if (!fullRotation)
                targetRotation.y += (direction * 90);
            else targetRotation.y += (direction * 180);
            
            // Reset interactable
            playerInteract.ResetInteract();
            
            while (time < currentSpeed)
            {
                Vector3 euler = Vector3.Lerp(startRotation, targetRotation, time / currentSpeed);
                transform.rotation = Quaternion.Euler(euler);
                time += Time.deltaTime;
                yield return null;
            }

            transform.rotation = Quaternion.Euler(targetRotation);

            isRotating = false;
            
            // Raise the player rotate event
            lookEvent.Raise((int) transform.rotation.eulerAngles.y);
            // Alert interactable that we might be looking at one
            playerInteract.CheckForObject();
        }


        public void CallTurnEvent()
        {
            lookEvent.Raise((int) transform.rotation.eulerAngles.y);
            // Alert interactable that we might be looking at one
            playerInteract.CheckForObject();
        }

        public float GetWalkingSpeed() => walkingSpeed;
        public float GetRunningSpeed() => runningSpeed;
        public bool isPlayerRotating() => isRotating;
    }
}