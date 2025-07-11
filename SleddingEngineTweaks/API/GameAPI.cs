using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using UnityEngine;
using SleddingEngineTweaks.UI;
using SleddingEngineTweaks.UI.Options.Base;
using System;
using UnityEngine.InputSystem;

namespace SleddingEngineTweaks.API
{
    [MoonSharpUserData]
    public class GameAPI
    {
        private readonly Plugin _plugin;

        public GameAPI(Plugin plugin)
        {
            _plugin = plugin;
        }

        [MoonSharpVisible(true)]
        public void Log(string message)
        {
            Plugin.StaticLogger.LogInfo($"[Lua] {message}");
        }
        
        #region UI Methods
        
        [MoonSharpVisible(true)]
        public bool RegisterModPanel(string modName)
        {
            return SleddingAPI.RegisterModPanel(modName) == SleddingAPIStatus.Ok;
        }
        
        [MoonSharpVisible(true)]
        public bool RegisterModTab(string modName, string tabName)
        {
            return SleddingAPI.RegisterModTab(modName, tabName) == SleddingAPIStatus.Ok;
        }
        
        [MoonSharpVisible(true)]
        public bool RegisterLabelOption(string modName, string tabName, string optionName)
        {
            return SleddingAPI.RegisterOption(modName, tabName, optionName, OptionType.Label) == SleddingAPIStatus.Ok;
        }
        
        [MoonSharpVisible(true)]
        public bool UpdateLabelOption(string modName, string tabName, string newText)
        {
            return SleddingAPI.UpdateOption(modName, tabName, newText, OptionType.Label) == SleddingAPIStatus.Ok;
        }

        [MoonSharpVisible(true)]
        public bool RegisterButtonOption(string modName, string tabName, string buttonText, DynValue callback)
        {
            if (callback == null || callback.Type != DataType.Function)
            {
                Log("RegisterButtonOption requires a valid function callback.");
                return false;
            }
            
            var button = new ModOption_Button(buttonText);
            button.Clicked += () =>
            {
                try
                {
                    callback.Function.Call();
                }
                catch (ScriptRuntimeException ex)
                {
                    Log($"Error in button callback: {ex.DecoratedMessage}");
                }

            };
            
            return SleddingAPI.RegisterOption(modName, tabName, button) == SleddingAPIStatus.Ok;
        }
        
        [MoonSharpVisible(true)]
        public bool RegisterSelectorOption(string modName, string tabName, string selectorText, bool defaultValue, DynValue callback)
        {
            if (callback == null || callback.Type != DataType.Function)
            {
                Log("RegisterSelectorOption requires a valid function callback.");
                return false;
            }

            var selector = new ModOption_Selector(selectorText, defaultValue);
            selector.ValueChanged += (newValue) =>
            {
                try
                {
                    callback.Function.Call(newValue);
                }
                catch (ScriptRuntimeException ex)
                {
                    Log($"Error in selector callback: {ex.DecoratedMessage}");
                }

            };

            return SleddingAPI.RegisterOption(modName, tabName, selector) == SleddingAPIStatus.Ok;
        }
        
        #endregion
        
        #region Player Methods

        [MoonSharpVisible(true)]
        public GameObject GetPlayer()
        {
            // Assuming the player GameObject is tagged with "Player"
            return GameObject.FindGameObjectWithTag("Player");
        }

        [MoonSharpVisible(true)]
        public Vector3 GetPlayerPosition()
        {
            var player = GetPlayer();
            return player != null ? player.transform.position : Vector3.zero;
        }

        [MoonSharpVisible(true)]
        public void SetPlayerPosition(Vector3 position)
        {
            var player = GetPlayer();
            if (player != null)
            {
                player.transform.position = position;
            }
        }

        #endregion

        #region Input Methods

        [MoonSharpVisible(true)]
        public bool WasKeyPressedThisFrame(string keyName)
        {
            if (Keyboard.current != null && Enum.TryParse<Key>(keyName, true, out var key))
            {
                return Keyboard.current[key].wasPressedThisFrame;
            }
            return false;
        }
        
        [MoonSharpVisible(true)]
        public bool IsKeyDown(string keyName)
        {
            if (Keyboard.current != null && Enum.TryParse<Key>(keyName, true, out var key))
            {
                return Keyboard.current[key].isPressed;
            }
            return false;
        }

        #endregion

        #region Configuration Methods

        [MoonSharpVisible(true)]
        public string GetConfigValue(string section, string key, string defaultValue)
        {
            var configEntry = _plugin.Config.Bind(section, key, defaultValue);
            return configEntry.Value;
        }

        [MoonSharpVisible(true)]
        public void SetConfigValue(string section, string key, string value)
        {
            var configEntry = _plugin.Config.Bind(section, key, "");
            configEntry.Value = value;
            _plugin.Config.Save();
        }

        #endregion
        
        #region Game State Methods

        [MoonSharpVisible(true)]
        public bool IsGamePaused()
        {
            return Time.timeScale == 0;
        }
        
        [MoonSharpVisible(true)]
        public void SetTimeScale(float scale)
        {
            Time.timeScale = Mathf.Clamp(scale, 0f, 10f);
        }
        
        #endregion
        
        #region Scene Management Methods

        [MoonSharpVisible(true)]
        public string GetCurrentSceneName()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            return scene.IsValid() ? scene.name : "Unknown";
        }
        
        #endregion
        
<<<<<<< Updated upstream
        #region GameObject Methods

        [MoonSharpVisible(true)]
        public GameObject FindGameObject(string name)
        {
            return GameObject.Find(name);
=======
        #region GameObject API
        public GameObject FindGameObject(string name)
        {
            // check if the cached object still exists
            // since it can be destroyed while playing
            if (_gameObjectCache.TryGetValue(name, out var cahcedObj))
            {
                if (cahcedObj != null)
                {
                    return cahcedObj;
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
>>>>>>> Stashed changes
        }
        
        [MoonSharpVisible(true)]
        public Vector3 GetObjectPosition(GameObject obj)
        {
<<<<<<< Updated upstream
            return obj != null ? obj.transform.position : Vector3.zero;
=======
            if (obj == null) return Vector3.zero;
            return obj.transform.position;
>>>>>>> Stashed changes
        }

        [MoonSharpVisible(true)]
        public void SetObjectPosition(GameObject obj, Vector3 position)
        {
<<<<<<< Updated upstream
            if (obj != null)
                obj.transform.position = position;
=======
            if (obj == null) return;
            obj.transform.position = position;
>>>>>>> Stashed changes
        }
        
        #endregion
        
        #region Debug Methods

        [MoonSharpVisible(true)]
        public void DrawDebugLine(Vector3 start, Vector3 end, float duration = 1.0f)
        {
            Debug.DrawLine(start, end, Color.yellow, duration);
        }

        #endregion

        // TODO:
        // Add more methods here that need to be exposed to Lua scripts
    }
}