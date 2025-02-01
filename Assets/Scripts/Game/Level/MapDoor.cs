using Game.Interactable;
using UnityEngine;

namespace Game.Level
{
    
    public abstract class MapDoor : MapCollider, IInteractable
    {
        [SerializeField] protected string destinationName;

        public void Interact() { }

        public bool isInteractable()
        {
            return true;
        }

        public string getInteractableName()
        {
            return destinationName;
        }
        
        public Animator getDoorAnimator() => GetComponentInChildren<Animator>();
    }
}