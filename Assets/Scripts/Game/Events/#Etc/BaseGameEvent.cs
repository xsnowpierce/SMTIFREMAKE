using UnityEngine;
using System.Collections.Generic;

namespace Game.Events
{
    public abstract class BaseGameEvent<T> : ScriptableObject
    {
        private readonly List<IGameEventListener<T>> listeners = new List<IGameEventListener<T>>();

        public void Raise(T item)
        {
            if (listeners.Count == 0) return;
            
            for(int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(item);
            }
        }

        public void RegisterListener(IGameEventListener<T> listener){
            if(!listeners.Contains(listener)){
                listeners.Add(listener);
            }
        }
        
        public void UnregisterListener(IGameEventListener<T> listener){
            if(listeners.Contains(listener)){
                listeners.Remove(listener);
            }
        }
    }
}