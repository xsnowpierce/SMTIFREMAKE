using Game.Level;
using UnityEngine;

namespace Game.Interactable
{
    public abstract class MapInteractable : MapCollider, IInteractable
    {
        [Space(10)]
        private string objectName;
        protected bool canInteract;
        
        protected void Initialize(string ObjectName, bool CanInteract)
        {
            this.objectName = ObjectName;
            this.canInteract = CanInteract;
        }
        
        public virtual void Interact()
        {
            
        }

        public bool isInteractable()
        {
            return canInteract;
        }

        public string getInteractableName()
        {
            return objectName;
        }
    }
}