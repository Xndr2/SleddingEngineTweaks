using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SleddingEngineTweaks.Scripting
{
    public class LuaEventHandler : MonoBehaviour
    {
        internal static LuaEventHandler Instance { get; private set; }
        
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
            LuaManager.Instance.CallLuaFunction("OnUpdate", Time.deltaTime);
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LuaManager.Instance.CallLuaFunction("OnSceneLoaded", scene.name);
        }
    }
}