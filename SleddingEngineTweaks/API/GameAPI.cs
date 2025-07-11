using BepInEx;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using SleddingEngineTweaks.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
            _plugin.LuaManager.OutputMessage($"[LUA]: {message}");
        }

        public bool RegisterModPanel(string modName)
        {
            // Placeholder for SleddingAPI integration
            Log($"Registered mod panel: {modName}");
            return true;
        }

        #region Mod Panel API
        public bool RegisterModTab(string modName, string tabName)
        {
            // Placeholder for SleddingAPI integration
            Log($"Registered tab '{tabName}' for mod '{modName}'");
            return true;
        }

        public bool RegisterLabelOption(string modName, string tabName, string optionName)
        {
            // Placeholder for SleddingAPI integration
            Log($"Registered label '{optionName}' in tab '{tabName}' for mod '{modName}'");
            return true;
        }

        public bool UpdateLabelOption(string modName, string tabName, string newText)
        {
            // Placeholder for SleddingAPI integration
            Log($"Updating label in tab '{tabName}' for mod '{modName}' to '{newText}'");
            return true;
        }

        public bool RegisterButtonOption(string modName, string tabName, string buttonText, DynValue callback)
        {
            // Placeholder for SleddingAPI integration
            Log($"Registered button '{buttonText}' in tab '{tabName}' for mod '{modName}'");
            // Example of how someone might call the Lua function:
            // _plugin.LuaManager.CallLuaFunction(callback);
            return true;
        }

        public bool RegisterSelectorOption(string modName, string tabName, string selectorText, bool defaultValue, DynValue callback)
        {
            // Placeholder for SleddingAPI integration
            Log($"Registered selector '{selectorText}' in tab '{tabName}' for mod '{modName}'");
            // Example of how someone might call the Lua function:
            // _plugin.LuaManager.CallLuaFunction(callback, newValue);
            return true;
        }
        #endregion

        #region Player API
        public GameObject GetPlayer()
        {
            return GameObject.FindGameObjectWithTag("Player");
        }

        public Vector3 GetPlayerPosition()
        {
            var player = GetPlayer();
            return player != null ? player.transform.position : Vector3.zero;
        }

        public void SetPlayerPosition(Vector3 position)
        {
            var player = GetPlayer();
            if (player != null)
            {
                player.transform.position = position;
            }

        }
        #endregion
        
        #region Input API
        public bool WasKeyPressedThisFrame(string keyName)
        {
            if (Enum.TryParse<Key>(keyName, true, out var key))
            {
                return Keyboard.current != null && Keyboard.current[key].wasPressedThisFrame;
            }
            return false;
        }
        public bool IsKeyDown(string keyName)
        {
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
        #endregion
        
        #region GameObject API
        /// <summary>
        /// Find a GameObject by name, using a cache for performance
        /// </summary>
        public GameObject FindGameObject(string name)
        {
            // check if the cached object still exists
            // since it can be destroyed while playing
            if (_gameObjectCache.TryGetValue(name, out var cachedObj))
            {
                if (cachedObj != null)
                {
                    return cachedObj;
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
            return obj;
        }
        
        public Vector3 GetObjectPosition(GameObject obj)
        {
            if (obj == null) return Vector3.zero;
            return obj.transform.position;
        }

        public void SetObjectPosition(GameObject obj, Vector3 position)
        {
            if (obj == null) return;
            obj.transform.position = position;
        }

        public void DrawDebugLine(Vector3 start, Vector3 end, float duration = 1.0f)
        {
            Debug.DrawLine(start, end, Color.white, duration);
        }


        #endregion
    }
    
}