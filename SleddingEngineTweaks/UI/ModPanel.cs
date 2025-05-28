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
            foreach (ModTab tab in tabs)
            {
                if (tab.GetName() == tabName)
                    return SleddingAPIStatus.ModTabAlreadyRegistered;
            }
            ModTab newTab = new ModTab(tabName);
            tabs.Add(newTab);
            return SleddingAPIStatus.Ok;
        }

        public SleddingAPIStatus AddOption(string tabName, string optionName, OptionType optionType)
        {
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
            WindowRect = GUILayout.Window(Name.GetHashCode(), WindowRect, DrawWindow, "", style);

            // Optional: enforce minimum size
            // WindowRect.width = Mathf.Max(WindowRect.width, 250f);
            // WindowRect.height = Mathf.Max(WindowRect.height, 100f);
        }

        private void DrawWindow(int id)
        {
            // Header: Name + Collapse Button
            GUILayout.BeginHorizontal();
            GUILayout.Label(Name, GUILayout.ExpandWidth(true));
            if (GUILayout.Button(Collapsed ? "▼" : "▲", GUILayout.Width(40)))
            {
                Collapsed = !Collapsed;
            }
            GUILayout.EndHorizontal();

            // Allow dragging from the top bar
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            if (Collapsed)
            {
                WindowRect.height = 50; // Just header height
                return;
            }

            GUILayout.Space(5);

            // Tabs
            if (tabs.Count > 0)
            {
                GUILayout.BeginHorizontal();
                for (int i = 0; i < tabs.Count; i++)
                {
                    if (GUILayout.Toggle(currentTab == i, tabs[i].GetName(), GUI.skin.button))
                    {
                        currentTab = i;
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }

            // Render options for current tab
            if (currentTab >= 0 && currentTab < tabs.Count)
            {
                tabs[currentTab].Render();
            }
        }
    }
}
