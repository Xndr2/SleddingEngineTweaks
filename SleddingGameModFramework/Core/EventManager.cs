using System;
using System.Collections.Generic;
using SleddingGameModFramework.API;
using SleddingGameModFramework.Utils;

namespace SleddingGameModFramework.Core
{
    /// <summary>
    /// Manages events and communication between mods
    /// </summary>
    public class EventManager
    {
        private readonly Dictionary<string, List<Action<object[]>>> _eventHandlers = new();
        private readonly ModManager _modManager;
        
        /// <summary>
        /// Creates a new event manager
        /// </summary>
        /// <param name="modManager">The mod manager instance</param>
        public EventManager(ModManager modManager)
        {
            _modManager = modManager ?? throw new ArgumentNullException(nameof(modManager));
        }
        
        /// <summary>
        /// Registers a handler for the specified event
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <param name="handler">Handler function</param>
        public void RegisterHandler(string eventName, Action<object[]> handler)
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));
            
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            
            if (!_eventHandlers.TryGetValue(eventName, out var handlers))
            {
                handlers = new List<Action<object[]>>();
                _eventHandlers[eventName] = handlers;
            }
            
            handlers.Add(handler);
            Logger.Debug($"Registered handler for event: {eventName}");
        }
        
        /// <summary>
        /// Unregisters a handler for the specified event
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <param name="handler">Handler function to remove</param>
        /// <returns>True if the handler was removed, false if not found</returns>
        public bool UnregisterHandler(string eventName, Action<object[]> handler)
        {
            if (string.IsNullOrEmpty(eventName) || handler == null)
                return false;
            
            if (_eventHandlers.TryGetValue(eventName, out var handlers))
            {
                bool removed = handlers.Remove(handler);
                
                if (removed)
                    Logger.Debug($"Unregistered handler for event: {eventName}");
                
                // Remove the event entirely if no more handlers
                if (handlers.Count == 0)
                    _eventHandlers.Remove(eventName);
                
                return removed;
            }
            
            return false;
        }
        
        /// <summary>
        /// Triggers an event with the specified arguments
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <param name="args">Event arguments</param>
        public void TriggerEvent(string eventName, params object[] args)
        {
            if (string.IsNullOrEmpty(eventName))
                return;
            
            if (_eventHandlers.TryGetValue(eventName, out var handlers))
            {
                Logger.Debug($"Triggering event: {eventName} with {handlers.Count} handlers");
                
                // Create a copy to prevent issues if handlers are modified during iteration
                var handlersCopy = new List<Action<object[]>>(handlers);
                
                foreach (var handler in handlersCopy)
                {
                    try
                    {
                        handler(args);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Error in event handler for {eventName}: {ex.Message}");
                    }
                }
            }
        }
        
        /// <summary>
        /// Distributes game lifecycle events to mods that implement IModEvents
        /// </summary>
        public void DistributeGameEvents()
        {
            foreach (var modContainer in _modManager.LoadedMods)
            {
                if (modContainer.IsInitialized && modContainer.Mod is IModEvents modEvents)
                {
                    // Register this mod for game events
                    RegisterGameEventListeners(modEvents);
                }
            }
        }
        
        /// <summary>
        /// Registers a mod for game lifecycle events
        /// </summary>
        /// <param name="modEvents">The mod events implementation</param>
        private void RegisterGameEventListeners(IModEvents modEvents)
        {
            // These handlers will forward events to mods
            RegisterHandler("GameStartLoading", _ => modEvents.OnGameStartLoading());
            RegisterHandler("GameLoaded", _ => modEvents.OnGameLoaded());
            RegisterHandler("GameUpdate", args => modEvents.OnUpdate((float)args[0]));
            RegisterHandler("GameUnloading", _ => modEvents.OnGameUnloading());
        }
        
        /// <summary>
        /// Clears all event handlers
        /// </summary>
        public void ClearAllHandlers()
        {
            _eventHandlers.Clear();
            Logger.Debug("Cleared all event handlers");
        }
    }
}