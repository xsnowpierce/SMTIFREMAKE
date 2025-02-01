using Game.Events.Custom_Events;
using Game.Events.EventTypes;

namespace Game.Events.Listeners {
    public class IntEventListener : BaseGameEventListener<int, IntEvent, CustomIntEvent> { }
}
