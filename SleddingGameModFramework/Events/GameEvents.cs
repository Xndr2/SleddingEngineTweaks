using SleddingGameModFramework.Interfaces;

namespace SleddingGameModFramework.Events
{
    public class PlayerSpawnEvent : IGameEvent
    {
        public string EventName => "PlayerSpawn";
        public bool IsCancellable => false;
        public bool IsCancelled { get; set; }
    }

    public class PlayerDeathEvent : IGameEvent
    {
        public string EventName => "PlayerDeath";
        public bool IsCancellable => true;
        public bool IsCancelled { get; set; }
    }

    public class GameStartEvent : IGameEvent
    {
        public string EventName => "GameStart";
        public bool IsCancellable => false;
        public bool IsCancelled { get; set; }
    }

    public class GameEndEvent : IGameEvent
    {
        public string EventName => "GameEnd";
        public bool IsCancellable => false;
        public bool IsCancelled { get; set; }
    }
} 