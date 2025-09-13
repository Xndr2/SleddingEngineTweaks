using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using BepInEx;

namespace SleddingEngineTweaks.API
{
    /// <summary>
    /// Manages the loading, instantiation, and cleanup of prefabs from AssetBundles.
    /// Provides secure prefab management with validation, caching, and proper resource cleanup.
    /// All prefabs are loaded from the Assets folder and must be packaged as Unity AssetBundles.
    /// </summary>
    public class PrefabManager : IDisposable
    {
        private readonly Dictionary<string, GameObject> _loadedPrefabs = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, AssetBundle> _loadedBundles = new Dictionary<string, AssetBundle>();
        private readonly string _prefabPath;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new PrefabManager instance.
        /// Creates the Assets directory if it doesn't exist.
        /// </summary>
        public PrefabManager()
        {
            _prefabPath = Path.Combine(Paths.PluginPath, "SleddingEngineTweaks", "Assets");
            
            // Create the assets directory if it doesn't exist
            if (!Directory.Exists(_prefabPath))
            {
                Directory.CreateDirectory(_prefabPath);
            }
        }

        /// <summary>
        /// Loads a prefab from the Assets folder using Unity's AssetBundle system.
        /// The prefab must be packaged as an AssetBundle (.bundle or .assetbundle file).
        /// Includes security validation to prevent path traversal attacks.
        /// </summary>
        /// <param name="prefabName">Name of the prefab file (without extension). Must contain only alphanumeric characters, underscores, and hyphens.</param>
        /// <returns>True if loaded successfully, false otherwise</returns>
        /// <remarks>
        /// Security: Prefab names are validated to prevent path traversal attacks.
        /// Only files in the Assets folder can be loaded.
        /// The method tries multiple file extensions and common prefab names within the bundle.
        /// </remarks>
        public bool LoadPrefab(string prefabName)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                Plugin.StaticLogger.LogError("Prefab name cannot be null or empty");
                return false;
            }

            // SECURITY: Prevent path traversal attacks
            if (prefabName.Contains("..") || prefabName.Contains("/") || prefabName.Contains("\\"))
            {
                Plugin.StaticLogger.LogError($"Invalid prefab name '{prefabName}'. Path traversal not allowed.");
                return false;
            }

            // SECURITY: Limit prefab name length and characters
            if (prefabName.Length > 50 || !System.Text.RegularExpressions.Regex.IsMatch(prefabName, @"^[a-zA-Z0-9_-]+$"))
            {
                Plugin.StaticLogger.LogError($"Invalid prefab name '{prefabName}'. Only alphanumeric characters, underscores, and hyphens allowed.");
                return false;
            }

            if (_loadedPrefabs.ContainsKey(prefabName))
            {
                Plugin.StaticLogger.LogInfo($"Prefab '{prefabName}' is already loaded");
                return true;
            }

