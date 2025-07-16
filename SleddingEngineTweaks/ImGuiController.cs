using SleddingEngineTweaks.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using BepInEx.Configuration;
using SleddingEngineTweaks.API;

namespace SleddingEngineTweaks
{
    public class ImGuiController : MonoBehaviour
    {
        internal static ImGuiController Instance { get; private set; }
        
        private bool showUI = Plugin.ShowOnStart.Value;

        public GameObject prefab; // we set this at runtime
        
        internal void Setup()
        {
            if (prefab != null)
            {
                
                GameObject prefabObj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                DontDestroyOnLoad(prefabObj);
                prefabObj.hideFlags = HideFlags.HideAndDontSave;
            }
            else
            {
                Plugin.StaticLogger.LogError("ImGuiController prefab not set!");
            }
            
            GameObject obj = new GameObject("ImGuiController");
            DontDestroyOnLoad(obj);
            obj.hideFlags = HideFlags.HideAndDontSave;
            Instance = obj.AddComponent<ImGuiController>();
        }
        
        void Start()
        {
            Plugin.StaticLogger.LogInfo($"Controller start");
        }

        private bool IsDown = false;
        void Update()
        {
            if (Keyboard.current[Plugin.MasterKey.Value].wasPressedThisFrame)
            {
                if (!IsDown)
                {
                    InputDown();
                }
            }

            if (Keyboard.current[Plugin.MasterKey.Value].wasReleasedThisFrame)
            {
                IsDown = false;
            }
        }

        private void InputDown()
        {
            IsDown = true;
            showUI = !showUI;
        }

        

        private void OnGUI()
        {
            if (!showUI) return;

            GUI.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
            GUI.contentColor = Color.white;
            GUIStyle panelStyle = new GUIStyle(GUI.skin.window)
            {
                padding = new RectOffset(10, 10, 20, 10),
                fontSize = 14
            };

            foreach (var panel in Plugin.SleddingAPI.GetAllModPanels().Values)
            {
                panel.Render(panelStyle);
            }
        }
    }
}
