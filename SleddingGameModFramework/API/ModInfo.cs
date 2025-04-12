using System;

namespace SleddingGameModFramework.API
{
    /// <summary>
    /// Contains metadata about a mod
    /// </summary>
    public class ModInfo
    {
        /// <summary>
        /// Unique identifier for the mod
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Display name of the mod
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Mod author name
        /// </summary>
        public string Author { get; set; }
        
        /// <summary>
        /// Mod version
        /// </summary>
        public Version Version { get; set; }
        
        /// <summary>
        /// Brief description of what the mod does
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Optional website URL for the mod
        /// </summary>
        public string Website { get; set; }
        
        /// <summary>
        /// Dependencies on other mods (by ID)
        /// </summary>
        public string[] Dependencies { get; set; } = Array.Empty<string>();
    }
}