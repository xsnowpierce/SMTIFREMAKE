using UnityEngine;

namespace Game.Events.EventTypes {

    [CreateAssetMenu(fileName = "New Int Event", menuName = "Events/Int Event")]
    public class IntEvent : BaseGameEvent<int> { }
}