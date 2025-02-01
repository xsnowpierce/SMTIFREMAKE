using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Level
{
    [CreateAssetMenu(fileName = "New Level Info", menuName = "Map/Level Info")]
    public class LevelInfo : ScriptableObject
    {
        [SerializeField] private string mapName;
        [SerializeField] private string mapID;
        [SerializeField] private int recommendedLevel;
        
        [SerializeField] private MapBackgroundStyle backgroundStyle;
        [SerializeField] private GameObject[] floors;
        [SerializeField] private GameObject upStairs, downStairs;
        
        public string GetMapName() => mapName;
        public string GetMapID() => mapID;
        public int GetRecommendedLevel() => recommendedLevel;
        public MapBackgroundStyle GetBackgroundStyle() => backgroundStyle;
        public GameObject[] GetFloors() => floors;
        public GameObject GetUpstairsPrefab() => upStairs;
        public GameObject GetDownstairsPrefab() => downStairs;
        
    }
}