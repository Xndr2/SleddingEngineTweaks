using System;
using System.Collections.Generic;
using System.Linq;
using SleddingGameModFramework.Interfaces;

namespace SleddingGameModFramework.Core
{
    public class EventManager
    {
        private readonly Dictionary<string, List<IEventHandler>> _eventHandlers = new Dictionary<string, List<IEventHandler>>();

        public void RegisterHandler(string eventName, IEventHandler handler)
        {
            if (!_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName] = new List<IEventHandler>();
            }

            _eventHandlers[eventName].Add(handler);
            // Sort handlers by priority
            _eventHandlers[eventName] = _eventHandlers[eventName].OrderBy(h => h.Priority).ToList();
        }

        public void UnregisterHandler(string eventName, IEventHandler handler)
        {
            if (_eventHandlers.TryGetValue(eventName, out var handlers))
            {
                handlers.Remove(handler);
            }
        }

        public void RaiseEvent(IGameEvent gameEvent)
        {
            if (!_eventHandlers.TryGetValue(gameEvent.EventName, out var handlers))
            {
                return;
            }

            foreach (var handler in handlers)
            {
                try
                {
                    handler.OnEvent(gameEvent);
                    if (gameEvent.IsCancellable && gameEvent.IsCancelled)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError($"Error in event handler for {gameEvent.EventName}: {ex}");
                }
            }
        }

        public void ClearHandlers()
        {
            _eventHandlers.Clear();
        }
    }
} 