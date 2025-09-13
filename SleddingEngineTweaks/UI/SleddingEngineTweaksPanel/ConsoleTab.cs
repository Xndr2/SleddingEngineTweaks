using System;
using System.Collections.Generic;
using SleddingEngineTweaks.UI.Options;
using UnityEngine;
using MoonSharp.Interpreter;
using SleddingEngineTweaks.Scripting;

namespace SleddingEngineTweaks.UI.SleddingEngineTweaksPanel
{
    public class ConsoleTab : ModTab, IDynamicSizedTab
    {
        private string inputCommand = "";
        private List<string> commandHistory = new List<string>();
        private Vector2 scrollPosition = Vector2.zero;
        private LuaManager luaManager;
        private bool shouldExecute = false;
        
        
        public ConsoleTab() : base("Console")
        {
            luaManager = LuaManager.Instance;
            luaManager.OnScriptOutput += AppendToHistory;
        }
        
        /// <summary>
        /// Gets the requested size for this tab based on current content
        /// </summary>
        public Vector2? GetRequestedSize()
        {
            // Console tab needs more space for command history
            return new Vector2(400f, 300f); // Minimum size for console
        }

        public override void Render()
        {
            // command history area
            GUILayout.BeginVertical(GUI.skin.box);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
            foreach (string command in commandHistory)
            {
                GUILayout.Label(command);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            
            // input area
            GUILayout.BeginHorizontal();
            
            // Check if the text field has focus to prevent game input
            bool textFieldFocused = GUI.GetNameOfFocusedControl() == "ConsoleInput";
            
            // handle keyboard input
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                shouldExecute = true;
                Event.current.Use();
            }
        
            GUI.SetNextControlName("ConsoleInput");
            inputCommand = GUILayout.TextField(inputCommand);
            
            // Consume all input events when text field is focused
            if (textFieldFocused)
            {
                if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.KeyUp)
                {
                    Event.current.Use();
                }
            }
        
            // Check if we should execute (either from Enter key or button)
            if (shouldExecute || GUILayout.Button("Execute", GUILayout.Width(70)))
            {
                if (!string.IsNullOrEmpty(inputCommand))
                {
                    ExecuteCommand(inputCommand);
                    inputCommand = "";
                }
                shouldExecute = false;
            }
        
            GUILayout.EndHorizontal();

        }
        
        private void ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command)) return;

            AppendToHistory($"> {command}");
            if (command.Trim().ToLower() == "reloadscripts")
            {
                LuaManager.Instance.ReloadAllScripts();
                AppendToHistory("[Console] Reloaded all Lua scripts.");
                return;
            }
            luaManager.ExecuteCommand(command);
        }

        
        private void AppendToHistory(string text)
        {
            commandHistory.Add(text);
            scrollPosition.y = float.MaxValue; // Auto-scroll to bottom
        }

        ~ConsoleTab()
        {
            // Safety: make sure we do not leak subscriptions on reload/dispose
            if (luaManager != null)
            {
                luaManager.OnScriptOutput -= AppendToHistory;
            }
        }

    }
}