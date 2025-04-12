using System;
using System.Reflection;
using SleddingGameModFramework.API;


namespace SleddingGameModFramework.Core
{
    /// <summary>
    /// Container for a loaded mod and its metadata
    /// </summary>
    public class ModContainer
    {
        /// <summary>
        /// The mod instance
        /// </summary>
        public IMod Mod { get; }
        
        /// <summary>
        /// Mod metadata
        /// </summary>
        public ModInfo Info { get; }
        
        /// <summary>
        /// The assembly containing the mod
        /// </summary>
        public Assembly Assembly { get; }
        
        /// <summary>
        /// Whether the mod has been initialized
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Creates a new mod container
        /// </summary>
        public ModContainer(IMod mod, ModInfo info, Assembly assembly)
        {
            Mod = mod ?? throw new ArgumentNullException(nameof(mod));
            Info = info ?? throw new ArgumentNullException(nameof(info));
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            IsInitialized = false;
        }

        /// <summary>
        /// Initializes the mod
        /// </summary>
        public void Initialize()
        {
            if (IsInitialized) return;

            try
            {
                Mod.Initialize();
                IsInitialized = true;
            }
            catch (Exception ex)
            {
                Utils.Logger.Error($"Failed to initialize mod {Info.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Shuts down the mod
        /// </summary>
        public void Shutdown()
        {
            if (!IsInitialized) return;

            try
            {
                Mod.Shutdown();
                IsInitialized = false;
            }
            catch (Exception ex)
            {
                Utils.Logger.Error($"Error shutting down mod {Info.Id}: {ex.Message}");
                throw;
            }
        }
    }
}