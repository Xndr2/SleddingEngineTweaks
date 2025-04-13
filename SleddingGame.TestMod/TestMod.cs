using System;
using SleddingGameModFramework.Templates;
using SleddingGameModFramework.Utils;

namespace SleddingGame.TestMod
{
    /// <summary>
    /// A simple test mod to verify that the framework works
    /// </summary>
    public class TestMod : BaseMod
    {
        public override string Id => "test_mod";
        
        public override string Name => "Test Mod";
        
        public override string Author => "Xndr";
        
        public override Version Version => new Version(0, 1, 0);
        
        private TestModConfig _config;
        
        public override void Initialize()
        {
            Logger.Info($"Initializing {Name} v{Version}");
            
            // Load config
            _config = CreateConfig<TestModConfig>().Config;
            
            // Register for events
            RegisterEventHandler("GameLoaded", OnGameLoadedEvent);
            
            // Apply Harmony patches
            ApplyHarmonyPatches();
            
            Logger.Info($"{Name} initialized successfully");
        }
        
        private void OnGameLoadedEvent(object[] args)
        {
            Logger.Info("Test mod received GameLoaded event");
            
            if (_config.ShowWelcomeMessage)
            {
                // In a real implementation, this would display a message in-game
                Logger.Info("Welcome to the Test Mod for Sledding Game!");
            }
        }
        
        public override void OnUpdate(float deltaTime)
        {
            // This would run every frame in the actual game
            // For testing, we'll just log occasionally
            if (DateTime.Now.Second % 10 == 0 && DateTime.Now.Millisecond < 100)
            {
                Logger.Debug($"Test mod update: {deltaTime}s");
            }
        }
        
        public override void Shutdown()
        {
            Logger.Info($"Shutting down {Name}");
        }
    }
    
    /// <summary>
    /// Configuration for the test mod
    /// </summary>
    public class TestModConfig
    {
        /// <summary>
        /// Whether to show a welcome message when the game loads
        /// </summary>
        public bool ShowWelcomeMessage { get; set; } = true;
        
        /// <summary>
        /// Example setting for mod features
        /// </summary>
        public float SpeedMultiplier { get; set; } = 1.5f;
        
        /// <summary>
        /// Example keybinding setting
        /// </summary>
        public string ActivationKey { get; set; } = "F7";
    }
}