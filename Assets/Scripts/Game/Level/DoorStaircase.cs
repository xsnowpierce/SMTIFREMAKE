using UnityEngine;

namespace Game.Level
{
    public enum StaircaseType
    {
        Upstairs,
        Downstairs
    }
    public class DoorStaircase : MapDoor
    {
        [SerializeField] private int destinationFloor;
        [SerializeField] private StaircaseType _type;
        [Header("Next Floor Position")]
        [SerializeField] private Vector3 playerPosition;
        [SerializeField] private Vector3 playerRotation;

        public Vector3 GetNextFloorPosition() => playerPosition;
        public Vector3 GetNextFloorRotation() => playerRotation;
        public StaircaseType getStaircaseType() => _type;
    }
}