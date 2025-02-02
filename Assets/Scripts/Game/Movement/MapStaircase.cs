using System;
using Game.Level;
using UnityEngine;

namespace Game.Movement
{
    public class MapStaircase : MapCollider
    {
        [SerializeField] private Vector3 endPosition;
        [SerializeField] private Vector3 walkToAfter;
        [SerializeField] private float waitForIncline;
        [SerializeField] private float climbSpeed = .7f;
        [SerializeField] private int loadFloorNumber = -1;
        [SerializeField] private int unloadFloorNumber = -1;
        [SerializeField] private int setMainFloor = -1;

        public Vector3 GetEndPosition() => endPosition;
        public Vector3 GetWalkTo() => walkToAfter;
        public float GetInclineWaitTime() => waitForIncline;
        public int GetLoadFloor() => loadFloorNumber;
        public int GetUnloadFloor() => unloadFloorNumber;
        public int GetSetMainFloor() => setMainFloor;
        public float GetClimbSpeed() => climbSpeed;
    }
}