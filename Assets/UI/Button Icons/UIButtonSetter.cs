using Database;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIButtonSetter : MonoBehaviour
    {
        [SerializeField] private UIButtonID button;
        [SerializeField] private Image imageToSet;
        
        private void Awake()
        {
            SetNewButton(FindObjectOfType<Databases>());
        }

        public void SetNewButton(Databases database)
        {
            imageToSet.sprite = database.GetButtonIcon(button);
        }
    }
}