using UnityEngine;

namespace Game.Level
{
    public class FloorInfo : MonoBehaviour
    {
        [SerializeField] private string areaName;
        [SerializeField] private GameObject[] entranceDoors;
        [SerializeField] private float nearClippingPlane = 0.01f;
        [SerializeField] private float farClippingPlane = 40f;

        public GameObject[] GetEntranceDoors() => entranceDoors;
        public float GetNearClippingPlane() => nearClippingPlane;
        public float GetFarClippingPlane() => farClippingPlane;
    }
}