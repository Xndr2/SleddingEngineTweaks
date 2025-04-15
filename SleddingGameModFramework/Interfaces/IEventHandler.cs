using System;

namespace SleddingGameModFramework.Interfaces
{
    public interface IEventHandler
    {
        /// <summary>
        /// Called when an event is raised
        /// </summary>
        /// <param name="gameEvent">The event that was raised</param>
        void OnEvent(IGameEvent gameEvent);

        /// <summary>
        /// The priority of this event handler (lower numbers are called first)
        /// </summary>
        int Priority { get; }
    }
} 