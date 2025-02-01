using System;
using System.Collections;
using Data;
using Database;
using Entity;
using Game.UI;
using TMPro;
using UnityEngine;

namespace Game.Battle.UI
{
    public class BattleChangePartyUI : MonoBehaviour
    {
        
        [Header("Objects")]
        [SerializeField] private GameObject partyStatusInstantiate;
        [SerializeField] private Transform stockPartyParent;

        [Header("Others")] 
        [SerializeField] private TMP_Text stockAmount;
        [SerializeField] private PartyStatus[] activeObjects;
        [SerializeField] private GameObject disableObject;

        private bool isOpened;
        private Vector2 currentSelection = Vector2.zero;
        private readonly int ROW_LENGTH = 3;
        
        public void SetupUI(SaveData saveData)
        {
            Databases databases = FindObjectOfType<Databases>();
            for (int i = 0; i < saveData.partySlots.Length; i++)
            {
                Entity.Entity entity = saveData.GetPartyMembers()[i];
                if(entity == null) activeObjects[i].SetupEmpty();
                else activeObjects[i].SetupMember(entity, databases);
            }

            stockAmount.text = saveData.GetDemonTotal() + "/" + saveData.maxDemons;

            for (int i = 0; i < saveData.entityStock.Length; i++)
            {
                Entity.Entity entity = saveData.entityStock[i];
                if(entity is Human) continue;
                GameObject newObject = Instantiate(partyStatusInstantiate, stockPartyParent, true);
                RectTransform rect = newObject.GetComponent<RectTransform>();
                PartyStatus partyStatus = newObject.GetComponent<PartyStatus>();
                rect.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                if (entity == null)
                    partyStatus.SetupEmpty();
                else partyStatus.SetupMember(entity, databases);
            }
        }
        
        private int GetPositionFromVector(Vector2 position)
        {
            return (int) position.x * ROW_LENGTH + (int) position.y;
        }

        private Vector2 GetVectorFromPosition(int position)
        {
            return new Vector2(position / ROW_LENGTH, position % ROW_LENGTH);
        }
    }
}