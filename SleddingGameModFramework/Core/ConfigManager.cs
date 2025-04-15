using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SleddingGameModFramework.Interfaces;

namespace SleddingGameModFramework.Core
{
    public class ConfigManager
    {
        private readonly string _configDirectory;
        private readonly Dictionary<string, IModConfig> _configs = new Dictionary<string, IModConfig>();

        public ConfigManager(string configDirectory)
        {
            _configDirectory = configDirectory;
            if (!Directory.Exists(_configDirectory))
            {
                Directory.CreateDirectory(_configDirectory);
            }
        }

        public void RegisterConfig(IModConfig config)
        {
            if (_configs.ContainsKey(config.ModName))
            {
                Plugin.Log.LogWarning($"Configuration for mod {config.ModName} is already registered");
                return;
            }

            _configs[config.ModName] = config;
            LoadConfig(config);
        }

        public void UnregisterConfig(string modName)
        {
            if (_configs.TryGetValue(modName, out var config))
            {
                SaveConfig(config);
                _configs.Remove(modName);
            }
        }

        public void SaveConfig(IModConfig config)
        {
            try
            {
                string configPath = GetConfigPath(config.ModName);
                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(configPath, json);
                config.OnConfigSaved();
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to save config for {config.ModName}: {ex}");
            }
        }

        public void LoadConfig(IModConfig config)
        {
            try
            {
                string configPath = GetConfigPath(config.ModName);
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    JsonSerializer.Deserialize(json, config.GetType());
                    config.OnConfigLoaded();
                }
                else
                {
                    // Save default config if it doesn't exist
                    SaveConfig(config);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to load config for {config.ModName}: {ex}");
            }
        }

        private string GetConfigPath(string modName)
        {
            return Path.Combine(_configDirectory, $"{modName}.json");
        }

        public void SaveAllConfigs()
        {
            foreach (var config in _configs.Values)
            {
                SaveConfig(config);
            }
        }

        public void LoadAllConfigs()
        {
            foreach (var config in _configs.Values)
            {
                LoadConfig(config);
            }
        }
    }
} 