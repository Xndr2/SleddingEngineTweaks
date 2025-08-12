using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SleddingEngineTweaks.Scripting
{
    public class LuaEventHandler : MonoBehaviour
    {
        internal static LuaEventHandler Instance { get; private set; }
        
        private float _secondTimer = 0f;
        
        internal void Setup()
        {
            GameObject obj = new GameObject("SceneEventHandler");
            DontDestroyOnLoad(obj);
            obj.hideFlags = HideFlags.HideAndDontSave;
            Instance = obj.AddComponent<LuaEventHandler>();
        }
        
        void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void Update()
        {
            try
            {
                LuaManager.Instance.CallLuaFunction("OnUpdate", Time.deltaTime);
            }
            catch (Exception ex)
            {
                Plugin.StaticLogger.LogError($"Lua OnUpdate error: {ex.Message}");
            }
            
            _secondTimer += Time.deltaTime;
            if (_secondTimer >= 1.0f)
            {
                try
                {
                    LuaManager.Instance.CallLuaFunction("OnUpdateSecond");
                    // Update debug labels
                    var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                    var path = Plugin.GameAPI.GetPlayerPath();
                    var pos = Plugin.GameAPI.GetPlayerPosition();
                    Plugin.SleddingAPI.UpdateOption(MyPluginInfo.PLUGIN_NAME, "Debug", new UI.Options.Base.UpdateOptionRequest { OptionId = "dbg_scene_label", NewName = $"Scene: {scene}" });
                    Plugin.SleddingAPI.UpdateOption(MyPluginInfo.PLUGIN_NAME, "Debug", new UI.Options.Base.UpdateOptionRequest { OptionId = "dbg_player_path_label", NewName = $"Player Path: {path}" });
                    Plugin.SleddingAPI.UpdateOption(MyPluginInfo.PLUGIN_NAME, "Debug", new UI.Options.Base.UpdateOptionRequest { OptionId = "dbg_player_pos_label", NewName = $"Player Pos: {pos.x:F2}, {pos.y:F2}, {pos.z:F2}" });
                }
                catch (Exception ex)
                {
                    Plugin.StaticLogger.LogError($"Lua OnUpdateSecond error: {ex.Message}");
                }
                _secondTimer = 0f;
            }
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            try
            {
                LuaManager.Instance.CallLuaFunction("OnSceneLoaded", scene.name);
            }
            catch (Exception ex)
            {
                Plugin.StaticLogger.LogError($"Lua OnSceneLoaded error: {ex.Message}");
            }
        }
    }
}