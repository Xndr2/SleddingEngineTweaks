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
        private Vector2 lastPosition;

        public const float MIN_WIDTH = 250f;
        public const float MIN_HEIGHT = 150f;
        public const float HEADER_HEIGHT = 50f;


        public ModPanel(string name, Rect rect)
        {
            Name = name;
            rect.width = Mathf.Max(rect.width, MIN_WIDTH);
            rect.height = Mathf.Max(rect.height, MIN_HEIGHT);
            WindowRect = rect;
            lastPosition = new Vector2(rect.x, rect.y);
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
        
        public SleddingAPIStatus UpdateOption(string tabName, string newText, OptionType optionType)
        {
            foreach (ModTab tab in tabs)
            {
                if (tab.GetName() == tabName)
                {
                    tab.UpdateOption(newText, optionType);
                    return SleddingAPIStatus.Ok;
                }
            }
            return SleddingAPIStatus.ModTabNotFound;
        }

        public void Render(GUIStyle style)
        {
            WindowRect = GUILayout.Window(Name.GetHashCode(), WindowRect, DrawWindow, "", style);

            // Enforce minimum size
            WindowRect.width = Mathf.Max(WindowRect.width, MIN_WIDTH);
            WindowRect.height = Mathf.Max(WindowRect.height, Collapsed ? HEADER_HEIGHT : MIN_HEIGHT);

            // Add position saving
            Vector2 currentPosition = new Vector2(WindowRect.x, WindowRect.y);
            if (Vector2.Distance(lastPosition, currentPosition) > 1f)
            {
                lastPosition = currentPosition;
                Plugin.SavePanelPosition(Name, WindowRect);
            }


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
