using System.Linq;
using Data;
using Game.Minimap;
using Game.Movement;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Level
{
    public class MapLoader : MonoBehaviour
    {
        [SerializeField] private bool dontDestroy = true;
        [SerializeField] private bool loaded;
        private GameObject currentMapObject;
        private MapData currentMapData;
        private int currentFloorNumber = 1;
        private string currentAreaName = "Void";
        static MapLoader instance;
        
        private void Awake()
        {
            if (dontDestroy)
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
            }
        }
        
        public void LoadLevel(GameObject mapObject, int floorNum)
        {
            MapData mapData = mapObject.GetComponent<MapData>();

            if (floorNum < 1)
            {
                Debug.LogError("Floor number cannot be less than 1.");
                return;
            }
            
            if (mapData == null)
            {
                Debug.LogError("Tried to load a map that doesn't have a MapData attached to it.");
                return;
            }
            
            if (mapData.GetMapInfo().floors.Length < floorNum)
            {
                Debug.LogError("Tried to access a floor higher than possible.");
                return;
            }
            
            // TODO add check to see if this is the same map+floor we are on right now
            
            if (currentMapObject != null)
            {
                DestroyLevel();
            }
            
            // Everything is fine, load level
            
            // Load map if it's a different one
            if (mapData != currentMapData)
            { 
                currentMapObject = Instantiate(mapObject, Vector3.zero, Quaternion.identity);
                currentMapData = currentMapObject.GetComponent<MapData>();
            }
            else
            {
                // Check to see if we need to unload floors to get to the new one
                if (currentMapData.GetCurrentMainFloor() != floorNum)
                {
                    // We must unload!
                    currentMapData.UnloadAllFloors();
                }
            }
            
            // Then load the proper floor
            currentMapData.LoadFloor(floorNum);
            currentMapData.SetMainFloor(floorNum);
            currentFloorNumber = floorNum;
            currentAreaName = currentMapData.GetMapInfo().mapName;
            loaded = true;
        }

        public void HideLevel()
        {
            currentMapObject.SetActive(false);
        }

        public void DestroyLevel()
        {
            Destroy(currentMapObject);
        }

        public void SetNewLevelObject(GameObject floorObject)
        {
            DestroyLevel();
            currentMapObject = floorObject;
        }

        public void ShowLevel()
        {
            currentMapObject.SetActive(true);
        }

        public bool isLoaded() => loaded;

        public GameObject GetCurrentFloorObject() => currentMapObject;

        public MapData GetCurrentMapData() => currentMapData;

        public void SetCurrentMapData(MapData mapData)
        {
            currentMapData = mapData;
        }

        public string GetCurrentMapName() => currentAreaName;
        public int GetCurrentFloorNum() => currentFloorNumber;

    }
}