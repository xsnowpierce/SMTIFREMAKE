using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using Game;
using Game.Movement;
using UnityEngine;

public class DialogueTile : EventTile
{
    public PossibleDialogue[] dialogues;
    private InteractUI interactUI;
    private UIManager uiManager;
    private DungeonDialogueController dialogueController;
    private Movement movement;
    [SerializeField] private Vector3 relativeSpritePosition = new Vector3(0, 0, 3.5f);
    private bool isInside;

    private void Awake()
    {
        interactUI = FindObjectOfType<InteractUI>();
        uiManager = FindObjectOfType<UIManager>();
        movement = FindObjectOfType<Movement>();
        dialogueController = FindObjectOfType<DungeonDialogueController>();
    }

    public override void OnTileEntered(Vector3 playerRotation)
    {
        isInside = true;
        CheckForDialogue((int)playerRotation.y);
    }

    public override void OnTileExited()
    {
        isInside = false;
    }

    private void CheckForDialogue(int yvalue)
    {
        foreach (PossibleDialogue dialogue in dialogues)
        {
            foreach (Vector3 requiredRotation in dialogue.requiredRotationsForTile)
            {
                if (!(Math.Abs(requiredRotation.y - yvalue) < 1f)) continue;
                if (dialogue.inkJsonAsset == null) continue;

                // Call event
                StartCoroutine(tileDialogue());
                break;
            }
        }
    }

    public void OnRotation(int yvalue)
    {
        if (!isInside) return;

        CheckForDialogue(yvalue);
    }

    private IEnumerator tileDialogue()
    {
        PlayerRotation rot = movement.GetComponent<PlayerRotation>();
        float currentSpeed = rot.GetWalkingSpeed();
        Vector3 spawnPosition = transform.position + relativeSpritePosition;
        
        dialogueController.DetermineAndLoadDialogue(dialogues, spawnPosition);
        PossibleDialogue? chosen = dialogueController.GetChosenDialogue();
        if (chosen == null)
            yield break;

        // Move out UI
        uiManager.HideUI();
        movement.SetCanMove(false);
        dialogueController.OpenDialogue();

        while (dialogueController.IsDialogueOpened())
        {
            yield return null;
        }
        
        if (chosen.Value.afterwardsRotation != Vector3.zero)
        {
            // Rotate player before ending
            Vector3 startRotation = movement.transform.rotation.eulerAngles;
            Vector3 targetRotation = startRotation + chosen.Value.afterwardsRotation;
            float time = 0;

            while (time < currentSpeed)
            {
                movement.transform.rotation = Quaternion.Euler(Vector3.Lerp(startRotation, targetRotation, time / currentSpeed));
                time += Time.deltaTime;
                yield return null;
            }

            movement.transform.rotation = Quaternion.Euler(targetRotation);

            // Raise the player rotate event
            rot.CallTurnEvent();
        }
        
        dialogueController.KillActors();
        
        // Move back UI
        uiManager.ShowUI();
        movement.SetCanMove(true);
    }
}