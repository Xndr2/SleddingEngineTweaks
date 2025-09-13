using BepInEx;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using SleddingEngineTweaks.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Reflection;

namespace SleddingEngineTweaks.API
{
    /// <summary>
    /// Main API class providing safe access to game functionality for Lua scripts.
    /// This class exposes player manipulation, camera control, input handling, and GameObject operations.
    /// All methods are designed to be safe for modding and prevent crashes.
    /// </summary>
    public class GameAPI : IDisposable
    {
        private readonly Plugin _plugin;
        private readonly Dictionary<string, GameObject> _gameObjectCache = new Dictionary<string, GameObject>();

        /// <summary>
        /// Initializes a new instance of the GameAPI.
        /// </summary>
        /// <param name="plugin">The plugin instance that owns this API</param>
        public GameAPI(Plugin plugin)
        {
            _plugin = plugin;
            // sub to scene changes to clear the cache
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Clean up the event subscriptions when the API is no longer needed.
        /// This prevents memory leaks and ensures proper cleanup.
        /// </summary>
        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // clear the cache to avoid holding refs to a destroyed object
            _gameObjectCache.Clear();
            Plugin.StaticLogger.LogInfo("Game object cache cleared due to scene change.");
        }

        /// <summary>
        /// Logs a message to the console with GameAPI prefix.
        /// Useful for debugging and providing feedback to modders.
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Log(string message)
        {
            _plugin.LuaManager.OutputMessage($"[GameAPI]: {message}");
        }

        #region Player API
        private static readonly string[] KnownPlayerPaths = new[]
        {
            "Environment/HangarShip/Player", // lobby
            "PlayersContainer/Player"         // in-game
        };

        private GameObject ResolvePlayerRoot()
        {
            // Try cached player first
            if (_gameObjectCache.TryGetValue("Player", out var cachedPlayer) && cachedPlayer != null)
            {
                return cachedPlayer;
            }

            // Try known explicit paths first
            foreach (var path in KnownPlayerPaths)
            {
                var goByPath = GameObject.Find(path);
                if (goByPath != null) 
                {
                    _gameObjectCache["Player"] = goByPath;
                    return goByPath;
                }
            }

            // Fallbacks: by tag, by name
            var byTag = GameObject.FindGameObjectWithTag("Player");
            if (byTag != null) 
            {
                _gameObjectCache["Player"] = byTag;
                return byTag;
            }

            var byName = GameObject.Find("Player");
            if (byName != null) 
            {
                _gameObjectCache["Player"] = byName;
                return byName;
            }

            return null;
        }

        /// <summary>
        /// Gets the player GameObject as a SafeGameObject wrapper.
        /// Returns null if the player is not found in the current scene.
        /// </summary>
        /// <returns>A SafeGameObject representing the player, or null if not found</returns>
        public SafeGameObject GetPlayer()
        {
            var go = ResolvePlayerRoot();
            return go != null ? new SafeGameObject(go) : null;
        }

        /// <summary>
        /// Gets the current position of the player in world coordinates.
        /// Returns Vector3.zero if the player is not found.
        /// </summary>
        /// <returns>The player's world position as Vector3</returns>
        public Vector3 GetPlayerPosition()
        {
            var player = ResolvePlayerRoot();
            return player != null ? player.transform.position : Vector3.zero;
        }

        /// <summary>
        /// Sets the player's position in world coordinates.
        /// Logs a warning if the player is not found.
        /// </summary>
        /// <param name="position">The new world position for the player</param>
        public void SetPlayerPosition(Vector3 position)
        {
            var player = ResolvePlayerRoot();
            if (player != null)
            {
                player.transform.position = position;
            }
            else
            {
                Log("Warning: Player not found, cannot set position");
            }
        }

