using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Audio;
using Game.Input;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueUIController : MonoBehaviour
    {

        private bool inputAllowed, interactPressed, dialogueOpened, isTyping, skipAnimation, fastForwarding;
        [SerializeField] private PlayerInputWrapper input;
        [SerializeField] private float dialogueTypingTimer = 0.05f;
        [Header("Objects")] 
        [SerializeField] private TMP_Text characterText;
        [SerializeField] private GameObject characterBox;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private SFXController soundController;
        
        [Header("Dialogue Settings")]
        [SerializeField] private RectTransform nextButton;
        [SerializeField] private Vector2 nextButtonOffset;
        [SerializeField] private Transform mwToggler;
        [SerializeField] private Animator dialogueAnimator;
        [SerializeField] private float animationWaitTimer = 0.25f;
        
        [Header("Dialogue Response")] 
        [SerializeField] private Transform dialogueResponseParent;
        [SerializeField] private GameObject dialogueResponsePrefab;
        [SerializeField] private Vector2 responseFirstPosition;
        [SerializeField] private float responseHeightDifference;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color deSelectedColor;
        
        private RectTransform[] responseTransforms;
        private Image[] responseImages;
        private int currentSelected;
        private string characterName, message;
        
        private void Awake()
        {
            if(input == null)
                input = FindObjectOfType<PlayerInputWrapper>();
            
            nextButton.gameObject.SetActive(false);
            soundController = FindObjectOfType<SFXController>();
            dialogueText.text = "";
            characterText.text = "";
            HideBox();
            input.GetMouse().performed += ctx => OnMouse(ctx.ReadValue<Vector2>());
            input.GetMovementAction().started += ctx => OnMovement(ctx.ReadValue<Vector2>());
            input.GetInteract().started += _ => OnInteract();
        }
        
        private void UpdateMw()
        {
            if (!dialogueOpened) return;
            float button = input.GetMwToggler().ReadValue<float>();
            switch (button)
            {
                case > 0:
                    mwToggler.gameObject.SetActive(false);
                    break;
                case 0 when mwToggler.gameObject.activeSelf == false:
                    mwToggler.gameObject.SetActive(true);
                    break;
            }
        }
        private void UpdateFastForward()
        {
            if (!dialogueOpened)
            {
                fastForwarding = false;
                return;
            }
            
            float button = input.GetFastForwardToggler().ReadValue<float>();
            fastForwarding = button > 0;
        }
        void OnMouse(Vector2 mousePosition)
        {
            if (inputAllowed == false) return;
            if (responseTransforms is not {Length: > 0}) return;
            
            for (int i = 0; i < responseTransforms.Length; i++)
            {
                RectTransform rectTransform = responseTransforms[i];
                if (MouseAPI.isMouseInBounds(mousePosition, rectTransform))
                {
                    // Check if hovering over new one
                    if (currentSelected == i)
                        continue;
                    // Change selection 
                    currentSelected = i;

                    // TODO Update somehow?
                }
            }
        }
        void OnMovement(Vector2 movement)
        {
            if (!inputAllowed || interactPressed) return;
            if (responseTransforms == null || responseTransforms.Length == 0)
                return;
            if (movement.y != 0)
            {
                currentSelected += Mathf.CeilToInt(movement.y);
                // Change selection
                if (currentSelected > responseTransforms.Length - 1)
                    currentSelected = 0;
                else if (currentSelected < 0)
                    currentSelected = responseTransforms.Length - 1;

                // Update transform images to show which one is selected
                foreach (Image image in responseImages)
                {
                    image.color = deSelectedColor;
                }

                responseImages[currentSelected].color = selectedColor;
            }
        }
        void OnInteract()
        {
            if (!inputAllowed) return;
            if (isTyping) 
            {
                skipAnimation = true;
                return;
            }
            
            interactPressed = true;
            nextButton.gameObject.SetActive(false);
            // TODO add confirm sound here
            soundController.PlaySound("ui_select");
        }
        public void SetDialogueText(string characterName, string message)
        {
            this.characterName = characterName;
            this.message = message;
        }
        public IEnumerator ShowDialogueBox(bool hasAnswers)
        {
            if (dialogueOpened)
            {
                // Just change text to new text
                yield return StartCoroutine(ReadText(hasAnswers));
            }
            else
            {
                // Do dialogue box opening animation, then show new text
                characterBox.SetActive(characterName != "system");
                yield return StartCoroutine(OpenBox(hasAnswers));
            }

            yield return null;
        }

        private IEnumerator ReadText(bool hasAnswers)
        {
            // Open new box if the name is different
            if (characterText.text != characterName)
            {
                yield return StartCoroutine(ChangeSpeaker());
            }
            
            // Start typing out the text
            dialogueText.text = message;
            isTyping = true;
            dialogueOpened = true;
            skipAnimation = false;
            inputAllowed = true;
            int currentIterator = 0;
            dialogueText.maxVisibleCharacters = 0;
            while (!skipAnimation && isTyping && !fastForwarding)
            {
                // Print a new character
                // TODO Possibly add typewriter sound or something here

                dialogueText.maxVisibleCharacters = currentIterator;
                if (currentIterator >= message.Length)
                    isTyping = false;

                currentIterator++;
                yield return new WaitForSeconds(dialogueTypingTimer);
                yield return null;
            }

            dialogueText.maxVisibleCharacters = message.Length;
            yield return null;
            TextAnimationFinished(hasAnswers);
            skipAnimation = false;
            isTyping = false;
        }

        private void TextAnimationFinished(bool hasAnswers)
        {
            if (hasAnswers) return;
            try
            {
                TMP_CharacterInfo info = dialogueText.textInfo.characterInfo[dialogueText.textInfo.characterCount - 1];
                float xPosition = info.bottomRight.x + nextButtonOffset.x;
                float yPosition = info.descender + ((info.ascender - info.descender) / 2) + nextButtonOffset.y;
                Vector2 middlePoint = new Vector2(xPosition, yPosition);
                nextButton.anchoredPosition = middlePoint;
                nextButton.gameObject.SetActive(true);
            }
            catch (IndexOutOfRangeException)
            {
                Debug.LogError("Tried to access text at value " + (dialogueText.textInfo.characterCount - 1) + ", but it is only a length of " + dialogueText.textInfo.characterCount + ".");
            }
            
        }
        
        private IEnumerator OpenBox(bool hasAnswers)
        {
            characterText.text = characterName;
            
            if (!string.IsNullOrEmpty(characterName) && characterName != "system")
                characterBox.SetActive(true);
            else 
                characterBox.SetActive(false);
            
            dialogueBox.SetActive(true);
            
            yield return StartCoroutine(OpenBoxAnimation());
            
            yield return StartCoroutine(ReadText(hasAnswers));
        }

        private void HideBox()
        {
            dialogueBox.SetActive(false);
            characterBox.SetActive(false);
        }
        
        public IEnumerator CloseBox()
        {
            yield return StartCoroutine(CloseBoxAnimation());
            dialogueBox.SetActive(false);
            characterBox.SetActive(false);
            CloseResponses();
            inputAllowed = false;
            dialogueOpened = false;
            yield return null;
            characterName = "";
            message = "";
            dialogueText.text = "";
            characterText.text = "";
        }

        // TODO Create opening and closing box animations
        private IEnumerator OpenBoxAnimation()
        {
            dialogueAnimator.Play("Open", 0, 0f);
            yield return new WaitForSeconds(animationWaitTimer);
            yield return null;
        }
        
        private IEnumerator CloseBoxAnimation()
        {
            dialogueAnimator.Play("Close", 0, 0.0f);
            yield return new WaitForSeconds(animationWaitTimer);
            yield return null;
        }

        public IEnumerator OpenResponses(string[] choices)
        {
            currentSelected = 0;
            
            Vector2 spawnPosition = responseFirstPosition;
            responseTransforms = new RectTransform[choices.Length];
            responseImages = new Image[choices.Length];
            for (int i = choices.Length - 1; i >= 0; i--)
            {
                string choice = choices[i];
                GameObject obj = Instantiate(dialogueResponsePrefab, Vector3.zero, Quaternion.identity,
                    dialogueResponseParent);
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.localRotation = Quaternion.identity;
                rectTransform.anchoredPosition3D = Vector3.zero;
                rectTransform.anchoredPosition = spawnPosition;
                obj.GetComponentInChildren<TMP_Text>().text = choice;
                responseImages[i] = obj.GetComponentInChildren<Image>();
                responseTransforms[i] = rectTransform;
                spawnPosition.y += responseHeightDifference;
            }
            
            // Update transform images to show which one is selected
            foreach (Image image in responseImages)
            {
                image.color = deSelectedColor;
            }

            responseImages[currentSelected].color = selectedColor;

            yield return null;
        }

        public void CloseResponses()
        {
            if (responseTransforms is {Length: > 0})
            {
                foreach (RectTransform rectTransform in responseTransforms)
                {
                    Destroy(rectTransform.gameObject);
                }
            }
            
            responseTransforms = null;
            responseImages = null;
            interactPressed = false;
        }

        private IEnumerator ChangeSpeaker()
        {
            yield return StartCoroutine(CloseBoxAnimation());
            dialogueText.text = "";
            characterText.text = characterName;
            characterBox.SetActive(characterName != "system");
            yield return StartCoroutine(OpenBoxAnimation());
        }

        public bool IsInteractPressed()
        {
            bool value = interactPressed;
            interactPressed = false;
            return value;
        }

        public int GetCurrentSelected() => currentSelected;
    }
}