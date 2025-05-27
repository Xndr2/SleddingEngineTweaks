using System;
using System.Collections.Generic;
using SleddingEngineTweaks.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SleddingEngineTweaks
{
    public class ImGuiController : MonoBehaviour
    {
        internal static ImGuiController Instance { get; private set; }
        
        private bool showUI = true;

        private Dictionary<string, ModPanel> modPanels = new();
        
        internal static void Setup()
        {
            GameObject obj = new GameObject("ImGuiController");
            DontDestroyOnLoad(obj);
            obj.hideFlags = HideFlags.HideAndDontSave;
            Instance = obj.AddComponent<ImGuiController>();
        }
        
        void Start()
        {
            Plugin.StaticLogger.LogInfo($"Controller start");
            
            // // Sample Mod Panels
            AddPanel("SleddingEngineTweaks");
            AddTab("SleddingEngineTweaks", "Main");
            AddOption("SleddingEngineTweaks", "Main", "Option 1", OptionType.Label);
            AddOption("SleddingEngineTweaks", "Main", "Option 2", OptionType.Label);
            AddTab("SleddingEngineTweaks", "Keybinds");
            AddOption("SleddingEngineTweaks", "Keybinds", "Option 4", OptionType.Button);
            
            AddPanel("Test 2");
        }

        private bool IsDown = false;
        void Update()
        {
            if (Keyboard.current.deleteKey.wasPressedThisFrame)
            {
                if (!IsDown)
                {
                    InputDown();
                }
            }

            if (Keyboard.current.deleteKey.wasReleasedThisFrame)
            {
                IsDown = false;
            }
        }

        private void InputDown()
        {
            IsDown = true;
            showUI = !showUI;
        }

        public void AddPanel(string modName)
        {
            ModPanel panel = new ModPanel(modName, new Rect(400, 10, 350, 300));
            modPanels.Add(modName, panel);
        }

        public bool AddTab(string modName, string tabName)
        {
            ModPanel panel = modPanels[modName];
            if(panel == null) return false;
            
            panel.AddTab(tabName);
            return true;
        }

        public void AddOption(string modName, string tabName, string optionName, OptionType optionType)
        {
            ModPanel panel = modPanels[modName];
            panel.AddOption(tabName, optionName, optionType);
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

            foreach (var panel in modPanels.Values)
            {
                panel.Render(panelStyle);
            }
        }
    }
}
