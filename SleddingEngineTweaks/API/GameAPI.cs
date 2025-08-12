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
    public class GameAPI : IDisposable
    {
        private readonly Plugin _plugin;
        private readonly Dictionary<string, GameObject> _gameObjectCache = new Dictionary<string, GameObject>();

        public GameAPI(Plugin plugin)
        {
            _plugin = plugin;
            // sub to scene changes to clear the cache
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Clean up the event subs when the API is no longer needed
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
            // Try known explicit paths first
            foreach (var path in KnownPlayerPaths)
            {
                var goByPath = GameObject.Find(path);
                if (goByPath != null) return goByPath;
            }

            // Fallbacks: by tag, by name
            var byTag = GameObject.FindGameObjectWithTag("Player");
            if (byTag != null) return byTag;

            var byName = GameObject.Find("Player");
            if (byName != null) return byName;

            return null;
        }

        public SafeGameObject GetPlayer()
        {
            var go = ResolvePlayerRoot();
            return go != null ? new SafeGameObject(go) : null;
        }

        public Vector3 GetPlayerPosition()
        {
            var player = ResolvePlayerRoot();
            return player != null ? player.transform.position : Vector3.zero;
        }

        public void SetPlayerPosition(Vector3 position)
        {
            var player = ResolvePlayerRoot();
            if (player != null)
            {
                player.transform.position = position;
            }

        }

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

        public void MovePlayer(Vector3 worldDelta)
        {
            var player = ResolvePlayerRoot();
            if (player == null) return;
            player.transform.position += worldDelta;
        }

        public void MovePlayerForward(float distance)
        {
            var player = ResolvePlayerRoot();
            if (player == null) return;
            var forward = player.transform.forward;
            player.transform.position += forward * distance;
        }

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
        public Camera GetMainCamera()
        {
            return Camera.main;
        }

        public Vector3 GetCameraPosition()
        {
            return Camera.main != null ? Camera.main.transform.position : Vector3.zero;
        }

        public void SetCameraPosition(Vector3 position)
        {
            if (Camera.main != null)
            {
                Camera.main.transform.position = position;
            }
        }
        #endregion
        
        #region Input API
        public bool WasKeyPressedThisFrame(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName)) return false;
            if (Enum.TryParse<Key>(keyName, true, out var key))
            {
                return Keyboard.current != null && Keyboard.current[key].wasPressedThisFrame;
            }
            return false;
        }
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
        public string GetConfigValue(string section, string key, string defaultValue)
        {
            return _plugin.Config.Bind(section, key, defaultValue).Value;
        }

        public void SetConfigValue(string section, string key, string value)
        {
            _plugin.Config.Bind(section, key, value).Value = value;
            _plugin.Config.Save();
        }
        
        #endregion

        #region Utils

        public bool IsGamePaused()
        {
            return Time.timeScale == 0f;
        }
        
        public void SetTimeScale(float scale)
        {
            Time.timeScale = scale;
        }

        public string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        public void LoadSceneByName(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        
        public float GetDeltaTime()
        {
            return Time.deltaTime;
        }
        
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

        public SafeGameObject GetChild(SafeGameObject parent, int index)
        {
            if (parent == null || parent.Inner == null) return null;
            if (index < 0 || index >= parent.Inner.transform.childCount) return null;
            return new SafeGameObject(parent.Inner.transform.GetChild(index).gameObject);
        }

        public int GetChildCount(SafeGameObject parent)
        {
            if (parent == null || parent.Inner == null) return 0;
            return parent.Inner.transform.childCount;
        }

        public string GetTag(SafeGameObject obj)
        {
            return obj != null && obj.Inner != null ? obj.Inner.tag : string.Empty;
        }

        public bool GetActive(SafeGameObject obj)
        {
            return obj != null && obj.Inner != null && obj.Inner.activeSelf;
        }

        public void SetActive(SafeGameObject obj, bool active)
        {
            if (obj != null && obj.Inner != null) obj.Inner.SetActive(active);
        }

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

        public void PrintAllGameObjects()
        {
            var objs = GameObject.FindObjectsOfType<GameObject>();
            foreach (var obj in objs)
            {
                Plugin.StaticLogger.LogInfo($"GameObject: {obj.name}");
            }
        }
        
        public Vector3 GetObjectPosition(SafeGameObject obj)
        {
            if (obj == null || obj.Inner == null) return Vector3.zero;
            return obj.Inner.transform.position;
        }

        public void SetObjectPosition(SafeGameObject obj, Vector3 position)
        {
            if (obj == null || obj.Inner == null) return;
            obj.Inner.transform.position = position;
        }

        public void DrawDebugLine(Vector3 start, Vector3 end, float duration = 1.0f)
        {
            Debug.DrawLine(start, end, Color.white, duration);
        }

        public SafeGameObject CreateEmpty(string name, Vector3 position)
        {
            var go = new GameObject(string.IsNullOrEmpty(name) ? "Empty" : name);
            go.transform.position = position;
            return new SafeGameObject(go);
        }

        public void DestroyObject(SafeGameObject obj)
        {
            if (obj != null && obj.Inner != null)
            {
                GameObject.Destroy(obj.Inner);
            }
        }

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

        public string GetPlayerPath()
        {
            var player = ResolvePlayerRoot();
            if (player == null) return string.Empty;
            return new SafeGameObject(player).GetPath();
        }


        #endregion
    }
    
}