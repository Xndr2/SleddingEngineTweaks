using SleddingGameModFramework.Interfaces;
using SleddingGameModFramework.Events;
using SleddingGameModFramework.UI;
using UnityEngine;

namespace ExampleMod
{
    public class ExampleMod : IMod, IEventHandler
    {
        private readonly ExampleModConfig _config;
        private readonly ExampleModUI _ui;
        private int _playerDeaths;

        public int Priority => 0; // Default priority for event handling

        public ExampleMod()
        {
            _config = new ExampleModConfig();
            _ui = new ExampleModUI();
        }

        public void OnLoad()
        {
            // Register our configuration
            Plugin.ConfigManager.RegisterConfig(_config);

            // Register event handlers
            Plugin.EventManager.RegisterHandler("PlayerDeath", this);
            Plugin.EventManager.RegisterHandler("GameStart", this);
            Plugin.EventManager.RegisterHandler("GameEnd", this);

            // Register our UI
            Plugin.UIManager.RegisterUI(_ui);
            if (_config.ShowUI)
            {
                _ui.Show();
            }
            else
            {
                _ui.Hide();
            }

            Debug.Log("ExampleMod loaded successfully!");
        }

        public void OnUnload()
        {
            // Unregister our UI
            Plugin.UIManager.UnregisterUI(_ui);

            // Unregister event handlers
            Plugin.EventManager.UnregisterHandler("PlayerDeath", this);
            Plugin.EventManager.UnregisterHandler("GameStart", this);
            Plugin.EventManager.UnregisterHandler("GameEnd", this);

            // Unregister our configuration
            Plugin.ConfigManager.UnregisterConfig(_config.ModName);

            Debug.Log("ExampleMod unloaded successfully!");
        }

        public void OnUpdate()
        {
            // UI updates are handled by the UIManager
        }

        public void OnPause()
        {
            _ui.Hide();
        }

        public void OnResume()
        {
            if (_config.ShowUI)
            {
                _ui.Show();
            }
        }

        public void OnEvent(IGameEvent gameEvent)
        {
            switch (gameEvent.EventName)
            {
                case "PlayerDeath":
                    _playerDeaths++;
                    Debug.Log($"Player died! Total deaths: {_playerDeaths}");
                    break;
                case "GameStart":
                    _playerDeaths = 0;
                    Debug.Log("Game started! Resetting death counter.");
                    break;
                case "GameEnd":
                    Debug.Log($"Game ended! Total deaths: {_playerDeaths}");
                    break;
            }
        }
    }
} 