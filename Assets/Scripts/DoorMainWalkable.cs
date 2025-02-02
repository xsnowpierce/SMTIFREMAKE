using UnityEngine;

namespace Game.Level
{
    public class DoorMainWalkable : DoorWalkable
    {
        [SerializeField] private int mapFloor;
        public int GetDesiredFloor() => mapFloor;
    }
}