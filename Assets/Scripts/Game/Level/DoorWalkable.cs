using UnityEngine;

namespace Game.Level
{
    public class DoorWalkable : MapDoor
    {
        [SerializeField] private GameObject nextAreaPrefab;
        [SerializeField] private Vector3 prefabSpawnPos;
        [SerializeField] private Vector3 prefabSpawnRot;
        [SerializeField] private bool fadeScreenOnLoad;
        
        public GameObject GetNextAreaPrefab() => nextAreaPrefab;
        public Vector3 GetNextAreaSpawnPosition() => prefabSpawnPos;
        public Quaternion GetNextAreaSpawnRotation() => Quaternion.Euler(prefabSpawnRot);

        public bool FadeScreenOnLoad() => fadeScreenOnLoad;
    }
}