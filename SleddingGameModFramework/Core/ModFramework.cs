using System;
using System.IO;
using SleddingGameModFramework.Core;
using SleddingGameModFramework.Utils;
using SleddingGameModFramework.Patches;

namespace SleddingGameModFramework
{
    /// <summary>
    /// Main entry point for the modding framework
    /// </summary>
    public class ModFramework
    {
        private static ModFramework _instance;
        
        /// <summary>
        /// Singleton instance of the mod framework
        /// </summary>
        public static ModFramework Instance => _instance ??= new ModFramework();
        
        /// <summary>
        /// The mod manager
        /// </summary>
        public ModManager ModManager { get; private set; }
        
        /// <summary>
        /// The event manager
        /// </summary>
        public EventManager EventManager { get; private set; }
        
        /// <summary>
        /// Whether the framework has been initialized
        /// </summary>
        public bool IsInitialized { get; private set; }
        
        /// <summary>
        /// Base directory for all mod-related data
        /// </summary>
        public string BaseDirectory { get; private set; }
        
        /// <summary>
        /// Directory where mods are stored
        /// </summary>
        public string ModsDirectory { get; private set; }
        
        /// <summary>
        /// The patch manager
        /// </summary>
        public PatchManager PatchManager { get; private set; }
        
        private ModFramework()
        {
            // Private constructor to enforce singleton pattern
        }
        
        /// <summary>
        /// Initializes the mod framework
        /// </summary>
        /// <param name="baseDir">Optional base directory override</param>
        public void Initialize(string baseDir = null)
        {
            if (IsInitialized) return;
            
            try
            {
                // Set up directories
                BaseDirectory = baseDir ?? AppDomain.CurrentDomain.BaseDirectory;
                ModsDirectory = Path.Combine(BaseDirectory, "Mods");
                
                // Initialize logger
                Logger.Initialize();
                Logger.Info("Initializing SleddingGameModFramework");
                
                // Create mod manager and event manager
                ModManager = new ModManager(ModsDirectory);
                EventManager = new EventManager(ModManager);
                PatchManager = new PatchManager();
                
                // Initialize the mod manager
                ModManager.Initialize();
                
                IsInitialized = true;
                Logger.Info("SleddingGameModFramework initialized successfully");
            }
            catch (Exception ex)
            {
                Logger.Critical($"Failed to initialize mod framework: {ex}");
                throw;
            }
        }
        
        /// <summary>
        /// Loads and initializes all discovered mods
        /// </summary>
        public void LoadMods()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("ModFramework must be initialized before loading mods");
            }
            
            try
            {
                Logger.Info("Loading mods");
                ModManager.InitializeMods();
                EventManager.DistributeGameEvents();
                Logger.Info("Finished loading mods");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading mods: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Shuts down the mod framework and all loaded mods
        /// </summary>
        public void Shutdown()
        {
            if (!IsInitialized) return;
            
            try
            {
                Logger.Info("Shutting down mod framework");
                
                // Shut down all mods
                ModManager.ShutdownMods();
                
                // Clear event handlers
                EventManager.ClearAllHandlers();
                
                IsInitialized = false;
                Logger.Info("Mod framework shut down successfully");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error during shutdown: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Simulation method to trigger game events (for testing until game launches)
        /// </summary>
        public void SimulateGameEvent(string eventName, params object[] args)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("ModFramework must be initialized");
            }
            
            EventManager.TriggerEvent(eventName, args);
        }
    }
}