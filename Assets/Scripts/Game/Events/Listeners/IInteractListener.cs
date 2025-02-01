using Game.Events.Custom_Events;
using Game.Events.EventTypes;
using Game.Interactable;

namespace Game.Events.Listeners {
    public class IInteractListener : BaseGameEventListener<IInteractable, IInteractEvent, CustomIInteractableEvent> { }
}