using UnityEngine;

namespace Game.Level
{
    public class UnloadZone : EventTile
    {
        [SerializeField] public int unloadFloorNumber;
        [SerializeField] public int keepFloorNumber;
        private MapLoader loader;
        private void Awake()
        {
            loader = FindObjectOfType<MapLoader>();
        }
        
        public override void OnTileEntered(Vector3 playerRotation)
        {
            // Load the next floor
            MapData mapData = loader.GetCurrentFloorObject().GetComponent<MapData>();
            mapData.UnloadFloor(unloadFloorNumber, keepFloorNumber);
        }

        public override void OnTileExited()
        {
            
        }
    }
}