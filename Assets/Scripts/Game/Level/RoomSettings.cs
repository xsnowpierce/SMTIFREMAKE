using UnityEngine;

namespace Game.Level
{
    [CreateAssetMenu(fileName = "New Room", menuName = "Map/Room Data")]
    public class RoomSettings : ScriptableObject
    {
        public GameObject roomPrefab;
        public Vector3 spritePosition;
        [Header("Camera Settings")] 
        public Vector3 playerPosition = new Vector3(0, 0.5f, 3);
        public float nearClipPlane = 19.3f;
        public float farClipPlane = 40f;
        public float FOV = 20;
    }
}