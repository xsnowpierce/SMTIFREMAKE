using Game.Interactable;
using Game.Level;
using UnityEngine;

namespace Game.Collision
{
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class MovementCollision : MonoBehaviour
    {
        public bool hasCollider;
        protected MapCollider collidingWith;
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            
            if (other.gameObject.GetComponent<MapCollider>() != null)
            {
                hasCollider = true;
                collidingWith = other.gameObject.GetComponent<MapCollider>();
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {

            if (other.gameObject.GetComponent<MapCollider>() != null)
            {
                hasCollider = false;
                collidingWith = null;
            }
        }

        public MapCollider getTile()
        {
            return this.collidingWith;
        }

        public bool isInteractable()
        {
            if (getTile() is IInteractable interactable)
            {
                if (!interactable.isInteractable()) return false;
                return true;
            }
            return false;
        }

        public void ResetCollision()
        {
            collidingWith = null;
            hasCollider = false;
        }
    }
    
}