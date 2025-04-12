using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using SleddingGameModFramework.API;
using SleddingGameModFramework.Utils;

namespace SleddingGameModFramework.Core
{
    /// <summary>
    /// Main mod management system
    /// </summary>
    public class ModManager
    {
        private readonly List<ModContainer> _loadedMods = new();
        private readonly string _modsDirectory;
        private bool _isInitialized = false;
        
        /// <summary>
        /// Read-only collection of loaded mods
        /// </summary>
        public IReadOnlyList<ModContainer> LoadedMods => _loadedMods.AsReadOnly();
        
        /// <summary>
        /// Creates a new mod manager
        /// </summary>
        /// <param name="modsDirectory">Directory to scan for mods</param>
        public ModManager(string modsDirectory)
        {
            _modsDirectory = modsDirectory ?? throw new ArgumentNullException(nameof(modsDirectory));
        }
        
        /// <summary>
        /// Initializes the mod manager and discovers mods
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;
            
            Logger.Initialize();
            Logger.Info("Initializing mod manager");
            
            if (!Directory.Exists(_modsDirectory))
            {
                Logger.Info($"Creating mods directory: {_modsDirectory}");
                Directory.CreateDirectory(_modsDirectory);
            }
            
            DiscoverMods();
            _isInitialized = true;
        }
        
        /// <summary>
        /// Discovers and loads mods from the mods directory
        /// </summary>
        public void DiscoverMods()
        {
            Logger.Info($"Discovering mods in: {_modsDirectory}");
            
            // Look for mod directories
            foreach (string modDir in Directory.GetDirectories(_modsDirectory))
            {
                string modInfoPath = Path.Combine(modDir, "modinfo.json");
                string dllPath = null;
                
                // Skip directories without a modinfo.json file
                if (!File.Exists(modInfoPath))
                {
                    Logger.Warning($"Directory without modinfo.json: {modDir}");
                    continue;
                }
                
                try
                {
                    // Load mod metadata
                    string json = File.ReadAllText(modInfoPath);
                    ModInfo modInfo = JsonConvert.DeserializeObject<ModInfo>(json);
                    
                    if (modInfo == null)
                    {
                        Logger.Error($"Invalid modinfo.json in: {modDir}");
                        continue;
                    }
                    
                    // Look for a DLL with the same name as the mod ID
                    dllPath = Path.Combine(modDir, $"{modInfo.Id}.dll");
                    if (!File.Exists(dllPath))
                    {
                        // Try looking for any DLL
                        var dlls = Directory.GetFiles(modDir, "*.dll");
                        if (dlls.Length > 0)
                        {
                            dllPath = dlls[0];
                        }
                        else
                        {
                            Logger.Error($"No DLL found for mod: {modInfo.Id}");
                            continue;
                        }
                    }
                    
                    LoadMod(dllPath, modInfo);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error loading mod from {modDir}: {ex.Message}");
                }
            }
            
            Logger.Info($"Discovered {_loadedMods.Count} mods");
        }
        
        /// <summary>
        /// Loads a mod from the specified DLL
        /// </summary>
        /// <param name="dllPath">Path to the mod DLL</param>
        /// <param name="modInfo">Metadata for the mod</param>
        private void LoadMod(string dllPath, ModInfo modInfo)
        {
            try
            {
                Logger.Info($"Loading mod: {modInfo.Id} from {dllPath}");
                
                // Load the assembly
                Assembly assembly = Assembly.LoadFrom(dllPath);
                
                // Find types that implement IMod
                Type modType = assembly.GetTypes()
                    .FirstOrDefault(t => typeof(IMod).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                
                if (modType == null)
                {
                    Logger.Error($"No IMod implementation found in {dllPath}");
                    return;
                }
                
                // Create an instance of the mod
                IMod mod = (IMod)Activator.CreateInstance(modType);
                
                if (mod == null)
                {
                    Logger.Error($"Failed to create instance of mod {modInfo.Id}");
                    return;
                }
                
                // Verify that the mod ID matches
                if (mod.Id != modInfo.Id)
                {
                    Logger.Warning($"Mod ID mismatch: {mod.Id} vs {modInfo.Id}");
                }
                
                // Create and add the mod container
                ModContainer container = new ModContainer(mod, modInfo, assembly);
                _loadedMods.Add(container);
                
                Logger.Info($"Successfully loaded mod: {modInfo.Id} v{modInfo.Version}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading mod {modInfo.Id}: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Initializes all loaded mods in dependency order
        /// </summary>
        public void InitializeMods()
        {
            Logger.Info("Initializing mods");
            
            // This is a simplified approach - a real implementation would need
            // to handle circular dependencies and more robust error handling
            
            // Build a dictionary of mods by ID for dependency lookup
            Dictionary<string, ModContainer> modsById = _loadedMods.ToDictionary(m => m.Info.Id);
            
            // Process mods in order of dependencies
            foreach (ModContainer mod in _loadedMods)
            {
                InitializeModWithDependencies(mod, modsById, new HashSet<string>());
            }
            
            Logger.Info($"Initialized {_loadedMods.Count(m => m.IsInitialized)} mods");
        }
        
        /// <summary>
        /// Initializes a mod after initializing its dependencies
        /// </summary>
        private void InitializeModWithDependencies(
            ModContainer mod, 
            Dictionary<string, ModContainer> modsById, 
            HashSet<string> initStack)
        {
            // Skip if already initialized
            if (mod.IsInitialized) return;
            
            // Check for circular dependencies
            if (initStack.Contains(mod.Info.Id))
            {
                Logger.Error($"Circular dependency detected for mod {mod.Info.Id}");
                return;
            }
            
            // Add this mod to the initialization stack
            initStack.Add(mod.Info.Id);
            
            // Initialize dependencies first
            foreach (string dependencyId in mod.Info.Dependencies)
            {
                if (modsById.TryGetValue(dependencyId, out ModContainer dependency))
                {
                    InitializeModWithDependencies(dependency, modsById, initStack);
                }
                else
                {
                    Logger.Warning($"Missing dependency {dependencyId} for mod {mod.Info.Id}");
                }
            }
            
            // Initialize the mod
            try
            {
                mod.Initialize();
                Logger.Info($"Initialized mod: {mod.Info.Id}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize mod {mod.Info.Id}: {ex.Message}");
            }
            
            // Remove this mod from the initialization stack
            initStack.Remove(mod.Info.Id);
        }
        
        /// <summary>
        /// Shuts down all loaded mods
        /// </summary>
        public void ShutdownMods()
        {
            Logger.Info("Shutting down mods");
            
            // Shutdown in reverse order of loading (dependencies last)
            foreach (ModContainer mod in _loadedMods.AsEnumerable().Reverse())
            {
                try
                {
                    if (mod.IsInitialized)
                    {
                        mod.Shutdown();
                        Logger.Info($"Shut down mod: {mod.Info.Id}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error shutting down mod {mod.Info.Id}: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Gets a mod by its ID
        /// </summary>
        /// <param name="modId">The mod ID to look for</param>
        /// <returns>The mod container, or null if not found</returns>
        public ModContainer GetMod(string modId)
        {
            return _loadedMods.FirstOrDefault(m => m.Info.Id == modId);
        }
    }
}