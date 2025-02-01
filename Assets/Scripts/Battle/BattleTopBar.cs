using TMPro;
using UnityEngine;

namespace Game.Battle
{
    public class BattleTopBar : MonoBehaviour
    {
        [SerializeField] private GameObject contentObject;
        [SerializeField] private TMP_Text descriptionText;
        
        public void SetBarActive(bool value)
        {
            if (value)
            {
                contentObject.SetActive(true);
            }
            else
            {
                contentObject.SetActive(false);
                descriptionText.text = "";
            }
        }

        public void SetBarText(string text)
        {
            descriptionText.text = text;
        }
    }
}