using System.Collections;
using System.Collections.Generic;
using Intro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NovaDialogueUI : MonoBehaviour
{
    private PlayerInput _input;
    [Header("Dialogue")]
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private Text dialogueTextbox;
    [Header("Question Box")]
    [SerializeField] private GameObject questionBoxParent;
    [SerializeField] private Image[] questionBoxes;
    [SerializeField] private Text[] questionTextBoxes;
    [SerializeField] private Text questionTextTitle;
    private bool inQuestionBox;
    private NovaQuestion.NovaQuestionChoice? lastChoice;
    private NovaQuestion currentQuestion;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _input.actions["Mouse"].performed += ctx => OnPoint(ctx.ReadValue<Vector2>());
        _input.actions["Interact"].started += ctx => OnClick();
    }
    
    public void OpenDialogue(string text)
    {
        CloseQuestionBox();
        inQuestionBox = false;
        lastChoice = null;
        dialogueTextbox.text = text;
        dialogueParent.SetActive(true);
    }
    
    public void OpenQuestionBox(NovaQuestion question)
    {
        currentQuestion = question;
        
        foreach (Image image in questionBoxes)
        {
            image.gameObject.SetActive(false);
        }
        
        lastChoice = null;
        questionTextTitle.text = question.englishQuestion;
        questionBoxParent.SetActive(true);
        questionTextTitle.gameObject.SetActive(true);
        for (int i = 0; i < question.choices.Length; i++)
        {
            NovaQuestion.NovaQuestionChoice choice = question.choices[i];
            questionTextBoxes[i].text = choice.englishText;
            questionBoxes[i].gameObject.SetActive(true);
        }
        inQuestionBox = true;
    }

    public NovaQuestion.NovaQuestionChoice? GetQuestionAnswer()
    {
        if (inQuestionBox)
        {
            CloseQuestionBox();
            inQuestionBox = false;
            NovaQuestion.NovaQuestionChoice? choice = lastChoice;
            lastChoice = null;
            return choice;
        }
        return null;
    }

    public void CloseQuestionBox()
    {
        for (int i = 0; i < questionTextBoxes.Length; i++)
        {
            Text txt = questionTextBoxes[i];
            txt.text = "";
            questionBoxes[i].gameObject.SetActive(false);
        }

        questionTextTitle.text = "";
        questionBoxParent.SetActive(false);
        inQuestionBox = false;
    }

    public bool IsQuestionBoxOpen() => inQuestionBox;
    
    public void CloseDialogue()
    {
        inQuestionBox = false;
        dialogueParent.SetActive(false);
        dialogueTextbox.text = "";
    }
    
    void OnPoint(Vector2 mousePos)
    {
        foreach (Image image in questionBoxes)
        {
            if (MouseAPI.isMouseInBounds(mousePos, image.GetComponent<RectTransform>()))
            {
                // Enable 
                image.color = Color.red;
                
                // Disable all others
                foreach (Image image2 in questionBoxes)
                {
                    if (image2 != image)
                    {
                        image2.color = Color.black;
                    }
                }
            }
        }
    }

    void OnClick()
    {
        if (inQuestionBox)
        {
            // Select question!
            for (int i = 0; i < questionBoxes.Length; i++)
            {
                if (questionBoxes[i].color != Color.red) continue;

                lastChoice = currentQuestion.choices[i];
                break;
            }
        }
    }
}
