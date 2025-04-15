using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SleddingGameModFramework.Interfaces;

namespace SleddingGameModFramework.Core
{
    public class ModLoader
    {
        private readonly List<IMod> _loadedMods = new List<IMod>();
        private readonly string _modsDirectory;

        public ModLoader(string modsDirectory)
        {
            _modsDirectory = modsDirectory;
            if (!Directory.Exists(_modsDirectory))
            {
                Directory.CreateDirectory(_modsDirectory);
            }
        }

        public void LoadMods()
        {
            Plugin.Log.LogInfo($"Loading mods from {_modsDirectory}");

            foreach (string file in Directory.GetFiles(_modsDirectory, "*.dll"))
            {
                try
                {
                    Assembly modAssembly = Assembly.LoadFrom(file);
                    foreach (Type type in modAssembly.GetTypes())
                    {
                        if (typeof(IMod).IsAssignableFrom(type) && !type.IsAbstract)
                        {
                            IMod mod = (IMod)Activator.CreateInstance(type);
                            _loadedMods.Add(mod);
                            mod.OnLoad();
                            Plugin.Log.LogInfo($"Loaded mod: {type.FullName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError($"Failed to load mod from {file}: {ex}");
                }
            }
        }

        public void UnloadMods()
        {
            foreach (IMod mod in _loadedMods)
            {
                try
                {
                    mod.OnUnload();
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError($"Error unloading mod: {ex}");
                }
            }
            _loadedMods.Clear();
        }

        public void UpdateMods()
        {
            foreach (IMod mod in _loadedMods)
            {
                try
                {
                    mod.OnUpdate();
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError($"Error updating mod: {ex}");
                }
            }
        }

        public void PauseMods()
        {
            foreach (IMod mod in _loadedMods)
            {
                try
                {
                    mod.OnPause();
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError($"Error pausing mod: {ex}");
                }
            }
        }

        public void ResumeMods()
        {
            foreach (IMod mod in _loadedMods)
            {
                try
                {
                    mod.OnResume();
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError($"Error resuming mod: {ex}");
                }
            }
        }
    }
} 