using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SleddingGameModFramework.Core;
using SleddingGameModFramework.UI;
using SleddingGameModFramework.GameState;
using UnityEngine;

namespace SleddingGameModFramework
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        internal static ConfigManager ConfigManager { get; private set; }
        internal static EventManager EventManager { get; private set; }
        internal static UIManager UIManager { get; private set; }
        internal static GameStateManager GameState { get; private set; }
        private Harmony _harmony;
        private ModLoader _modLoader;
        private bool _isPaused;

        private void Awake()
        {
            // Set up logging
            Log = Logger;
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            // Initialize Harmony for patching
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            // Initialize config manager
            string configPath = Path.Combine(Paths.GameRootPath, "Config");
            ConfigManager = new ConfigManager(configPath);

            // Initialize event manager
            EventManager = new EventManager();

            // Initialize UI manager
            UIManager = new UIManager();

            // Initialize game state manager
            GameState = new GameStateManager();

            // Initialize game accessor
            GameAccessor.Initialize();

            // Initialize mod loader
            string modsPath = Path.Combine(Paths.GameRootPath, "Mods");
            _modLoader = new ModLoader(modsPath);
            _modLoader.LoadMods();
        }

        private void Update()
        {
            if (!_isPaused)
            {
                _modLoader.UpdateMods();
                UIManager.UpdateUI();
                GameState.Update();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && !_isPaused)
            {
                _isPaused = true;
                _modLoader.PauseMods();
                UIManager.HideAll();
                GameState.SetGamePaused(true);
            }
            else if (!pauseStatus && _isPaused)
            {
                _isPaused = false;
                _modLoader.ResumeMods();
                UIManager.ShowAll();
                GameState.SetGamePaused(false);
            }
        }

        private void OnDestroy()
        {
            // Save all configurations
            ConfigManager.SaveAllConfigs();

            // Clear all event handlers
            EventManager.ClearHandlers();

            // Clear all UI elements
            UIManager.Clear();

            // Clean up Harmony patches
            _harmony?.UnpatchSelf();
            
            // Unload all mods
            _modLoader.UnloadMods();
        }
    }
} 