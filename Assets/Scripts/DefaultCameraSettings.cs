using UnityEngine;

public class DefaultCameraSettings : MonoBehaviour
{
    [Header("Exploring")]
    [SerializeField] private float FOV = 60;
    [SerializeField] private float nearClippingPlane = 0.01f;
    [SerializeField] private float farClippingPlane = 40f;
    
    
    public float GetExploreFOV() => FOV;
    public float GetExploreNearClippingPlane() => nearClippingPlane;
    public float GetExploreFarClippingPlane() => farClippingPlane;
}