        /// <summary>
        /// Enables or disables noclip mode for the player.
        /// When enabled, the player can move through walls and objects.
        /// When disabled, all physics and collision settings are restored.
        /// </summary>
        /// <param name="enabled">True to enable noclip, false to disable</param>
        public void SetNoClip(bool enabled)
        {
            var player = ResolvePlayerRoot();
            if (player == null) return;

            var marker = player.GetComponent<NoclipStateMarker>();
            if (enabled)
            {
                if (marker == null)
                {
                    marker = player.AddComponent<NoclipStateMarker>();
                }
                marker.WasActive = true;
                marker.AffectedColliders.Clear();
                marker.ColliderPrevEnabled.Clear();
                marker.AffectedRigidbodies.Clear();
                marker.RbPrevUseGravity.Clear();
                marker.RbPrevDetectCollisions.Clear();
                marker.RbPrevIsKinematic.Clear();
                marker.CharacterControllers.Clear();
                marker.CharacterControllersPrevEnabled.Clear();

                var colliders = player.GetComponentsInChildren<Collider>(true);
                foreach (var c in colliders)
                {
                    marker.AffectedColliders.Add(c);
                    marker.ColliderPrevEnabled.Add(c.enabled);
                    // Disable collisions either by disabling or making it a trigger
                    if (c is CharacterController)
                    {
                        // handled separately below
                    }
                    else
                    {
                        c.enabled = false;
                    }
                }

                var rigidbodies = player.GetComponentsInChildren<Rigidbody>(true);
                foreach (var rb in rigidbodies)
                {
                    marker.AffectedRigidbodies.Add(rb);
                    marker.RbPrevUseGravity.Add(rb.useGravity);
                    marker.RbPrevDetectCollisions.Add(rb.detectCollisions);
                    marker.RbPrevIsKinematic.Add(rb.isKinematic);
                    rb.useGravity = false;
                    rb.detectCollisions = false;
                    rb.isKinematic = true;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                var ccs = player.GetComponentsInChildren<CharacterController>(true);
                foreach (var cc in ccs)
                {
                    marker.CharacterControllers.Add(cc);
                    marker.CharacterControllersPrevEnabled.Add(cc.enabled);
                    cc.enabled = false;
                }
            }
            else
            {
                if (marker == null) return;
                for (int i = 0; i < marker.AffectedColliders.Count; i++)
                {
                    if (marker.AffectedColliders[i] != null)
                        marker.AffectedColliders[i].enabled = marker.ColliderPrevEnabled[i];
                }
                for (int i = 0; i < marker.AffectedRigidbodies.Count; i++)
                {
                    var rb = marker.AffectedRigidbodies[i];
                    if (rb != null)
                    {
                        rb.useGravity = marker.RbPrevUseGravity[i];
                        rb.detectCollisions = marker.RbPrevDetectCollisions[i];
                        rb.isKinematic = marker.RbPrevIsKinematic[i];
                    }
                }
                for (int i = 0; i < marker.CharacterControllers.Count; i++)
                {
                    var cc = marker.CharacterControllers[i];
                    if (cc != null) cc.enabled = marker.CharacterControllersPrevEnabled[i];
                }
                UnityEngine.Object.Destroy(marker);
            }
        }

        /// <summary>
        /// Moves the player by a specified world-space offset.
        /// This is additive to the current position.
        /// </summary>
        /// <param name="worldDelta">The world-space offset to move the player by</param>
        public void MovePlayer(Vector3 worldDelta)
        {
            var player = ResolvePlayerRoot();
            if (player == null) return;
            player.transform.position += worldDelta;
        }

        /// <summary>
        /// Moves the player forward by a specified distance in the direction they are facing.
        /// </summary>
        /// <param name="distance">The distance to move forward (negative values move backward)</param>
        public void MovePlayerForward(float distance)
        {
            var player = ResolvePlayerRoot();
            if (player == null) return;
            var forward = player.transform.forward;
            player.transform.position += forward * distance;
        }

        /// <summary>
        /// Moves the player along a specified direction relative to their orientation.
        /// The direction is interpreted in the player's local space.
        /// </summary>
        /// <param name="direction">The direction to move in (relative to player's orientation)</param>
        /// <param name="distance">The distance to move</param>
        public void MovePlayerAlong(Vector3 direction, float distance)
        {
            var player = ResolvePlayerRoot();
            if (player == null) return;
            // Interpret given direction in local space
            Vector3 local = player.transform.TransformDirection(direction.normalized);
            player.transform.position += local * distance;
        }
        #endregion
        
        #region Camera API
        /// <summary>
        /// Gets the main camera GameObject.
        /// Returns null if no main camera is found.
        /// </summary>
        /// <returns>The main camera, or null if not found</returns>
        public Camera GetMainCamera()
        {
            return Camera.main;
        }

        /// <summary>
        /// Gets the current position of the main camera in world coordinates.
        /// Returns Vector3.zero if no main camera is found.
        /// </summary>
        /// <returns>The camera's world position as Vector3</returns>
        public Vector3 GetCameraPosition()
        {
            return Camera.main != null ? Camera.main.transform.position : Vector3.zero;
        }

        /// <summary>
        /// Sets the main camera's position in world coordinates.
        /// Does nothing if no main camera is found.
        /// </summary>
        /// <param name="position">The new world position for the camera</param>
        public void SetCameraPosition(Vector3 position)
        {
            if (Camera.main != null)
            {
                Camera.main.transform.position = position;
            }
        }
        #endregion
        
        #region Input API
        /// <summary>
        /// Checks if a key was pressed this frame (just pressed, not held).
        /// Key names are case-insensitive and should match Unity Input System key names.
        /// </summary>
        /// <param name="keyName">The name of the key to check (e.g., "Space", "W", "LeftShift")</param>
        /// <returns>True if the key was pressed this frame, false otherwise</returns>
        public bool WasKeyPressedThisFrame(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName)) return false;
            if (Enum.TryParse<Key>(keyName, true, out var key))
            {
                return Keyboard.current != null && Keyboard.current[key].wasPressedThisFrame;
            }
            return false;
        }
        /// <summary>
        /// Checks if a key is currently being held down.
        /// Key names are case-insensitive and should match Unity Input System key names.
        /// </summary>
        /// <param name="keyName">The name of the key to check (e.g., "Space", "W", "LeftShift")</param>
        /// <returns>True if the key is currently pressed, false otherwise</returns>
        public bool IsKeyDown(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName)) return false;
            if (Enum.TryParse<Key>(keyName, true, out var key))
            {
                return Keyboard.current != null && Keyboard.current[key].isPressed;
            }
            return false;
        }


        #endregion

        #region Config
        /// <summary>
        /// Gets a configuration value from the plugin's config file.
        /// Creates the config entry with the default value if it doesn't exist.
        /// </summary>
        /// <param name="section">The config section name</param>
        /// <param name="key">The config key name</param>
        /// <param name="defaultValue">The default value if the key doesn't exist</param>
        /// <returns>The config value as a string</returns>
        public string GetConfigValue(string section, string key, string defaultValue)
        {
            return _plugin.Config.Bind(section, key, defaultValue).Value;
        }

        /// <summary>
        /// Sets a configuration value in the plugin's config file.
        /// The config file is automatically saved after setting the value.
        /// </summary>
        /// <param name="section">The config section name</param>
        /// <param name="key">The config key name</param>
        /// <param name="value">The value to set</param>
        public void SetConfigValue(string section, string key, string value)
        {
            _plugin.Config.Bind(section, key, value).Value = value;
            _plugin.Config.Save();
        }
        
        #endregion

        #region Utils

        /// <summary>
        /// Checks if the game is currently paused.
        /// Game is considered paused when time scale is 0.
        /// </summary>
        /// <returns>True if the game is paused, false otherwise</returns>
        public bool IsGamePaused()
        {
            return Time.timeScale == 0f;
        }
        
        /// <summary>
        /// Sets the game's time scale.
        /// 1.0 = normal speed, 0.5 = half speed, 2.0 = double speed, 0.0 = paused.
        /// </summary>
        /// <param name="scale">The time scale multiplier</param>
        public void SetTimeScale(float scale)
        {
            Time.timeScale = scale;
        }

        /// <summary>
        /// Gets the name of the currently active scene.
        /// </summary>
        /// <returns>The name of the active scene</returns>
        public string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        /// <summary>
        /// Loads a scene by its name.
        /// This will change the current scene and may cause objects to be destroyed.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load</param>
        public void LoadSceneByName(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        
        /// <summary>
        /// Gets the time elapsed since the last frame in seconds.
        /// Useful for frame-rate independent movement and animations.
        /// </summary>
        /// <returns>The delta time in seconds</returns>
        public float GetDeltaTime()
        {
            return Time.deltaTime;
        }
        
        /// <summary>
        /// Gets the current time scale of the game.
        /// </summary>
        /// <returns>The current time scale multiplier</returns>
        public float GetTimeScale()
        {
            return Time.timeScale;
        }
        #endregion
        
        #region GameObject API
        /// <summary>
        /// Find a GameObject by name, using a cache for performance
        /// </summary>
        public SafeGameObject FindGameObject(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            // check if the cached object still exists
            // since it can be destroyed while playing
            if (_gameObjectCache.TryGetValue(name, out var cachedObj))
            {
                if (cachedObj != null)
                {
                    return new SafeGameObject(cachedObj);
                }
                else
                {
                    _gameObjectCache.Remove(name);
                }
            }

            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                _gameObjectCache[name] = obj;
            }
            return obj != null ? new SafeGameObject(obj) : null;
        }

        /// <summary>
        /// Gets a child GameObject by index from a parent SafeGameObject.
        /// </summary>
        /// <param name="parent">The parent SafeGameObject</param>
        /// <param name="index">The index of the child (0-based)</param>
        /// <returns>The child SafeGameObject, or null if index is invalid</returns>
        public SafeGameObject GetChild(SafeGameObject parent, int index)
        {
            if (parent == null || parent.Inner == null) return null;
            if (index < 0 || index >= parent.Inner.transform.childCount) return null;
            return new SafeGameObject(parent.Inner.transform.GetChild(index).gameObject);
        }

        /// <summary>
        /// Gets the number of child GameObjects under a parent SafeGameObject.
        /// </summary>
        /// <param name="parent">The parent SafeGameObject</param>
        /// <returns>The number of children, or 0 if parent is null</returns>
        public int GetChildCount(SafeGameObject parent)
        {
            if (parent == null || parent.Inner == null) return 0;
            return parent.Inner.transform.childCount;
        }

        /// <summary>
        /// Gets the tag of a SafeGameObject.
        /// </summary>
        /// <param name="obj">The SafeGameObject to get the tag from</param>
        /// <returns>The tag string, or empty string if object is null</returns>
        public string GetTag(SafeGameObject obj)
        {
            return obj != null && obj.Inner != null ? obj.Inner.tag : string.Empty;
        }

        /// <summary>
        /// Checks if a SafeGameObject is currently active.
        /// </summary>
        /// <param name="obj">The SafeGameObject to check</param>
        /// <returns>True if the object is active, false otherwise</returns>
        public bool GetActive(SafeGameObject obj)
        {
            return obj != null && obj.Inner != null && obj.Inner.activeSelf;
        }

        /// <summary>
        /// Sets the active state of a SafeGameObject.
        /// </summary>
        /// <param name="obj">The SafeGameObject to modify</param>
        /// <param name="active">True to activate, false to deactivate</param>
        public void SetActive(SafeGameObject obj, bool active)
        {
            if (obj != null && obj.Inner != null) obj.Inner.SetActive(active);
        }

        /// <summary>
        /// Gets the names of all components attached to a SafeGameObject.
        /// Useful for debugging and understanding object structure.
        /// </summary>
        /// <param name="obj">The SafeGameObject to inspect</param>
        /// <returns>Array of component type names</returns>
        public string[] GetComponentTypeNames(SafeGameObject obj)
        {
            if (obj == null || obj.Inner == null) return new string[0];
            var comps = obj.Inner.GetComponents<Component>();
            var names = new List<string>(comps.Length);
            foreach (var c in comps)
            {
                if (c != null) names.Add(c.GetType().Name);
            }
            return names.ToArray();
        }

        /// <summary>
        /// Prints all GameObjects in the current scene to the console.
        /// Limited to 100 objects to prevent console spam.
        /// Useful for debugging and finding object names.
        /// </summary>
        public void PrintAllGameObjects()
        {
            // PERFORMANCE: Limit the number of objects to prevent spam
            var objs = GameObject.FindObjectsOfType<GameObject>();
            int maxObjects = 100; // Limit to prevent console spam
            int count = 0;
            
            foreach (var obj in objs)
            {
                if (count >= maxObjects)
                {
                    Plugin.StaticLogger.LogInfo($"... and {objs.Length - maxObjects} more objects (truncated)");
                    break;
                }
                Plugin.StaticLogger.LogInfo($"GameObject: {obj.name}");
                count++;
            }
            
            Plugin.StaticLogger.LogInfo($"Total GameObjects found: {objs.Length}");
        }
        
        /// <summary>
        /// Gets the world position of a SafeGameObject.
        /// </summary>
        /// <param name="obj">The SafeGameObject to get position from</param>
        /// <returns>The world position as Vector3, or Vector3.zero if object is null</returns>
        public Vector3 GetObjectPosition(SafeGameObject obj)
        {
            if (obj == null || obj.Inner == null) return Vector3.zero;
            return obj.Inner.transform.position;
        }

        /// <summary>
        /// Sets the world position of a SafeGameObject.
        /// </summary>
        /// <param name="obj">The SafeGameObject to modify</param>
        /// <param name="position">The new world position</param>
        public void SetObjectPosition(SafeGameObject obj, Vector3 position)
        {
            if (obj == null || obj.Inner == null) return;
            obj.Inner.transform.position = position;
        }

        /// <summary>
        /// Draws a debug line in the scene view.
        /// Only visible in the Unity editor or when debug drawing is enabled.
        /// </summary>
        /// <param name="start">The start position of the line</param>
        /// <param name="end">The end position of the line</param>
        /// <param name="duration">How long the line should be visible (in seconds)</param>
        public void DrawDebugLine(Vector3 start, Vector3 end, float duration = 1.0f)
        {
            Debug.DrawLine(start, end, Color.white, duration);
        }

        /// <summary>
        /// Creates a new empty GameObject at the specified position.
        /// </summary>
        /// <param name="name">The name for the new GameObject (uses "Empty" if null/empty)</param>
        /// <param name="position">The world position where the object should be created</param>
        /// <returns>A SafeGameObject wrapper for the new GameObject</returns>
        public SafeGameObject CreateEmpty(string name, Vector3 position)
        {
            var go = new GameObject(string.IsNullOrEmpty(name) ? "Empty" : name);
            go.transform.position = position;
            return new SafeGameObject(go);
        }

        /// <summary>
        /// Destroys a SafeGameObject and its underlying GameObject.
        /// The object will be destroyed at the end of the current frame.
        /// </summary>
        /// <param name="obj">The SafeGameObject to destroy</param>
        public void DestroyObject(SafeGameObject obj)
        {
            if (obj != null && obj.Inner != null)
            {
                GameObject.Destroy(obj.Inner);
            }
        }

        /// <summary>
        /// Performs a raycast from the main camera's position in the forward direction.
        /// Useful for detecting what the player is looking at.
        /// </summary>
        /// <param name="maxDistance">The maximum distance to raycast</param>
        /// <returns>A RaycastResult containing hit information</returns>
        public RaycastResult RaycastFromCamera(float maxDistance)
        {
            var result = new RaycastResult { Hit = false };
            if (Camera.main == null) return result;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out var hitInfo, maxDistance))
            {
                result.Hit = true;
                result.Point = hitInfo.point;
                result.Normal = hitInfo.normal;
                result.HitObject = hitInfo.collider != null ? new SafeGameObject(hitInfo.collider.gameObject) : null;
            }
            return result;
        }

        /// <summary>
        /// Gets the full hierarchy path of the player GameObject.
        /// Returns a string like "Root/Environment/Player" showing the object's position in the hierarchy.
        /// </summary>
        /// <returns>The hierarchy path as a string, or empty string if player not found</returns>
        public string GetPlayerPath()
        {
            var player = ResolvePlayerRoot();
            if (player == null) return string.Empty;
            return new SafeGameObject(player).GetPath();
        }


        #endregion
    }
    
}