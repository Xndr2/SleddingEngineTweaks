using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using UnityEngine;
using SleddingEngineTweaks.UI;

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
            Debug.Log($"[Lua] {message}");
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
        
        #region GameObject Methods

        [MoonSharpVisible(true)]
        public GameObject FindGameObject(string name)
        {
            return GameObject.Find(name);
        }
        
        [MoonSharpVisible(true)]
        public Vector3 GetObjectPosition(GameObject obj)
        {
            return obj != null ? obj.transform.position : Vector3.zero;
        }

        [MoonSharpVisible(true)]
        public void SetObjectPosition(GameObject obj, Vector3 position)
        {
            if (obj != null)
                obj.transform.position = position;
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