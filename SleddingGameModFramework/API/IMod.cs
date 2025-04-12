using System;

namespace SleddingGameModFramework.API
{
    /// <summary>
    /// Base interface for all mods
    /// </summary>
    public interface IMod
    {
        /// <summary>
        /// Unique identifier for the mod
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Display name of the mod
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Mod author name
        /// </summary>
        string Author { get; }
        
        /// <summary>
        /// Mod version
        /// </summary>
        Version Version { get; }
        
        /// <summary>
        /// Called when the mod is being initialized
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Called when the mod is being unloaded
        /// </summary>
        void Shutdown();
    }
}

