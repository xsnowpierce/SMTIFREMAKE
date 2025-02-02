using System;
using System.Collections.Generic;
using Game.Minimap;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Level
{
    public class MapData : MonoBehaviour
    {
        [Serializable]
        public struct Floor
        {
            public int floorNumber;
            public Sprite floorMap;
            public GameObject floorObject;
            public FloorInfo floorInfo;
        }

        [Serializable]
        public struct Staircase
        {
            public GameObject staircaseObject;
            public int bottomFloorNumber;
        }

        [Serializable]
        public struct Map
        {
            public Floor[] floors;
            public Staircase[] starcases;
            public string mapID;
            public string mapName;
        }

        [SerializeField] private Map map;
        private List<int> currentLoadedFloors;
        private int currentMainFloor = 1;
        //private MinimapController minimapController;

        private void Awake()
        {
            currentLoadedFloors = new List<int>();
            //minimapController = FindObjectOfType<MinimapController>();
        }

        public void ResetFloors()
        {
            // Disable all maps
            for (int i = 0; i < map.floors.Length; i++)
            {
                map.floors[i] .floorObject.SetActive(false);
            }

            // Disable all staircases
            for (int i = 0; i < map.starcases.Length; i++)
            {
                map.starcases[i].staircaseObject.SetActive(false);
            }
            
        }

        public void LoadFloor(int number)
        {
            // Load staircases for this floor
            for (int i = 0; i < map.starcases.Length; i++)
            {
                if (map.starcases[i].bottomFloorNumber == number 
                    || map.starcases[i].bottomFloorNumber == number - 1) // Check if this staircase is to go down or up
                {
                    // Staircase is relevant to this floor
                    map.starcases[i].staircaseObject.SetActive(true);
                }
            }
            
            // Load main floor
            map.floors[number - 1].floorObject.SetActive(true);
            
            // Add to current loaded floors if not already there
            if(!currentLoadedFloors.Contains(number))
                currentLoadedFloors.Add(number);
        }

        public void UnloadFloor(int unloadNumber, int currentFloorNumber)
        {
            
            // Disable staircases for this floor
            for (int i = 0; i < map.starcases.Length; i++)
            {
                if (map.starcases[i].bottomFloorNumber != currentFloorNumber && map.starcases[i].bottomFloorNumber != currentFloorNumber - 1)
                {
                    // Staircase is not relevant to the current floor
                    map.starcases[i].staircaseObject.SetActive(false);
                }
            }
            
            // Unload main floor    
            try
            {
                map.floors[unloadNumber - 1].floorObject.SetActive(false);
            }
            catch (IndexOutOfRangeException)
            {
                Debug.LogError("Tried to load staircase that was out of bounds. Attempted: " + (unloadNumber - 1));
            }
            
            
            // Remove number from currentLoadedFloors
            currentLoadedFloors.Remove(unloadNumber);
        }

        public void RefreshLoads(int currentFloor)
        {
            UnloadAllFloors();
            LoadFloor(currentFloor);
        }

        public void UnloadAllFloors()
        {
            foreach (Staircase staircase in map.starcases)
            {
                // Staircase is not relevant to this floor
                staircase.staircaseObject.SetActive(false);
            }
            
            foreach (Floor floor in map.floors)
            {
                // Staircase is not relevant to this floor
                floor.floorObject.SetActive(false);
            }
        }

        public void SetMainFloor(int mainFloor)
        {
            currentMainFloor = mainFloor;
            // Update the minimap's floor number
            //minimapController.LoadMap(map.mapID, currentMainFloor, map.floors[currentMainFloor - 1].floorMap);
        }

        public FloorInfo GetFloorInfo(int floorNumber)
        {
            return map.floors[floorNumber].floorInfo;
        }

        public List<int> GetLoadedFloors() => currentLoadedFloors;

        public int GetCurrentMainFloor() => currentMainFloor;

        public Map GetMapInfo() => map;
    }
}