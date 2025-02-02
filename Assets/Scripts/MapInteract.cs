using System;
using System.Collections;
using System.Collections.Generic;
using Game.Input;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.WorldMap
{
    public class MapInteract : MonoBehaviour
    {
        
        [SerializeField] private TMP_Text promptText;
        [SerializeField] private GameObject deactivateObject;
        private WorldMapDialogueController dialogueController;
        [SerializeField] private PlayerInputWrapper playerInput;
        private List<WorldMapInteractable> interactables;
        private bool isPromptOpen;
        private string promptWord;
        private bool isInteracting;

        public bool IsInteracting() => isInteracting;
        
        private void Awake()
        {
            interactables = new List<WorldMapInteractable>();
            dialogueController = FindObjectOfType<WorldMapDialogueController>();
            playerInput.GetInteract().performed += ctx => InteractButton();
        }

        private void InteractButton()
        {
            if (isInteracting) return;
            // Start interaction
            if (interactables.Count > 0)
            {
                isInteracting = true;
                if (interactables[0] is MapNPC mapNpc)
                {
                    // Open chat box
                    StartCoroutine(Talking(mapNpc));
                }
                else if (interactables[0] is WorldMapDoor mapDoor)
                {
                    // Load through door
                    StartCoroutine(DoorLoad());
                }
                else
                {
                    Debug.LogError("Interactable wasn't handled.");
                    return;
                }
            }
        }

        private IEnumerator Talking(MapNPC npc)
        {
            MapNPC.WorldMapDialogue[] mapDialogues = npc.GetMapDialogues();
            StartCoroutine(ClosePrompt());
            dialogueController.LoadMapDialogue(mapDialogues);
            while (dialogueController.IsDialogueOpened())
            {
                yield return null;
            }

            isInteracting = false;
            
            // Set the npc overhead to gray
            npc.SetReadState(true);
        }

        private IEnumerator DoorLoad()
        {
            isInteracting = false;
            yield return null;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Interactable"))
            {
                interactables.Add(other.GetComponent<WorldMapInteractable>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.CompareTag("Interactable"))
            {
                interactables.Remove(other.GetComponent<WorldMapInteractable>());
            }
        }

        private void Update()
        {
            if (isPromptOpen && interactables.Count == 0 && !isInteracting)
            {
                // Close prompt 
                isPromptOpen = false;
                StartCoroutine(ClosePrompt());
            }
            else if (!isPromptOpen && interactables.Count > 0 && !isInteracting)
            {
                // Open prompt
                isPromptOpen = true;
                StartCoroutine(OpenPrompt());
            }

            if (interactables.Count > 0)
            {
                // Update text on prompt
                if (interactables[0] is MapNPC)
                {
                    promptWord = "Talk";
                }
                else
                {
                    promptWord = "Enter";
                }

                // TODO add translation here
                promptText.text = promptWord;
            }
        }

        private IEnumerator ClosePrompt()
        {
            deactivateObject.SetActive(false);
            yield return null;
        }

        private IEnumerator OpenPrompt()
        {
            deactivateObject.SetActive(true);
            yield return null;
        }
    }
}