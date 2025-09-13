using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace SleddingEngineTweaks.API
{
    /// <summary>
    /// API class for managing prefab loading, instantiation, and parenting in the game.
    /// Provides safe methods for modders to load AssetBundle prefabs and instantiate them
    /// with various parenting options (world space, player head, camera, bones, etc.).
    /// All methods return SafeGameObject wrappers to prevent direct Unity object access.
    /// </summary>
    public class PrefabAPI : IDisposable
    {
        private readonly Plugin _plugin;
        private readonly PrefabManager _prefabManager;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the PrefabAPI.
        /// </summary>
        /// <param name="plugin">The plugin instance that owns this API</param>
        public PrefabAPI(Plugin plugin)
        {
            _plugin = plugin;
            _prefabManager = new PrefabManager();
        }

        /// <summary>
        /// Disposes of the PrefabAPI and cleans up all loaded prefabs and instances.
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
                    _prefabManager?.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Loads a prefab from the Assets folder using Unity's AssetBundle system.
        /// The prefab must be packaged as an AssetBundle (.bundle or .assetbundle file).
        /// </summary>
        /// <param name="prefabName">Name of the prefab file (without extension). Must contain only alphanumeric characters, underscores, and hyphens.</param>
        /// <returns>True if loaded successfully, false otherwise</returns>
        /// <remarks>
        /// Security: Prefab names are validated to prevent path traversal attacks.
        /// Only files in the Assets folder can be loaded.
        /// </remarks>
        public bool LoadPrefab(string prefabName)
        {
            return _prefabManager.LoadPrefab(prefabName);
        }

        /// <summary>
        /// Instantiates a prefab at a specific world position.
        /// The prefab must be loaded first using LoadPrefab().
        /// </summary>
        /// <param name="prefabName">Name of the prefab to instantiate</param>
        /// <param name="position">World position where the prefab should be instantiated</param>
        /// <param name="rotation">World rotation as Quaternion or Vector3 (optional, defaults to identity)</param>
        /// <returns>SafeGameObject wrapper for the instantiated object, or null if failed</returns>
        /// <remarks>
        /// Rotation parameter accepts both Quaternion and Vector3 (converted to Euler angles).
        /// </remarks>
        public SafeGameObject InstantiatePrefab(string prefabName, Vector3 position, object rotation = null)
        {
            Quaternion rot = Quaternion.identity;
            if (rotation != null)
            {
                if (rotation is Quaternion quat)
                {
                    rot = quat;
                }
                else if (rotation is Vector3 vec)
                {
                    rot = Quaternion.Euler(vec);
                }
            }
            return _prefabManager.InstantiatePrefab(prefabName, position, rot);
        }

        /// <summary>
        /// Instantiates a prefab and parents it to another GameObject.
        /// The prefab will be positioned and rotated relative to its parent.
        /// </summary>
        /// <param name="prefabName">Name of the prefab to instantiate</param>
        /// <param name="parent">Parent SafeGameObject to attach the prefab to</param>
        /// <param name="localPosition">Local position relative to parent (optional, defaults to Vector3.zero)</param>
        /// <param name="localRotation">Local rotation relative to parent as Quaternion or Vector3 (optional, defaults to identity)</param>
        /// <returns>SafeGameObject wrapper for the instantiated object, or null if failed</returns>
        /// <remarks>
        /// If parent is null or invalid, the method will log an error and return null.
        /// </remarks>
        public SafeGameObject InstantiatePrefabAsChild(string prefabName, SafeGameObject parent, Vector3? localPosition = null, object localRotation = null)
        {
            if (parent == null || parent.Inner == null)
            {
                Log("Parent object is null or invalid");
                return null;
            }

            Vector3 pos = localPosition ?? Vector3.zero;
            Quaternion rot = Quaternion.identity;
            if (localRotation != null)
            {
                if (localRotation is Quaternion quat)
                {
                    rot = quat;
                }
                else if (localRotation is Vector3 vec)
                {
                    rot = Quaternion.Euler(vec);
                }
            }
            return _prefabManager.InstantiatePrefabAsChild(prefabName, parent.Inner, pos, rot);
        }

        /// <summary>
        /// Instantiates a prefab and parents it to the player's head/camera.
        /// Automatically searches for "Head", "Camera", or MainCamera tagged objects.
        /// Falls back to player root if no head/camera is found.
        /// </summary>
        /// <param name="prefabName">Name of the prefab to instantiate</param>
        /// <param name="localPosition">Local position relative to head (optional, defaults to Vector3.zero)</param>
        /// <param name="localRotation">Local rotation relative to head as Quaternion or Vector3 (optional, defaults to identity)</param>
        /// <returns>SafeGameObject wrapper for the instantiated object, or null if player not found</returns>
        /// <remarks>
        /// Useful for attaching UI elements, weapons, or accessories to the player's view.
        /// </remarks>
        public SafeGameObject InstantiatePrefabOnPlayerHead(string prefabName, Vector3? localPosition = null, object localRotation = null)
        {
            var player = Plugin.GameAPI?.GetPlayer();
            if (player == null || player.Inner == null)
            {
                Log("Player not found");
                return null;
            }

            // Try to find the head/camera in the player hierarchy
            Transform head = FindChildByName(player.Inner.transform, "Head");
            if (head == null)
            {
                head = FindChildByName(player.Inner.transform, "Camera");
            }
            if (head == null)
            {
                head = FindChildByTag(player.Inner.transform, "MainCamera");
            }
            if (head == null)
            {
                // Fallback to player root
                head = player.Inner.transform;
            }

            Vector3 pos = localPosition ?? Vector3.zero;
            Quaternion rot = Quaternion.identity;
            if (localRotation != null)
            {
                if (localRotation is Quaternion quat)
                {
                    rot = quat;
                }
                else if (localRotation is Vector3 vec)
                {
                    rot = Quaternion.Euler(vec);
                }
            }
            return _prefabManager.InstantiatePrefabAsChild(prefabName, head.gameObject, pos, rot);
        }

        /// <summary>
        /// Instantiates a prefab and parents it to the main camera.
        /// The prefab will move and rotate with the camera.
        /// </summary>
        /// <param name="prefabName">Name of the prefab to instantiate</param>
        /// <param name="localPosition">Local position relative to camera (optional, defaults to Vector3.zero)</param>
        /// <param name="localRotation">Local rotation relative to camera as Quaternion or Vector3 (optional, defaults to identity)</param>
        /// <returns>SafeGameObject wrapper for the instantiated object, or null if main camera not found</returns>
        /// <remarks>
        /// Useful for camera-mounted effects, UI overlays, or first-person accessories.
        /// </remarks>
        public SafeGameObject InstantiatePrefabOnCamera(string prefabName, Vector3? localPosition = null, object localRotation = null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Log("Main camera not found");
                return null;
            }

            Vector3 pos = localPosition ?? Vector3.zero;
            Quaternion rot = Quaternion.identity;
            if (localRotation != null)
            {
                if (localRotation is Quaternion quat)
                {
                    rot = quat;
                }
                else if (localRotation is Vector3 vec)
                {
                    rot = Quaternion.Euler(vec);
                }
            }
            return _prefabManager.InstantiatePrefabAsChild(prefabName, mainCamera.gameObject, pos, rot);
        }

        /// <summary>
        /// Instantiates a prefab and parents it to a specific bone in a skeleton.
        /// Searches recursively through the skeleton hierarchy to find the bone.
        /// </summary>
        /// <param name="prefabName">Name of the prefab to instantiate</param>
        /// <param name="skeletonRoot">Root SafeGameObject of the skeleton to search</param>
        /// <param name="boneName">Name of the bone to parent to (case-insensitive)</param>
        /// <param name="localPosition">Local position relative to bone (optional, defaults to Vector3.zero)</param>
        /// <param name="localRotation">Local rotation relative to bone as Quaternion or Vector3 (optional, defaults to identity)</param>
        /// <returns>SafeGameObject wrapper for the instantiated object, or null if bone not found</returns>
        /// <remarks>
        /// Useful for attaching weapons, accessories, or effects to specific body parts.
        /// </remarks>
        public SafeGameObject InstantiatePrefabOnBone(string prefabName, SafeGameObject skeletonRoot, string boneName, Vector3? localPosition = null, object localRotation = null)
        {
            if (skeletonRoot == null || skeletonRoot.Inner == null)
            {
                Log("Skeleton root is null or invalid");
                return null;
            }

            Vector3 pos = localPosition ?? Vector3.zero;
            Quaternion rot = Quaternion.identity;
            if (localRotation != null)
            {
                if (localRotation is Quaternion quat)
                {
                    rot = quat;
                }
                else if (localRotation is Vector3 vec)
                {
                    rot = Quaternion.Euler(vec);
                }
            }
            return _prefabManager.InstantiatePrefabOnBone(prefabName, skeletonRoot.Inner, boneName, pos, rot);
        }

        /// <summary>
        /// Instantiates a prefab and parents it to a bone in the player's skeleton.
        /// Convenience method that automatically finds the player and searches their skeleton.
        /// </summary>
        /// <param name="prefabName">Name of the prefab to instantiate</param>
        /// <param name="boneName">Name of the bone to parent to (case-insensitive)</param>
        /// <param name="localPosition">Local position relative to bone (optional, defaults to Vector3.zero)</param>
        /// <param name="localRotation">Local rotation relative to bone as Quaternion or Vector3 (optional, defaults to identity)</param>
        /// <returns>SafeGameObject wrapper for the instantiated object, or null if player or bone not found</returns>
        /// <remarks>
        /// Common bone names: "Head", "Neck", "Chest", "Hand", "Foot", etc.
        /// </remarks>
        public SafeGameObject InstantiatePrefabOnPlayerBone(string prefabName, string boneName, Vector3? localPosition = null, object localRotation = null)
        {
            var player = Plugin.GameAPI?.GetPlayer();
            if (player == null || player.Inner == null)
            {
                Log("Player not found");
                return null;
            }

            return InstantiatePrefabOnBone(prefabName, player, boneName, localPosition, localRotation);
        }

        /// <summary>
        /// Destroys a prefab instance by its instance name.
        /// The instance name is automatically generated when the prefab is instantiated.
        /// </summary>
        /// <param name="instanceName">Name of the instantiated object to destroy</param>
        /// <returns>True if destroyed successfully, false if instance not found</returns>
        /// <remarks>
        /// Use GetInstantiatedPrefabNames() to see all available instance names.
        /// </remarks>
        public bool DestroyPrefabInstance(string instanceName)
        {
            return _prefabManager.DestroyPrefabInstance(instanceName);
        }

        /// <summary>
        /// Gets all currently loaded prefab names.
        /// These are the prefabs that have been successfully loaded from AssetBundles.
        /// </summary>
        /// <returns>Array of loaded prefab names</returns>
        public string[] GetLoadedPrefabNames()
        {
            return _prefabManager.GetLoadedPrefabNames();
        }

        /// <summary>
        /// Gets all currently instantiated prefab instance names.
        /// These are the names of prefab instances that exist in the scene.
        /// </summary>
        /// <returns>Array of instantiated prefab instance names</returns>
        public string[] GetInstantiatedPrefabNames()
        {
            return _prefabManager.GetInstantiatedPrefabNames();
        }

        /// <summary>
        /// Checks if a prefab is currently loaded in memory.
        /// </summary>
        /// <param name="prefabName">Name of the prefab to check</param>
        /// <returns>True if the prefab is loaded, false otherwise</returns>
        public bool IsPrefabLoaded(string prefabName)
        {
            return _prefabManager.IsPrefabLoaded(prefabName);
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
            return _prefabManager.UnloadPrefab(prefabName);
        }

        /// <summary>
        /// Destroys all instantiated prefab instances and cleans up memory.
        /// This does not unload the prefabs themselves, only their instances.
        /// </summary>
        public void CleanupAllInstances()
        {
            _prefabManager.CleanupAllInstances();
        }

        /// <summary>
        /// Recursively finds a child object by name in a transform hierarchy.
        /// Search is case-insensitive.
        /// </summary>
        /// <param name="parent">The parent transform to search under</param>
        /// <param name="name">The name to search for</param>
        /// <returns>The found transform, or null if not found</returns>
        private Transform FindChildByName(Transform parent, string name)
        {
            if (parent.name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return parent;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform found = FindChildByName(parent.GetChild(i), name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Recursively finds a child object by tag in a transform hierarchy.
        /// </summary>
        /// <param name="parent">The parent transform to search under</param>
        /// <param name="tag">The tag to search for</param>
        /// <returns>The found transform, or null if not found</returns>
        private Transform FindChildByTag(Transform parent, string tag)
        {
            if (parent.CompareTag(tag))
            {
                return parent;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform found = FindChildByTag(parent.GetChild(i), tag);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Logs a message to the console with PrefabAPI prefix.
        /// </summary>
        /// <param name="message">The message to log</param>
        private void Log(string message)
        {
            _plugin.LuaManager.OutputMessage($"[PrefabAPI]: {message}");
        }
    }
}
