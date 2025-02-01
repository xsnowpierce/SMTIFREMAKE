using UnityEngine;

namespace Game.WorldMap
{
    public class WorldMapMarker : MonoBehaviour
    {
        [SerializeField] private string markerKey;
        public string GetMarkerKey() => markerKey;
    }
}