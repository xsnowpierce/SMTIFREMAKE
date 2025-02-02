using System;
using UnityEngine;

namespace Game.Occlusion
{
    [RequireComponent(typeof(BoxCollider))]
    public class OcclusionZone : MonoBehaviour
    {
        private BoxCollider col;
        [SerializeField] private MeshRenderer[] objects;
        private bool isInside;

        private void Awake()
        {
            col = GetComponent<BoxCollider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isInside = true;
                UpdateCulling();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isInside = false;
                UpdateCulling();
            }
        }

        private void UpdateCulling()
        {
            foreach (MeshRenderer rndr in objects)
            {
                rndr.enabled = isInside;
            }
        }
    }
}