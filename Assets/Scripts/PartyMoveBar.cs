using System.Collections;
using Battle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Battle.UI
{
    public class PartyMoveBar : MonoBehaviour
    {
        [SerializeField] private GameObject moveBar;
        [SerializeField] private Image moveIcon;
        [SerializeField] private TMP_Text moveText;

        private void Awake()
        {
            moveBar.SetActive(false);
        }
        
        private void SetAction(Sprite icon, string moveName)
        {
            moveIcon.sprite = icon;
            moveText.text = moveName;
        }

        public void ShowMoveAction(Sprite icon, string moveName)
        {
            SetAction(icon, moveName);
            StartCoroutine(showAction());
        }

        public void HideMoveAction()
        {
            StartCoroutine(hideAction());
        }

        private IEnumerator showAction()
        {
            // TODO add animation here
            moveBar.SetActive(true);
            yield return null;
        }

        private IEnumerator hideAction()
        {
            // TODO add animation here
            moveBar.SetActive(false);
            yield return null;
        }
    }
}