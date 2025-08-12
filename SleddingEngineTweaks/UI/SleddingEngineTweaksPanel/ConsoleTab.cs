using System;
using System.Collections.Generic;
using SleddingEngineTweaks.UI.Options;
using UnityEngine;
using MoonSharp.Interpreter;
using SleddingEngineTweaks.Scripting;

namespace SleddingEngineTweaks.UI.SleddingEngineTweaksPanel
{
    public class ConsoleTab : ModTab
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
            
            // handle keyboard input
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                shouldExecute = true;
                Event.current.Use();
            }
        
            inputCommand = GUILayout.TextField(inputCommand);
        
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