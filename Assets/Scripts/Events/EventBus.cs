using System;
using System.Collections.Generic;
namespace MemoryGame.Events
{
    /// <summary>
    /// Lightweight in-process event bus for game-wide decoupled messaging.
    /// </summary>
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

            // Snapshot prevents issues if a listener subscribes/unsubscribes during publish.
            var snapshot = listeners.ToArray();
            for (int i = 0; i < snapshot.Length; i++)
            {
                ((Action<T>)snapshot[i]).Invoke(evt);
            }
        }

        public void Clear()
        {
            _subscribers.Clear();
        }
    }
}
