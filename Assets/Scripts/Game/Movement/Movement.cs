using System;
using System.Collections;
using Game.Audio;
using Game.Collision;
using Game.Input;
using Game.Level;
using Game.Minimap;
using Game.UI;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Game.Movement
{
    public class Movement : MonoBehaviour
    {
        [Header("General Variables")] 
        [SerializeField] private bool allowMovement = true;
        [SerializeField] private bool allowRunning;

        [Header("Wall Collision Variables")] 
        [SerializeField] protected Animator cameraAnimator;

        [Header("Movement Settings")] 
        [SerializeField] private bool canWalkBackwards = false;
        [SerializeField] private float walkingSeconds;
        [SerializeField] private float runningSeconds;
        [SerializeField] private float gridSize = 10;
        [SerializeField] private float rayCastDistance = 10f;
        [SerializeField] private float currentSpeed;

        private bool running;
        private bool isMoving;
        private PlayerRotation rotation;
        private PlayerDoorEntry doorEntry;
        [SerializeField] private PlayerInputWrapper input;
        private PlayerInteract playerInteract;
        private SFXController source;
        private MoonPhaseSystem moonPhase;
        private FloorUI floorUI;
        private int lastRotation = -1;

        protected virtual void Awake()
        {
            // Reset values just incase
            isMoving = false;
            currentSpeed = walkingSeconds;
            
            floorUI = FindObjectOfType<FloorUI>();
            moonPhase = FindObjectOfType<MoonPhaseSystem>();
            source = FindObjectOfType<SFXController>();
            rotation = GetComponent<PlayerRotation>();
            doorEntry = GetComponent<PlayerDoorEntry>();
            playerInteract = FindObjectOfType<PlayerInteract>();
        }

        private void Update()
        {
            ProcessInputs();
        }

        private void ProcessInputs()
        {
            if (Time.timeScale == 0) return;
            if (!allowMovement) return;
            if (rotation.isPlayerRotating() || isMoving) return;

            Vector2 movement = input.GetFieldMovement();
            if (movement != Vector2.zero)
                Move(movement);
            Rotate(input.GetRotation().x);
            Sprint(input.GetSprint().ReadValue<float>());
        }

        private void Move(Vector2 movement)
        {
            // Digitize input
            if (movement.x > 0) movement.x = 1;
            else if (movement.x < 0) movement.x = -1;
            if (movement.y > 0) movement.y = 1;
            else if (movement.y < 0) movement.y = -1;

            // Make sure that we aren't moving diagonally
            if (movement.x != 0 && movement.y != 0)
            {
                // We are trying to move diagonally, always prefer X movement
                movement.y = 0;
            }

            if (movement.x == 0 && movement.y == -1)
            {
                if(!canWalkBackwards && !running) 
                    DoBackwardsRotation();
                else if(running)
                {
                    StartCoroutine(ProcessMove(movement));
                }
            }
            else
                StartCoroutine(ProcessMove(movement));
        }

        private IEnumerator ProcessMove(Vector2 movement)
        {
            isMoving = true;

            // Determine if we are allowed to move- check with raycasts in the direction if there is anything blocking us
            // If there is, check if it might be a door or a staircase.
            // if not, allow movement

            int layer = LayerMask.GetMask("CollisionObject");

            // Get direction relative to transform
            Vector3 direction = new Vector3();
            if (movement.y < 0)
            {
                if (canWalkBackwards || running)
                    direction = -transform.forward;
                else
                {
                    // TODO Perform a full rotate here
                    isMoving = false;
                    yield break;
                }
            }
            else if (movement.y > 0) direction = transform.forward;
            else if (movement.x < 0) direction = -transform.right;
            else direction = transform.right;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, rayCastDistance, layer))
            {
                
                // Hit a wall- check if it's  out hit,a door or something
                if (hit.collider.GetComponent<MapCollider>() == null)
                {
                    // Just a normal wall, break
                    StartCoroutine(RunIntoWall(movement));
                    yield break;
                }

                // Collision was some sort of object.
                MapCollider collided = hit.collider.GetComponent<MapCollider>();
                if (collided.GetType().IsSubclassOf(typeof(MapDoor)))
                {
                    MapDoor door = collided as MapDoor;

                    // MAKE SURE WE ARE TRYING TO ENTER FROM FORWARD
                    if (movement.x != 0 || movement.y < 0)
                    {
                        // Player is moving sideways or backwards
                        StartCoroutine(RunIntoWall(movement));
                        yield break;
                    }

                    // Process door movement
                    try
                    {
                        door = hit.collider.GetComponent<MapDoor>();
                    }
                    catch (ArgumentException)
                    {
                        Debug.LogError("The door was not a door??");
                        // just block movement here then
                        StartCoroutine(RunIntoWall(movement));
                        yield break;
                    }

                    // Enter the door!
                    doorEntry.EnterDoor(door);
                }
                else if (collided.GetType() == typeof(MapStaircase))
                {
                    MapStaircase staircase = collided as MapStaircase;
                    Vector3 endPosition = staircase.GetEndPosition();
                    Vector3 walkTo = staircase.GetWalkTo();
                    float waitForIncline = staircase.GetInclineWaitTime();
                    MapLoader loader = FindObjectOfType<MapLoader>();
                    
                    // Process loading/unloading of floors
                    if (staircase.GetSetMainFloor() > 0)
                    {
                        // Set the new main floor number
                        loader.GetCurrentMapData().SetMainFloor(staircase.GetSetMainFloor());
                    }
                    if (staircase.GetLoadFloor() > 0)
                    {
                        // Not default, so load
                        loader.GetCurrentMapData().LoadFloor(staircase.GetLoadFloor());
                    }
                    if (staircase.GetUnloadFloor() > 0)
                    {
                        // Not default, so unload
                        loader.GetCurrentMapData().UnloadFloor(staircase.GetUnloadFloor(), loader.GetCurrentMapData().GetCurrentMainFloor());
                    }
                    
                    // Start moving towards X and Z of position, but wait for Y
                    float time = 0;
                    Vector3 startPosition = transform.position;
                    while (time < staircase.GetClimbSpeed())
                    {
                        // TODO ADD SOUND
                        Vector3 moveTo = endPosition;
                        if (time < waitForIncline) moveTo.y = startPosition.y;
                        transform.position = Vector3.Lerp(startPosition, moveTo, time / staircase.GetClimbSpeed());
                        time += Time.deltaTime;
                        yield return null;
                    }
                    
                    // Walk to the new tile position now
                    time = 0;
                    startPosition = transform.position;
                    
                    float timeToComplete = staircase.GetClimbSpeed() * .25f;
                    
                    
                    while (time < timeToComplete)
                    {
                        // TODO ADD SOUND
                        Vector3 moveTo = walkTo;
                        if (time < waitForIncline) moveTo.y = startPosition.y;
                        transform.position = Vector3.Lerp(startPosition, moveTo, time / timeToComplete);
                        time += Time.deltaTime;
                        yield return null;
                    }

                    transform.position = walkTo;
                    
                    // Reset to allow for another walk
                    isMoving = false;
                    yield break;
                }
                else
                {
                    // Couldn't find anything, bump into it
                    Debug.LogError("Tried to interact with MapCollider, but didn't have a type reaction. Type: " +
                                   collided.GetType());
                    StartCoroutine(RunIntoWall(movement));
                    yield break;
                }
                // Add other types of MapColliders here
            }
            else
            {
                // Nothing is in our way, so let's move in this direction
                Transform playerTransform = transform;
                Vector3 forward = playerTransform.forward;
                Vector3 right = playerTransform.right;

                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();

                Vector3 desiredDirection = forward * movement.y + right * movement.x;

                StartCoroutine(Step(desiredDirection, currentSpeed));
            }
        }

        private void DoBackwardsRotation()
        {
            rotation.FullRotate(lastRotation);
        }

        private void Sprint(float runningValue)
        {
            if (runningValue > 0)
            {
                currentSpeed = runningSeconds;
                running = true;
            }
            else
            {
                currentSpeed = walkingSeconds;
                running = false;
            }
        }

        private void Rotate(float rotate)
        {
            if (rotate != 0.0f)
            {
                // Call the turn method
                rotation.RotatePlayer(rotate);
                lastRotation = (int)rotate;
            }
        }

        private IEnumerator Step(Vector3 direction, float duration)
        {
            // Reset current (if possible) interactable
            playerInteract.ResetInteract();
            
            float time = 0;

            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + (direction * gridSize);

            
            bool madeSound = false;
            while (time < duration)
            {
                if (time >= duration / 2f && !madeSound)
                {
                    source.PlaySound("footstep");
                    madeSound = true;
                }
                Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
                transform.position = newPosition;
                time += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            
            // Allow the interact box to appear if possible
            playerInteract.CheckForObject();
            moonPhase.StepMade();
            floorUI.CheckForDifference();
            
            // Reset to allow for another walk
            isMoving = false;
            yield return null;
        }

        private IEnumerator RunIntoWall(Vector2 direction)
        {
            if (direction == Vector2.up) cameraAnimator.Play("HitWall", 0, 0.0f);
            else if (direction == Vector2.down) cameraAnimator.Play("HitWallDown", 0, 0.0f);
            else if (direction == Vector2.left) cameraAnimator.Play("HitWallLeft", 0, 0.0f);
            else if (direction == Vector2.right) cameraAnimator.Play("HitWallRight", 0, 0.0f);

            bool thumped = false;
            while (cameraAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                if (cameraAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f && thumped == false)
                {
                    source.PlaySound("collision");
                    thumped = true;
                }

                yield return null;
            }

            isMoving = false;
        }

        public void ForceForwardMovement()
        {
            Vector2 dir = new Vector2(0, 1);

            Transform playerTransform = transform;
            Vector3 forward = playerTransform.forward;
            Vector3 right = playerTransform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredDirection = forward * dir.y + right * dir.x;
            StartCoroutine(Step(desiredDirection, walkingSeconds));
        }

        public void ForceMovable()
        {
            isMoving = false;
        }

        public bool IsRunning() => running;
        public bool IsPlayerMoving() => isMoving;

        public MovementSpeed GetCurrentSpeed()
        {
            if (currentSpeed == runningSeconds)
                return MovementSpeed.FAST;
            else return MovementSpeed.NORMAL;
        }

        public void SetCanMove(bool value)
        {
            this.allowMovement = value;
        }

        public bool GetCanMove() => allowMovement;
    }
}