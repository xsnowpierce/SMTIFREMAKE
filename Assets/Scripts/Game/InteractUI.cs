using System.Collections;
using Game.Interactable;
using Game.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class InteractUI : MonoBehaviour
    {
        private Animator interactAnimator;
        private bool isOpened;
        [SerializeField] private TMP_Text interactText;
        [SerializeField] private float delayBetweenLetters;
        private Coroutine currentCoroutine;

        private void Awake()
        {
            interactAnimator = GetComponent<Animator>();
        }
        
        public void ShowInteractBox(IInteractable interactable)
        {
            if (interactable is MapDoor door)
            {
                if (door.getInteractableName() == "") return;

                if (isOpened) return;
                
                if (currentCoroutine != null) StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(showBox(door.getInteractableName()));
            }
        }

        private IEnumerator showBox(string interactName)
        {
            isOpened = true;
            //interactText.text = "";
            interactText.text = interactName;
            
            interactAnimator.Play("InteractOpen", 0, 0f);
            
            /*
            yield return new WaitForSeconds(0.1f);

            
            int currentIterator = 0;
            interactText.text = interactName;
            interactText.maxVisibleCharacters = 0;

            while (true)
            {
                interactText.maxVisibleCharacters = currentIterator;
                if (currentIterator >= interactName.Length)
                    break;

                currentIterator++;
                yield return new WaitForSeconds(delayBetweenLetters);
                yield return null;
            }

            interactText.maxVisibleCharacters = interactName.Length;
            */
            yield return null;
        }

        public void HideBox()
        {
            if (!isOpened) return;
            
            if(currentCoroutine != null) StopCoroutine(currentCoroutine);

            StartCoroutine(HideBoxIe());
        }

        private IEnumerator HideBoxIe()
        {
            
            interactAnimator.Play("InteractClose", 0, 0f);

            yield return new WaitForSeconds(0.1f);
            
            isOpened = false;
            interactText.text = "";
        }
    }
}