using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Battle
{
    public class BattleMoveUI : MonoBehaviour
    {
        [SerializeField] private float stayTime;
        
        [Header("Objects")]
        [SerializeField] private Image teamBackgroundImage;
        [SerializeField] private GameObject disableObject;
        [SerializeField] private TMP_Text moveText;
        [SerializeField] private Animator animator;
        [SerializeField] private float animationLength;
        [Header("Colors")] 
        [SerializeField] private Color playerColour;
        [SerializeField] private Color enemyColour;

        public IEnumerator StartMove(string moveName, bool isPlayer)
        {
            disableObject.SetActive(true);
            teamBackgroundImage.color = isPlayer ? playerColour : enemyColour;

            moveText.text = moveName;

            animator.Play("MoveIn");
            yield return null;

            yield return new WaitForSeconds(animationLength);

            yield return new WaitForSeconds(stayTime);
        }

        public IEnumerator EndMoveAnimation()
        {
            animator.Play("MoveOut");
            yield return null;
            yield return new WaitForSeconds(animationLength);
            disableObject.SetActive(false);
        }
    }
}