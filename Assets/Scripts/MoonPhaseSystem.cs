using System;
using Game;
using Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoonPhaseSystem : MonoBehaviour
{
    private UIManager uiManager;
    [SerializeField] private bool enableProgression = false;
    [SerializeField] private int stepsToProgress = 16;
    private int currentMoonPhase = 0;
    private int currentStepsLeft;
    private static readonly int MAX_MOON_PHASES = 15;
    [SerializeField] private MoonSprites moonSprites;
    [SerializeField] private Image moonImage;
    [SerializeField] private TMP_Text moonPhaseText;
    [SerializeField] private float animationSpeed;
    private Sprite[] currentSprites;
    private int currentFrame;

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        currentStepsLeft = stepsToProgress;
        UpdateMoonUI();
        //
        InvokeRepeating(nameof(ChangeFrame), 0, animationSpeed);
    }

    private void ChangeFrame()
    {
        currentFrame++;
        if (currentFrame > currentSprites.Length - 1)
            currentFrame = 0;

        moonImage.sprite = currentSprites[currentFrame];
    }

    public void StepMade()
    {
        if (!enableProgression) return;
            
        currentStepsLeft--;
            
        if (currentStepsLeft == 0)
        {
            if (currentMoonPhase >= MAX_MOON_PHASES)
                currentMoonPhase = 0;
            else currentMoonPhase++;
                
            currentStepsLeft = stepsToProgress;
            
            UpdateMoonUI();
        }
    }

    public int GetMoonPhaseInt() => currentMoonPhase;
        
    public string GetMoonPhaseString()
    {

        int number = currentMoonPhase;
        if (number > 8)
        {
            number = Mathf.Abs(currentMoonPhase - 16);
        }
            
        return number switch
        {
            0 => "NEW MOON",
            4 => "HALF MOON",
            8 => "FULL MOON",
            _ => number + "/8 MOON"
        };
    }

    private void UpdateMoonUI()
    {
        currentSprites = moonSprites.GetMoonSprites(currentMoonPhase);
        moonPhaseText.text = GetMoonPhaseString();
    }

    public void SetMoonProgression(bool value) => enableProgression = value;

    public void SetMoonPhase(int value)
    {
        value = Mathf.Clamp(value, 0, MAX_MOON_PHASES);
        currentMoonPhase = value;
        UpdateMoonUI();
    }

}