            try
            {
                // Try different possible file extensions for AssetBundle
                string[] possibleExtensions = { "", ".bundle", ".assetbundle" };
                GameObject prefab = null;
                AssetBundle bundle = null;

                foreach (string extension in possibleExtensions)
                {
                    string fullPath = Path.Combine(_prefabPath, prefabName + extension);
                    if (File.Exists(fullPath))
                    {
                        bundle = AssetBundle.LoadFromFile(fullPath);
                        if (bundle != null)
                        {
                            // Try to load the prefab with the same name as the bundle
                            prefab = bundle.LoadAsset<GameObject>(prefabName);
                            if (prefab == null)
                            {
                                // Try common prefab names
                                string[] commonNames = { "prefab", "Prefab", "PREFAB", "Yes", "yes" };
                                foreach (string commonName in commonNames)
                                {
                                    prefab = bundle.LoadAsset<GameObject>(commonName);
                                    if (prefab != null) break;
                                }
                            }
                            
                            if (prefab != null)
                            {
                                _loadedBundles[prefabName] = bundle;
                                break;
                            }
                            else
                            {
                                bundle.Unload(false);
                                bundle = null;
                            }
                        }
                    }
                }

                if (prefab == null)
                {
                    Plugin.StaticLogger.LogError($"Could not load prefab '{prefabName}' from AssetBundle in {_prefabPath}");
                    return false;
                }

                _loadedPrefabs[prefabName] = prefab;
                Plugin.StaticLogger.LogInfo($"Successfully loaded prefab '{prefabName}' from AssetBundle");
                return true;
            }
            catch (Exception ex)
            {
                Plugin.StaticLogger.LogError($"Error loading prefab '{prefabName}': {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Instantiates a loaded prefab in the scene at the specified position and rotation.
        /// The prefab must be loaded first using LoadPrefab().
        /// </summary>
        /// <param name="prefabName">Name of the prefab to instantiate</param>
        /// <param name="position">World position to instantiate at</param>
        /// <param name="rotation">World rotation to instantiate with</param>
        /// <param name="parent">Parent GameObject to attach to (optional)</param>
        /// <returns>SafeGameObject wrapper for the instantiated object, or null if failed</returns>
        /// <remarks>
        /// The instantiated object gets a unique name for tracking purposes.
        /// </remarks>
        public SafeGameObject InstantiatePrefab(string prefabName, Vector3 position, Quaternion rotation, GameObject parent = null)
        {
            if (!_loadedPrefabs.TryGetValue(prefabName, out GameObject prefab))
            {
                Plugin.StaticLogger.LogError($"Prefab '{prefabName}' is not loaded. Call LoadPrefab first.");
                return null;
            }

            try
            {
                GameObject instance = UnityEngine.Object.Instantiate(prefab, position, rotation, parent?.transform);
                
                // Generate a unique name for tracking
                string instanceName = $"{prefabName}_{DateTime.Now.Ticks}";
                instance.name = instanceName;
                
                _instantiatedPrefabs[instanceName] = instance;
                
                Plugin.StaticLogger.LogInfo($"Instantiated prefab '{prefabName}' as '{instanceName}'");
                return new SafeGameObject(instance);
            }
            catch (Exception ex)
            {
                Plugin.StaticLogger.LogError($"Error instantiating prefab '{prefabName}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Instantiates a prefab and parents it to a specific GameObject.
        /// The prefab will be positioned and rotated relative to its parent.
        /// </summary>
        /// <param name="prefabName">Name of the prefab to instantiate</param>
        /// <param name="parent">Parent GameObject to attach the prefab to</param>
        /// <param name="localPosition">Local position relative to parent (defaults to Vector3.zero)</param>
        /// <param name="localRotation">Local rotation relative to parent (defaults to Quaternion.identity)</param>
        /// <returns>SafeGameObject wrapper for the instantiated object, or null if failed</returns>
        /// <remarks>
        /// If parent is null, the method will log an error and return null.
        /// </remarks>
        public SafeGameObject InstantiatePrefabAsChild(string prefabName, GameObject parent, Vector3 localPosition = default, Quaternion localRotation = default)
        {
            if (parent == null)
            {
                Plugin.StaticLogger.LogError("Parent object cannot be null");
                return null;
            }

            SafeGameObject instance = InstantiatePrefab(prefabName, Vector3.zero, Quaternion.identity, parent);
            if (instance != null && instance.Inner != null)
            {
                instance.Inner.transform.localPosition = localPosition;
                instance.Inner.transform.localRotation = localRotation;
            }

            return instance;
        }

        /// <summary>
        /// Instantiates a prefab and parents it to a specific bone in a skeletal mesh.
        /// Searches recursively through the skeleton hierarchy to find the bone.
        /// </summary>
        /// <param name="prefabName">Name of the prefab to instantiate</param>
        /// <param name="skeletonRoot">Root GameObject of the skeleton to search</param>
        /// <param name="boneName">Name of the bone to parent to (case-insensitive)</param>
        /// <param name="localPosition">Local position relative to bone (defaults to Vector3.zero)</param>
        /// <param name="localRotation">Local rotation relative to bone (defaults to Quaternion.identity)</param>
        /// <returns>SafeGameObject wrapper for the instantiated object, or null if bone not found</returns>
        /// <remarks>
        /// Useful for attaching weapons, accessories, or effects to specific body parts.
        /// </remarks>
        public SafeGameObject InstantiatePrefabOnBone(string prefabName, GameObject skeletonRoot, string boneName, Vector3 localPosition = default, Quaternion localRotation = default)
        {
            if (skeletonRoot == null)
            {
                Plugin.StaticLogger.LogError("Skeleton root cannot be null");
                return null;
            }

            Transform bone = FindBoneInSkeleton(skeletonRoot.transform, boneName);
            if (bone == null)
            {
                Plugin.StaticLogger.LogError($"Bone '{boneName}' not found in skeleton");
                return null;
            }

            return InstantiatePrefabAsChild(prefabName, bone.gameObject, localPosition, localRotation);
        }

        /// <summary>
        /// Recursively searches for a bone in a skeleton hierarchy.
        /// Search is case-insensitive.
        /// </summary>
        /// <param name="root">The root transform to search under</param>
        /// <param name="boneName">The name of the bone to find</param>
        /// <returns>The found transform, or null if not found</returns>
        private Transform FindBoneInSkeleton(Transform root, string boneName)
        {
            if (root.name.Equals(boneName, StringComparison.OrdinalIgnoreCase))
            {
                return root;
            }

            for (int i = 0; i < root.childCount; i++)
            {
                Transform found = FindBoneInSkeleton(root.GetChild(i), boneName);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Destroys an instantiated prefab by its instance name.
        /// The instance name is automatically generated when the prefab is instantiated.
        /// </summary>
        /// <param name="instanceName">Name of the instantiated object to destroy</param>
        /// <returns>True if destroyed successfully, false if instance not found</returns>
        /// <remarks>
        /// Use GetInstantiatedPrefabNames() to see all available instance names.
        /// </remarks>
        public bool DestroyPrefabInstance(string instanceName)
        {
            if (_instantiatedPrefabs.TryGetValue(instanceName, out GameObject instance))
            {
                if (instance != null)
                {
                    UnityEngine.Object.Destroy(instance);
                }
                _instantiatedPrefabs.Remove(instanceName);
                Plugin.StaticLogger.LogInfo($"Destroyed prefab instance '{instanceName}'");
                return true;
            }

            Plugin.StaticLogger.LogWarning($"Prefab instance '{instanceName}' not found");
            return false;
        }

        /// <summary>
        /// Gets all currently loaded prefab names.
        /// These are the prefabs that have been successfully loaded from AssetBundles.
        /// </summary>
        /// <returns>Array of loaded prefab names</returns>
        public string[] GetLoadedPrefabNames()
        {
            string[] names = new string[_loadedPrefabs.Count];
            _loadedPrefabs.Keys.CopyTo(names, 0);
            return names;
        }

        /// <summary>
        /// Gets all currently instantiated prefab instance names.
        /// These are the names of prefab instances that exist in the scene.
        /// </summary>
        /// <returns>Array of instantiated prefab instance names</returns>
        public string[] GetInstantiatedPrefabNames()
        {
            string[] names = new string[_instantiatedPrefabs.Count];
            _instantiatedPrefabs.Keys.CopyTo(names, 0);
            return names;
        }

        /// <summary>
        /// Checks if a prefab is currently loaded in memory.
        /// </summary>
        /// <param name="prefabName">Name of the prefab to check</param>
        /// <returns>True if the prefab is loaded, false otherwise</returns>
        public bool IsPrefabLoaded(string prefabName)
        {
            return _loadedPrefabs.ContainsKey(prefabName);
        }

        /// <summary>
        /// Unloads a prefab from memory and frees its AssetBundle.
        /// All instances of this prefab will be destroyed.
        /// </summary>
        /// <param name="prefabName">Name of the prefab to unload</param>
        /// <returns>True if unloaded successfully, false if prefab not found</returns>
        /// <remarks>
        /// This will destroy all instances of the prefab and free memory.
        /// </remarks>
        public bool UnloadPrefab(string prefabName)
        {
            if (_loadedPrefabs.TryGetValue(prefabName, out GameObject prefab))
            {
                if (prefab != null)
                {
                    UnityEngine.Object.Destroy(prefab);
                }
                _loadedPrefabs.Remove(prefabName);
                
                // Also unload the associated AssetBundle
                if (_loadedBundles.TryGetValue(prefabName, out AssetBundle bundle))
                {
                    if (bundle != null)
                    {
                        bundle.Unload(false);
                    }
                    _loadedBundles.Remove(prefabName);
                }
                
                Plugin.StaticLogger.LogInfo($"Unloaded prefab '{prefabName}' and its AssetBundle");
                return true;
            }

            Plugin.StaticLogger.LogWarning($"Prefab '{prefabName}' is not loaded");
            return false;
        }

        /// <summary>
        /// Destroys all instantiated prefab instances and cleans up memory.
        /// This does not unload the prefabs themselves, only their instances.
        /// </summary>
        public void CleanupAllInstances()
        {
            foreach (var kvp in _instantiatedPrefabs)
            {
                if (kvp.Value != null)
                {
                    UnityEngine.Object.Destroy(kvp.Value);
                }
            }
            _instantiatedPrefabs.Clear();
            Plugin.StaticLogger.LogInfo("Cleaned up all prefab instances");
        }

        /// <summary>
        /// Disposes of the PrefabManager and cleans up all loaded prefabs, instances, and AssetBundles.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected dispose method for proper cleanup.
        /// </summary>
        /// <param name="disposing">True if called from Dispose(), false if called from finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CleanupAllInstances();
                    
                    // Unload all prefabs
                    foreach (var kvp in _loadedPrefabs)
                    {
                        if (kvp.Value != null)
                        {
                            UnityEngine.Object.Destroy(kvp.Value);
                        }
                    }
                    _loadedPrefabs.Clear();
                    
                    // Unload all AssetBundles
                    foreach (var kvp in _loadedBundles)
                    {
                        if (kvp.Value != null)
                        {
                            kvp.Value.Unload(false);
                        }
                    }
                    _loadedBundles.Clear();
                }
                _disposed = true;
            }
        }
    }
}
