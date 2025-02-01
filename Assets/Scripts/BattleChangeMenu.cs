using System.Collections;
using Data;
using UnityEngine;

namespace Game.Battle.UI
{
    public class BattleChangeMenu : MonoBehaviour
    {
        [SerializeField] private GameObject disableObject;
        [SerializeField] private BattleChangePartyUI changePartyUI;
        [SerializeField] private BattleChangeSelectedUI selectedUI;

        private SaveData saveData;
        
        private bool isOpened;

        private void Awake()
        {
            saveData = FindObjectOfType<SaveData>();
        }
        
        public IEnumerator OpenMenu(bool canSelectFirstDemon, int forcedFirstSelect)
        {
            /* TODO use canSelectFirstDemon and forcedFirstSelect to prevent demons
            /  from being able to change anyone but themselves
            */
            
            // Open
            isOpened = true;
            changePartyUI.SetupUI(saveData);
            disableObject.SetActive(true);
            
            // Loop
            while (isOpened)
            {
                yield return null;
            }

            // Close
            isOpened = false;
            disableObject.SetActive(false);
        }

        public void Movement(Vector2 movement)
        {
            
        }

        public void Select()
        {
            
        }

        public void Cancel()
        {
            
        }
    }
}