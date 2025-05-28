using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using SleddingEngineTweaks.API;


namespace SleddingEngineTweaks.UI
{
    public class ModPanel
    {
        public string Name;
        public Rect WindowRect;
        public bool Collapsed = false;
        private List<ModTab> tabs = new();
        private int currentTab = 0;

        public ModPanel(string name, Rect rect)
        {
            Name = name;
            WindowRect = rect;
        }

        
        public SleddingAPIStatus AddTab(string tabName)
        {
            // check if the tab already exists
            foreach (ModTab tab in tabs)
            {
                if (tab.GetName() == tabName)
                    return SleddingAPIStatus.ModTabAlreadyRegistered;
            }
            // add the tab if none exists
            ModTab newTab = new ModTab(tabName);
            tabs.Add(newTab);
            return SleddingAPIStatus.Ok;
        }

        public SleddingAPIStatus AddOption(string tabName, string optionName, OptionType optionType)
        {
            // find the correct tab from the list, and add an option to it
            foreach (ModTab tab in tabs)
            {
                if (tab.GetName() == tabName)
                {
                    tab.AddOption(optionName, optionType);
                    return SleddingAPIStatus.Ok;
                }
            }
            return SleddingAPIStatus.ModTabNotFound;
        }

        public SleddingAPIStatus AddOption(string tabName, ModOption option)
        {
            // find the correct tab from the list, and add an option to it
            foreach (ModTab tab in tabs)
            {
                if (tab.GetName() == tabName)
                {
                    tab.AddOption(option);
                    return SleddingAPIStatus.Ok;
                }
            }
            return SleddingAPIStatus.ModTabNotFound;
        }

        public void Render(GUIStyle style)
        {
            WindowRect = GUI.Window(Name.GetHashCode(), WindowRect, DrawWindow, Name, style);
        }

        private void DrawWindow(int id)
        {
            GUI.DragWindow(new Rect(0, 0, WindowRect.width - 60, 20));
            
            // collapsing header
            var button = GUI.skin.button;
            button.fontSize = 16;
            if (GUI.Button(new Rect(WindowRect.width - 50, 0, 40, 20), Collapsed ? "▼" : "▲", button))
            {
                Collapsed = !Collapsed;
            }

            if (!Collapsed)
            {
                GUILayout.BeginVertical();
                
                GUILayout.Space(10);
                
                // tabs names
                GUILayout.BeginHorizontal();
                foreach (var tab in tabs)
                {
                    if (GUILayout.Toggle(currentTab == tabs.IndexOf(tab), tab.GetName(), GUI.skin.button))
                        currentTab = tabs.IndexOf(tab);
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(10);
                
                // options for current tab
                foreach (var tab in tabs)
                {
                    if (currentTab == tabs.IndexOf(tab))
                        tab.Render();
                }
                
                GUILayout.EndVertical();
            }
        }
    }
}