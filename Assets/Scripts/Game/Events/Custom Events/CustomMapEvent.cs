using Game.Level;
using UnityEngine.Events;

namespace Game.Events.Custom_Events {
    [System.Serializable] public class CustomMapEvent : UnityEvent<MapCollider> {
        
    }
}