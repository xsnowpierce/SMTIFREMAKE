using UnityEngine;

namespace Game.Level
{
    [CreateAssetMenu(fileName = "New Map Background", menuName = "Map/Background Style")]
    public class MapBackgroundStyle : ScriptableObject
    {
        [SerializeField] private GameObject floor;
        [SerializeField] private GameObject roof;
        [SerializeField] private GameObject wall01, wall02, wall03;
        [SerializeField] private GameObject door;

        public GameObject GetFloor() => floor;
        public GameObject GetRoof() => roof;
        public GameObject GetWall01() => wall01;
        public GameObject GetWall02() => wall02;
        public GameObject GetWall03() => wall03;
        public GameObject GetDoor() => door;
    }
}