using Game.Interactable;
using UnityEngine;

namespace Game.Collision
{
    public class PlayerInteract : MonoBehaviour
    {
        [SerializeField] private float checkDistance = 6;
        [SerializeField] private InteractUI interactUI;
        
        public void CheckForObject()
        {
            LayerMask mask = LayerMask.GetMask("CollisionObject");
            Debug.DrawLine(transform.position, transform.position + (transform.forward * checkDistance), Color.red, 5);
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, checkDistance, mask))
            {
                // Hit a collider
                IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    // Is able to interact!
                    interactUI.ShowInteractBox(interactable);
                }
            }
        }

        public void ResetInteract()
        {
            interactUI.HideBox();
        }
    }
}