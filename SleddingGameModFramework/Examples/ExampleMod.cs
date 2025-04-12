using System;
using System.Reflection;
using HarmonyLib;
using SleddingGameModFramework.Configuration;
using SleddingGameModFramework.Templates;
using SleddingGameModFramework.Utils;

namespace SleddingGameModFramework.Examples
{
    /// <summary>
    /// Example mod configuration
    /// </summary>
    public class ExampleModConfig
    {
        public bool EnableFeatureA { get; set; } = true;
        public int FeatureBStrength { get; set; } = 10;
        public string WelcomeMessage { get; set; } = "Hello from Example Mod!";
    }
    
    /// <summary>
    /// Example mod implementation
    /// </summary>
    public class ExampleMod : BaseMod
    {
        public override string Id => "ExampleMod";
        public override string Name => "Example Mod";
        public override string Author => "Your Name";
        public override Version Version => new Version(1, 0, 0);
        
        private ConfigManager<ExampleModConfig> _config;
        private ExampleModConfig _settings;
        
        public override void Initialize()
        {
            Logger.Info($"Initializing {Name} v{Version}");
            
            // Load configuration
            _config = CreateConfig<ExampleModConfig>();
            _config.Load();
            _settings = _config.Config;
            
            Logger.Info(_settings.WelcomeMessage);
            
            // Register event handlers
            RegisterEventHandler("GameLoaded", OnGameFullyLoaded);
            
            // Apply Harmony patches
            ApplyHarmonyPatches();
            
            Logger.Info($"{Name} initialized successfully");
        }
        
        public override void OnGameLoaded()
        {
            Logger.Info("Game has finished loading!");
            
            // This would be where you hook into game systems once they're loaded
            
            if (_settings.EnableFeatureA)
            {
                Logger.Info("Feature A is enabled, activating it");
                // Activate Feature A
            }
        }
        
        public override void OnUpdate(float deltaTime)
        {
            // This runs every frame, be careful with performance!
            
            // Example: Only run logic every 1 second
            _updateAccumulator += deltaTime;
            if (_updateAccumulator >= 1.0f)
            {
                _updateAccumulator = 0;
                SlowUpdate();
            }
        }
        
        private float _updateAccumulator = 0;
        
        private void SlowUpdate()
        {
            // Logic that runs approximately once per second
            // This is much better for performance than doing everything every frame
        }
        
        private void OnGameFullyLoaded(object[] args)
        {
            Logger.Info("Game fully loaded event received!");
            
            // You can use args[0], args[1], etc. if the event passes parameters
        }
        
        public override void Shutdown()
        {
            Logger.Info($"Shutting down {Name}");
            
            // Save config changes
            _config.Save();
            
            // Always call base to ensure patches are unregistered
            base.Shutdown();
        }
        
        /// <summary>
        /// Example of a Harmony patch prefix
        /// </summary>
        [HarmonyPatch(typeof(object), "ToString")]
        [HarmonyPrefix]
        public static bool ExamplePrefix(ref string __result)
        {
            // This is just an example and won't actually be used
            // In a real mod, you'd patch actual game methods
            
            __result = "Patched by Example Mod";
            return false; // Skip original method
        }
    }
}