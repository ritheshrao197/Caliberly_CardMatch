namespace MemoryGame.Events
{
    /// <summary>
    /// Interface for event listeners. Implement this to receive events from the EventBus.
    /// </summary>
    /// <typeparam name="T">The event data type to listen for</typeparam>
    public interface IEventListener<T> where T : IGameEvent
    {
        /// <summary>
        /// Called when an event of type T is published
        /// </summary>
        /// <param name="evt">The event data</param>
        void OnEvent(T evt);
    }
}
