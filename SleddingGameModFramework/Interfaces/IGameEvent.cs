namespace SleddingGameModFramework.Interfaces
{
    public interface IGameEvent
    {
        /// <summary>
        /// The name of the event
        /// </summary>
        string EventName { get; }

        /// <summary>
        /// Whether the event can be cancelled
        /// </summary>
        bool IsCancellable { get; }

        /// <summary>
        /// Whether the event has been cancelled
        /// </summary>
        bool IsCancelled { get; set; }
    }
} 