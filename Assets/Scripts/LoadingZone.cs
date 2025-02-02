using UnityEngine;

namespace Game.Level
{
    public class LoadingZone : EventTile
    {
        [SerializeField] public int loadFloorNumber;
        private MapLoader loader;
        private void Awake()
        {
            loader = FindObjectOfType<MapLoader>();
        }
        
        public override void OnTileEntered(Vector3 playerRotation)
        {
            // Load the next floor
            MapData mapData = loader.GetCurrentFloorObject().GetComponent<MapData>();
            mapData.LoadFloor(loadFloorNumber);
        }

        public override void OnTileExited()
        {
            
        }
    }
}