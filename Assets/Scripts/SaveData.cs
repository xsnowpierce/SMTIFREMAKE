using System;
using System.Collections.Generic;
using Database;
using Entity;
using Game.Level;
using Game.Minimap;
using Items;
using UnityEngine;

namespace Data
{
    public enum PlayerType
    {
        Power,
        Speed,
        Balance,
        Lucky
    };

    public enum PlayerGender
    {
        Male,
        Female,
        Both
    };

    [Serializable]
    public struct InventoryItem
    {
        public Item item;
        public int amount;
    }
    
    [Serializable]
    public class SaveData : MonoBehaviour
    {
        [Header("Player Data")] 
        public string playerName;
        public PlayerType playerType;
        public PlayerGender playerGender;
        public int novaPoints;
        public int[] partySlots;
        public List<InventoryItem> inventory;
        
        [Header("Demon Settings")] 
        public int maxDemons;
        public Entity.Entity[] entityStock;
        
        [Header("Progression")]
        public int magnetite;
        public int macca;
        public bool hasComp;
        [Space(10)] 
        public List<string> playerFlags;
        public int currentFloor;
        public Vector3 playerPosition;
        public Vector3 playerRotation;
        public LevelInfo currentLevel;
        public int moonPhase;
        public List<MinimapController.MinimapData> minimapDatas;
        static SaveData instance;
        private Databases databases;
        
        private void Awake()
        {
            //Singleton method
            if (instance == null)
            {
                //First run, set the instance
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                //Instance is not the same as the one we have, destroy old one, and reset to newest one
                Destroy(instance.gameObject);
                instance = this;
                DontDestroyOnLoad(gameObject);
            }

            databases = FindObjectOfType<Databases>();
            inventory = new List<InventoryItem>();
            minimapDatas = new List<MinimapController.MinimapData>();
            playerFlags = new List<string>();
            
            if(partySlots == null || partySlots.Length < 6)
                partySlots = new[]
                {
                    -1, -1, -1, -1, -1, -1
                };
            if (entityStock == null || entityStock.Length < 6)
                entityStock = new Entity.Entity[maxDemons];
            else LoadPartyMemebers(entityStock);
        }
        
        public void LoadSave(SaveData data)
        {
            playerName = data.playerName;
            playerType = data.playerType;
            playerGender = data.playerGender;
            novaPoints = data.novaPoints;
            inventory = data.inventory;
            maxDemons = data.maxDemons;
            entityStock = data.entityStock;
            magnetite = data.magnetite;
            macca = data.macca;
            hasComp = data.hasComp;
            playerFlags = data.playerFlags;
            currentFloor = data.currentFloor;
            playerPosition = data.playerPosition;
            playerRotation = data.playerRotation;
            currentLevel = data.currentLevel;
            moonPhase = data.moonPhase;
            minimapDatas = data.minimapDatas;
            partySlots = data.partySlots;
        }
        public void LoadPartyMemebers(Entity.Entity[] loadSlots)
        {
            for (int i = 0; i < loadSlots.Length; i++)
            {
                Entity.Entity entity = loadSlots[i];
                if (entity == null) continue;
                Entity.Entity character = Instantiate(entity);
                entityStock[i] = character;
            }
        }
        public Protagonist GetPlayerCharacter()
        {
            foreach (Entity.Entity entity in entityStock)
            {
                if (entity.GetType() == typeof(Protagonist))
                {
                    return entity as Protagonist;
                }
            }

            return null;
        }
        
        public void AddPartyMember(int memberID, out string completionCode)
        {
            // TODO make a case for the situation where the player's party is full
            
            // Get character by ID
            Entity.Entity partyMember = databases.GetEntity(memberID);

            // Find first empty slot to add character to
            for (int i = 0; i < entityStock.Length; i++)
            {
                if (entityStock[i] == null)
                {
                    entityStock[i] = partyMember;
                    completionCode = "successful";
                    return;
                }
            }
            completionCode = "party_full";
        }

        public Entity.Entity GetEntityFromStockID(int id)
        {
            return entityStock[id];
        }

        public Entity.Entity[] GetPartyMembers()
        {
            Entity.Entity[] partyList = new Entity.Entity[6];
            for (int i = 0; i < partySlots.Length; i++)
            {
                if(partySlots[i] == -1)
                    continue;
                partyList[i] = entityStock[partySlots[i]];
            }

            return partyList;
        }

        public int GetDemonTotal()
        {
            int demons = 0;
            for (int i = 0; i < entityStock.Length; i++)
            {
                if (entityStock[i] != null)
                    if(entityStock[i] is not Human)
                        demons++;
            }
            return demons;
        }
    }
}