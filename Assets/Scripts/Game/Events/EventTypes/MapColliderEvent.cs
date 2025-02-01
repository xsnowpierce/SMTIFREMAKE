using Game.Level;
using UnityEngine;

namespace Game.Events.EventTypes {
    
    [CreateAssetMenu(fileName = "New MapCollider Event", menuName = "Events/MapCollider Event")]
    public class MapColliderEvent : BaseGameEvent<MapCollider> {
        
    }
}