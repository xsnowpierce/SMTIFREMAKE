using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NovaController : MonoBehaviour
{
    [Header("Images")] 
    [SerializeField] private Image faceImage;
    [SerializeField] private Image leftHand;
    [SerializeField] private Image rightHand;

    [Header("Sprites")]
    [SerializeField] private Sprite normalFace;
    [SerializeField] private Sprite openMouth;
    [SerializeField] private Sprite closedEyes;

    [Header("Values")] 
    [SerializeField] private float talkingSpeed;
    [SerializeField] private float talkingDuration;
    [SerializeField] private float blinkingSpeed;
    [SerializeField] private float blinkingDuration;
        
    private bool isTalking, isBlinking;
    private float spriteTimer, talkingTimer;
        
    private void Update()
    {
        ProcessSpriteChanges();
        ProcessTalking();
    }

    private void ProcessTalking()
    {
        if (!isTalking) return;
            
        if (talkingTimer < talkingDuration)
        {
            talkingTimer += Time.deltaTime;
        }
        else
        {
            isTalking = false;
            talkingTimer = 0f;
        }
    }

    private void ProcessSpriteChanges()
    {
        spriteTimer += Time.deltaTime;
        if (isTalking)
        {
            if (isBlinking)
            {
                faceImage.sprite = normalFace;
                isBlinking = false;
            }
                
            // Move mouth
            if (spriteTimer >= talkingSpeed)
            {
                // Change sprite
                if (faceImage.sprite != openMouth)
                    faceImage.sprite = openMouth;
                else
                    faceImage.sprite = normalFace;

                spriteTimer = 0f;
            }
        }
        else if (!isTalking && !isBlinking)
        {
            if (faceImage.sprite != normalFace)
                faceImage.sprite = normalFace;
        }
        else
        {
            if (isBlinking) return;
                
            // Blink
            if (spriteTimer >= blinkingSpeed)
            {
                // Change sprite
                faceImage.sprite = closedEyes;
                isBlinking = true;
                    
                Invoke(nameof(OpenEyes), blinkingDuration);
            }
        }
    }

    private void OpenEyes()
    {
        if (isTalking) return;
            
        // Change sprite to opened
        faceImage.sprite = normalFace;
            
        isBlinking = false;
        spriteTimer = 0f;
    }
        
    [ContextMenu("Start Talking")]
    public void StartTalking()
    {
        talkingTimer = 0f;
        isTalking = true;
    }
        
}