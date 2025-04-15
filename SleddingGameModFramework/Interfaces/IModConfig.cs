using System;

namespace SleddingGameModFramework.Interfaces
{
    public interface IModConfig
    {
        /// <summary>
        /// The name of the mod this configuration belongs to
        /// </summary>
        string ModName { get; }

        /// <summary>
        /// Called when the configuration is loaded
        /// </summary>
        void OnConfigLoaded();

        /// <summary>
        /// Called when the configuration is saved
        /// </summary>
        void OnConfigSaved();

        /// <summary>
        /// Called when a configuration value is changed
        /// </summary>
        /// <param name="propertyName">The name of the property that changed</param>
        void OnConfigChanged(string propertyName);
    }
} 