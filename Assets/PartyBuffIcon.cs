using UnityEngine;
using UnityEngine.UI;

namespace Game.Battle.UI
{
    public class PartyBuffIcon : MonoBehaviour
    {
        [SerializeField] private Image mainIcon;
        [SerializeField] private Image intensityIcon;

        public void SetIcons(Sprite icon, Sprite intensity)
        {
            mainIcon.sprite = icon;
            intensityIcon.sprite = intensity;
        }

        public void SetFading(bool value)
        {
            
        }
    }
}