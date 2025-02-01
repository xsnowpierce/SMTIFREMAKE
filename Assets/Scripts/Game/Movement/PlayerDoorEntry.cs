using System;
using System.Collections;
using Dialogue;
using Game.Audio;
using Game.Collision;
using Game.Encounter;
using Game.Interactable;
using Game.Level;
using Game.Minimap;
using Game.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Game.Movement
{
    [RequireComponent(typeof(Movement))]
    public class PlayerDoorEntry : MonoBehaviour
    {
        private MapLoader loader;
        private SFXController sfxController;

        [SerializeField] private CurrentCollision currentCollision;
        [SerializeField] private Movement movement;
        [SerializeField] private Camera cam;
        [SerializeField] private Animator cameraAnimator;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private InteractUI interactUI;
        [SerializeField] private DungeonDialogueController dialogue;
        [SerializeField] private PlayerFlashlight flashlight;

        [Header("Door Transition Options")]
        [SerializeField] private Image screenFadeImage;
        [SerializeField] private float screenFadeSpeed;
        [SerializeField] private float doorBlackTime;
        
        private void Awake()
        {
            sfxController = FindObjectOfType<SFXController>();
            loader = FindObjectOfType<MapLoader>();
        }

        public void EnterDoor(MapDoor door)
        {
            if (door.GetType() == typeof(DoorStaircase))
            {
                // Fade screen out, unload current map and load staircase
                DoorStaircase staircase = door as DoorStaircase;
                StartCoroutine(OpenRoom(staircase));
            }
            else if (door.GetType() == typeof(DoorRoom))
            {
                DoorRoom room = door as DoorRoom;
                StartCoroutine(OpenRoom(room));
            }
            else if (door.GetType() == typeof(DoorWalkable))
            {
                DoorWalkable walkable = door as DoorWalkable;
                if (walkable == null)
                {
                    Debug.LogError("There was no object on the other side of the door set.");
                    return;
                }
                
                if (walkable.FadeScreenOnLoad())
                {
                    StartCoroutine(LoadThroughDoor(walkable));
                }
                else StartCoroutine(WalkThroughDoor(walkable));
            }
            else if (door.GetType() == typeof(DoorMainWalkable))
            {
                DoorMainWalkable walkable = door as DoorMainWalkable;
                if (walkable == null)
                {
                    Debug.LogError("There was no object on the other side of the door set.");
                    return;
                }
                
                if (walkable.FadeScreenOnLoad())
                {
                    StartCoroutine(LoadThroughDoor(walkable));
                }
                else StartCoroutine(WalkThroughDoor(walkable));
            }
        }

        private IEnumerator LoadThroughDoor(DoorWalkable walkable)
        {
            if (walkable.GetNextAreaPrefab() == null)
            {
                Debug.LogError("Player tried to access a door that doesn't exist.");
                movement.ForceMovable();
                yield break;
                
            }

            if (walkable.getDoorAnimator() != null)
            {
                sfxController.PlaySound("slidingDoor");
                walkable.getDoorAnimator().Play("DoorOpen", 0, 0.0f);
                yield return new WaitForSeconds(0.1f);
            }

            // Force player to walk through door
            movement.ForceForwardMovement();
            movement.SetCanMove(false);
            
            // Fade to black
            for (float i = 0; i < 1; i += Time.deltaTime * (screenFadeSpeed * 2))
            {
                screenFadeImage.color = new Color(0, 0, 0, i);
                yield return null;
            }
            screenFadeImage.color = new Color(0, 0, 0, 1);
            yield return new WaitForSeconds(doorBlackTime);
            
            // Make sure player has stopped moving 
            while (movement.IsPlayerMoving())
            {
                yield return null;
            }
            
            // Spawn in next area
            GameObject nextArea = Instantiate(walkable.GetNextAreaPrefab(), walkable.GetNextAreaSpawnPosition(),
                walkable.GetNextAreaSpawnRotation());
            
            FloorInfo nextFloorInfo;
            
            if (walkable is DoorMainWalkable mainWalkable)
            {
                MapData mapData = nextArea.GetComponent<MapData>();
                mapData.LoadFloor(mainWalkable.GetDesiredFloor());
                nextFloorInfo = mapData.GetFloorInfo(mainWalkable.GetDesiredFloor());
            }
            else
            {
                nextFloorInfo = nextArea.GetComponent<FloorInfo>();
                
            }
            
            if (nextFloorInfo == null)
            {
                Debug.LogError("Prefab with name: '" + walkable.GetNextAreaPrefab().name + "' does not have a FloorInfo script attached to the root.");
                yield break;
            }
            
            cam.farClipPlane = nextFloorInfo.GetFarClippingPlane();
            cam.nearClipPlane = nextFloorInfo.GetNearClippingPlane();
            
            loader.SetNewLevelObject(nextArea);

            // TODO do a check for being currently in a flashlight area

            yield return null;
            currentCollision.ReloadList();
            currentCollision.CheckForFlashlightZone();
            yield return null;
            
            // Unfade
            for (float i = 1; i > 0; i -= Time.deltaTime * (screenFadeSpeed * 2))
            {
                screenFadeImage.color = new Color(0, 0, 0, i);
                yield return null;
            }
            screenFadeImage.color = new Color(0, 0, 0, 0);
            
            // Allow player to move now
            movement.SetCanMove(true);
        }

        private IEnumerator WalkThroughDoor(DoorWalkable walkable)
        {
            // Open the door, and then force the player to walk through the door
            // There will be no black screens here
            // The visibility issue could be fixed by disabling parts of the map
            // in "zones". Each doored off area is it's own zone, and the
            // only zone that is enabled is the one the player is in

            // Load area on other side of door
            GameObject nextArea;
            try
            {
                nextArea =
                    Instantiate(walkable.GetNextAreaPrefab(), walkable.GetNextAreaSpawnPosition(),
                        walkable.GetNextAreaSpawnRotation());
            }
            catch (NullReferenceException)
            {
                Debug.LogError("Player tried to access a door that doesn't exist.");
                movement.ForceMovable();
                yield break;
            }
            catch (UnassignedReferenceException)
            {
                Debug.LogError("Player tried to access a door that doesn't have a nextAreaPrefab.");
                movement.ForceMovable();
                yield break;
            }
            

            FloorInfo nextFloorInfo;
            
            if (walkable is DoorMainWalkable mainWalkable)
            {
                MapData mapData = nextArea.GetComponent<MapData>();
                mapData.LoadFloor(mainWalkable.GetDesiredFloor());
                nextFloorInfo = mapData.GetFloorInfo(mainWalkable.GetDesiredFloor() - 1);
                // Set the map loader's currentMapData
                loader.SetCurrentMapData(mapData);
            }
            else
            {
                nextFloorInfo = nextArea.GetComponent<FloorInfo>();
                loader.SetCurrentMapData(null);
            }
            
            if (nextFloorInfo == null)
            {
                Debug.LogError("Prefab with name: '" + walkable.GetNextAreaPrefab().name + "' does not have a FloorInfo script attached to the root.");
                yield break;
            }
            
            // Set the camera settings
            cam.farClipPlane = nextFloorInfo.GetFarClippingPlane();
            cam.nearClipPlane = nextFloorInfo.GetNearClippingPlane();
            
            if (nextFloorInfo.GetEntranceDoors() != null && nextFloorInfo.GetEntranceDoors().Length > 0)
            {
                foreach (GameObject obj in nextFloorInfo.GetEntranceDoors())
                {
                    obj.SetActive(false);
                }
            }

            sfxController.PlaySound("slidingDoor");
            walkable.getDoorAnimator().Play("DoorOpen", 0, 0.0f);
            yield return new WaitForSeconds(0.1f);

            // Force player to walk through door
            movement.ForceForwardMovement();
            
            while (movement.IsPlayerMoving())
            {
                yield return null;
            }
            
            // Unload current map
            loader.SetNewLevelObject(nextArea);
            
            if (nextFloorInfo.GetEntranceDoors() != null && nextFloorInfo.GetEntranceDoors().Length > 0)
            {
                foreach (GameObject obj in nextFloorInfo.GetEntranceDoors()) 
                { 
                    obj.SetActive(true);
                }
            }
        }

        private IEnumerator OpenRoom(MapDoor door)
        {
            // Get the dialogue loaded and music fading prematurely
            if (door is DoorRoom preRoom)
            {
                if (preRoom.GetRoomData() == null)
                {
                    Debug.LogError("Room settings are not set up.");
                    movement.ForceMovable();
                    yield break;
                }
                
                if (preRoom.GetRoomData().roomPrefab == null)
                {
                    Debug.LogError("Room prefab was null.");
                    movement.ForceMovable();
                    yield break;
                }
                // Note: The music is done in dialogue controller while loading dialogues
                PossibleDialogue[] dialogues = preRoom.GetDialogues();
                Vector3 spritePosition = preRoom.GetRoomData().spritePosition;
                if (dialogue == null)
                {
                    Debug.LogError("DialogueController was null. Is it enabled?");
                    yield break;
                }
                dialogue.DetermineAndLoadDialogue(dialogues, spritePosition);
            }
            
            Transform playerTransform = transform;
            Vector3 previousPosition = playerTransform.position;
            Vector3 previousRotation = playerTransform.rotation.eulerAngles;

            // Move character up closer to door, and move up the interact name box
            
            cameraAnimator.Play("EnterDoor", 0, 0.0f);
            uiManager.HideUI();
            uiManager.SetCanOpenPartyStatus(false);
            uiManager.SetCanOpenPauseMenu(false);
            interactUI.HideBox();
            
            // Do door open animation
            if (door.getDoorAnimator() != null)
            {
                sfxController.PlaySound("slidingDoor");
                door.getDoorAnimator().Play("DoorOpen", 0, 0.0f);
                yield return new WaitForSeconds(0.1f);
            }
            
            // Fade screen here, and move UI off screen
            for (float i = 0; i < 1; i += (screenFadeSpeed * Time.deltaTime))
            {
                screenFadeImage.color = new Color(0, 0, 0, i);
                yield return null;
            }
            screenFadeImage.color = new Color(0, 0, 0, 1);
            
            yield return new WaitForSeconds(doorBlackTime / 2);
            
            flashlight.ToggleFlashlight(false);

            // Reset camera position
            cam.GetComponent<Animator>().Play("Default State");
            
            // Wait a few frames for fun
            yield return null;
            yield return null;
            yield return null;

            // Set current level un-active
            loader.HideLevel();

            if (door is DoorRoom room)
            {
                GameObject roomObject = Instantiate(room.GetRoomData().roomPrefab);
                playerTransform.position = Vector3.zero;
                playerTransform.rotation = Quaternion.identity;
                if(room.GetNormalCameraSettingsOverride() == false)
                    SetRoomCameraSettings(room);

                // Remove the part of the minimap that the room takes place in
                Vector3 doorPosition = previousPosition + (playerTransform.forward * 10);
                //FindObjectOfType<MinimapController>().PlayerMovement(doorPosition);
                
                UltrawideBorders ultrawideBorders = FindObjectOfType<UltrawideBorders>();
                if (room.GetRequireBorders())
                {
                    if (ultrawideBorders != null)
                    {
                        ultrawideBorders.SetBordersVisible(true);
                    }
                    else
                    {
                        Debug.LogError("Ultrawide borders were not found.");
                    }
                }
    
                // Wait a little bit to make sure we're loaded in
                float waitTimeSeconds = 0.5f;
                for (float i = waitTimeSeconds; i > 0; i -= Time.deltaTime)
                {
                    yield return null;
                }
                
                currentCollision.ReloadList();
                
                // Un-fade screen
                for (float i = 1; i > 0; i -= (screenFadeSpeed * Time.deltaTime))
                {
                    screenFadeImage.color = new Color(0, 0, 0, i);
                    yield return null;
                }

                screenFadeImage.color = new Color(0, 0, 0, 0);

                // Open dialogue
                dialogue.OpenDialogue();
                
                while (dialogue.IsDialogueOpened())
                {
                    yield return null;
                }
                
                // Exit room
                // Fade screen to black
                for (float i = 0; i < 1; i += (screenFadeSpeed * Time.deltaTime))
                {
                    screenFadeImage.color = new Color(0, 0, 0, i);
                    yield return null;
                }

                screenFadeImage.color = new Color(0, 0, 0, 1);
                dialogue.KillActors();
                Destroy(roomObject);

                // Exit room
                loader.ShowLevel();
                
                ResetNormalCameraSettings();
                if (loader.GetCurrentMapData() != null)
                {
                    FloorInfo info = loader.GetCurrentMapData().GetFloorInfo(loader.GetCurrentMapData().GetCurrentMainFloor());
                    cam.farClipPlane = info.GetFarClippingPlane();
                    cam.nearClipPlane = info.GetNearClippingPlane();
                }
                else
                {
                    FloorInfo info = loader.GetCurrentFloorObject().GetComponent<FloorInfo>();
                    if (info != null)
                    {
                        cam.farClipPlane = info.GetFarClippingPlane();
                        cam.nearClipPlane = info.GetNearClippingPlane();
                    }
                }
                
                previousRotation.y -= 180;
                transform.position = previousPosition;
                transform.rotation = Quaternion.Euler(previousRotation);

                if (door.getDoorAnimator() != null)
                {
                    sfxController.PlaySound("slidingDoor");
                    door.getDoorAnimator().Play("DoorClose", 0, 0.0f);
                    yield return new WaitForSeconds(doorBlackTime);
                }

                if (room.GetRequireBorders())
                {
                    if (ultrawideBorders != null)
                    {
                        ultrawideBorders.SetBordersVisible(false);
                    }
                    else
                    {
                        Debug.LogError("Ultrawide borders were not found.");
                    }
                }

                currentCollision.ReloadList();
                
                // Un-fade screen
                for (float i = 1; i > 0; i -= (screenFadeSpeed * Time.deltaTime))
                {
                    screenFadeImage.color = new Color(0, 0, 0, i);
                    yield return null;
                }

                screenFadeImage.color = new Color(0, 0, 0, 0);

                // Re-show UI
                uiManager.ShowUI();
                uiManager.SetCanOpenPartyStatus(true);
                uiManager.SetCanOpenPauseMenu(true);
                movement.ForceMovable();
            }
            else
            {
                Debug.LogError("Tried to enter a room that is not actually a room.");
            }
        }

        private void SetRoomCameraSettings(DoorRoom room)
        {
            cam.fieldOfView = room.GetRoomData().FOV;
            cam.nearClipPlane = room.GetRoomData().nearClipPlane;
            movement.transform.position = room.GetRoomData().playerPosition;
            cam.farClipPlane = room.GetRoomData().farClipPlane;
        }

        private void ResetNormalCameraSettings()
        {
            DefaultCameraSettings cameraSettings = cam.GetComponent<DefaultCameraSettings>();
            cam.fieldOfView = cameraSettings.GetExploreFOV();
            cam.nearClipPlane = cameraSettings.GetExploreNearClippingPlane();
            cam.farClipPlane = cameraSettings.GetExploreFarClippingPlane();
        }
    }
}