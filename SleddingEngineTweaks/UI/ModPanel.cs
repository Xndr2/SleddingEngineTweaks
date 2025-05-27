using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


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

        
        public void AddTab(string tabName)
        {
            ModTab tab = new ModTab(tabName);
            tabs.Add(tab);
        }

        public void AddOption(string tabName, string optionName, OptionType optionType)
        {
            foreach (ModTab tab in tabs)
            {
                if (tab.GetName() == tabName) tab.AddOption(optionName, optionType);
            }
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