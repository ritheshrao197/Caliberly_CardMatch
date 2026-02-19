using System;
using System.Collections.Generic;
namespace MemoryGame.Events
{
    using System;
    using System.Collections.Generic;

    public sealed class EventBus
    {
        private static EventBus _instance;
        public static EventBus Instance => _instance ??= new EventBus();

        private readonly Dictionary<Type, List<Delegate>> _subscribers =
            new Dictionary<Type, List<Delegate>>(32);

        private EventBus() { }

        public void Subscribe<T>(Action<T> listener)
        {
            var type = typeof(T);

            if (!_subscribers.TryGetValue(type, out var listeners))
            {
                listeners = new List<Delegate>(4);
                _subscribers[type] = listeners;
            }

            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void Unsubscribe<T>(Action<T> listener)
        {
            var type = typeof(T);

            if (!_subscribers.TryGetValue(type, out var listeners))
                return;

            listeners.Remove(listener);

            if (listeners.Count == 0)
                _subscribers.Remove(type);
        }

        public void Publish<T>(T evt)
        {
            var type = typeof(T);

            if (!_subscribers.TryGetValue(type, out var listeners))
                return;

            for (int i = 0; i < listeners.Count; i++)
            {
                ((Action<T>)listeners[i]).Invoke(evt);
            }
        }

        public void Clear()
        {
            _subscribers.Clear();
        }
    }
}