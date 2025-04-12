using System;
using System.IO;
using Newtonsoft.Json;

namespace SleddingGameModFramework.Configuration
{
    /// <summary>
    /// Generic configuration manager for mod settings
    /// </summary>
    public class ConfigManager<T> where T : class, new()
    {
        private readonly string _configPath;
        private T _config;

        /// <summary>
        /// The configuration data
        /// </summary>
        public T Config
        {
            get => _config;
            set => _config = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Creates a new config manager for the specified mod
        /// </summary>
        /// <param name="modId">The unique ID of the mod</param>
        /// <param name="configName">Optional config name if mod has multiple configs</param>
        public ConfigManager(string modId, string configName = "config")
        {
            string configDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", modId);
            _configPath = Path.Combine(configDir, $"{configName}.json");
            _config = new T();
        }

        /// <summary>
        /// Loads configuration from disk
        /// </summary>
        /// <returns>True if loaded successfully, false otherwise</returns>
        public bool Load()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    string json = File.ReadAllText(_configPath);
                    _config = JsonConvert.DeserializeObject<T>(json) ?? new T();
                    return true;
                }
                
                // config does not exist yet, create default
                Save();
                return true;
            }
            catch (Exception ex)
            {
                // TODO: log this error
                Console.WriteLine($"Error while loading config: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Saves configuration to disk
        /// </summary>
        /// <returns>True if saved successfully, false otherwise</returns>
        public bool Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_configPath));
                string json = JsonConvert.SerializeObject(_config, Formatting.Indented);
                File.WriteAllText(_configPath, json);
                return true;
            }
            catch (Exception ex)
            {
                // TODO: log this error
                Console.WriteLine($"Error while saving config: {ex.Message}");
                return false;
            }
        }
    }
}

