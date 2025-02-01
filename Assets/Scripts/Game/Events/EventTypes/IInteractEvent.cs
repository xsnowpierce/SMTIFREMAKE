using Game.Interactable;
using UnityEngine;

namespace Game.Events.EventTypes {

    [CreateAssetMenu(fileName = "New Interact Event", menuName = "Events/Interact Event")]
    public class IInteractEvent : BaseGameEvent<IInteractable> { }
}