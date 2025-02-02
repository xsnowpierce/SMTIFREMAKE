using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Battle
{
    public class BattleResultsStatIncreaseObject : MonoBehaviour
    {
        private int currentStatAmount, defaultAmount;
        [SerializeField] private Image fillImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text currentStatText;
        [SerializeField] private TMP_Text statNameText;
        private bool isChanged = false;
        private bool isSelected;
        [SerializeField] private Color selectedBackgroundColour, unselectedBackgroundColour;
        [SerializeField] private Color unchangedColour, changedColour;

        public void SetStatAmount(int amount)
        {
            currentStatAmount = amount;
            defaultAmount = amount;
        }

        public void SetSelected(bool value)
        {
            if (value)
            {
                backgroundImage.color = selectedBackgroundColour;
            }
            else
            {
                backgroundImage.color = unselectedBackgroundColour;
            }
        }
        
        public void ChangeAmount(int amount)
        {
            if (currentStatAmount + amount == defaultAmount)
            {
                // Change back to default colours
                currentStatText.color = unchangedColour;
                statNameText.color = unchangedColour;
                fillImage.color = unchangedColour;
            }
            else
            {
                // Make altered colour
                currentStatText.color = changedColour;
                statNameText.color = changedColour;
                fillImage.color = changedColour;
            }

            currentStatAmount += amount;
            currentStatText.text = "" + currentStatAmount;
        }

        public int GetPointsAdded()
        {
            return currentStatAmount - defaultAmount;
        }
        
    }
}