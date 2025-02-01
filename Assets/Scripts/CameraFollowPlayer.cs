using UnityEngine;

namespace Game.WorldMap 
{
    public class CameraFollowPlayer : MonoBehaviour
    {
        [SerializeField] private GameObject followObject;
        [SerializeField] private float lowX, highX;
        [SerializeField] private float lowZ, highZ;

        private void Update()
        {
            Vector3 playerPos = followObject.transform.position;
            transform.position = new Vector3(
                Mathf.Clamp(playerPos.x, lowX, highX),
                transform.position.y,
                Mathf.Clamp(playerPos.z, lowZ, highZ));
        }
    }
}