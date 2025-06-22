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
        
        private bool isResizing = false;
        private Rect resizeHandle = new Rect(0, 0, 15, 15);
        private Vector2 lastSize;
        private GUIStyle resizeHandleStyle;


        public ModPanel(string name, Rect rect)
        {
            Name = name;
            rect.width = Mathf.Max(rect.width, MIN_WIDTH);
            rect.height = Mathf.Max(rect.height, MIN_HEIGHT);
            WindowRect = rect;
            lastPosition = new Vector2(rect.x, rect.y);
            lastSize = new Vector2(rect.width, rect.height);
            
            // Create resize handle style
            resizeHandleStyle = new GUIStyle();
            resizeHandleStyle.normal.textColor = Color.white;
            resizeHandleStyle.alignment = TextAnchor.LowerRight;
            resizeHandleStyle.fontSize = 12;
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
        
        public SleddingAPIStatus AddTab(ModTab tab)
        {
            foreach (ModTab tab2 in tabs)
            {
                if (tab2.GetName() == tab.GetName())
                    return SleddingAPIStatus.ModTabAlreadyRegistered;
            }
            tabs.Add(tab);
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

            // Add position and size saving
            Vector2 currentPosition = new Vector2(WindowRect.x, WindowRect.y);
            Vector2 currentSize = new Vector2(WindowRect.width, WindowRect.height);

            if (Vector2.Distance(lastPosition, currentPosition) > 1f || 
                Vector2.Distance(lastSize, currentSize) > 1f)
            {
                lastPosition = currentPosition;
                lastSize = currentSize;
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
            GUI.DragWindow(new Rect(0, 0, WindowRect.width - 20, 20));

            if (Collapsed)
            {
                WindowRect.height = HEADER_HEIGHT; 
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
            
            // draw resize handle
            resizeHandle.x = WindowRect.width - 15;
            resizeHandle.y = WindowRect.height - 15;
            GUI.Box(resizeHandle, "◢", resizeHandleStyle);
            
            // handle resizing
            HandleResize();
        }

        private void HandleResize()
        {
            Event e = Event.current;
            Vector2 mousePos = e.mousePosition;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (resizeHandle.Contains(mousePos))
                    {
                        isResizing = true;
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    isResizing = false;
                    break;

                case EventType.MouseDrag:
                    if (isResizing)
                    {
                        WindowRect.width = Mathf.Max(MIN_WIDTH, mousePos.x + 5);
                        WindowRect.height = Mathf.Max(MIN_HEIGHT, mousePos.y + 5);
                        e.Use();

                        // Save size if it has changed significantly
                        Vector2 currentSize = new Vector2(WindowRect.width, WindowRect.height);
                        if (Vector2.Distance(lastSize, currentSize) > 1f)
                        {
                            lastSize = currentSize;
                            Plugin.SavePanelPosition(Name, WindowRect);
                        }
                    }
                    break;
            }
            
            // Change cursor (this is the non-editor way to show resize cursor)
            if (resizeHandle.Contains(mousePos))
            {
                Cursor.visible = true;
                // Note: Unity doesn't have a built-in resize cursor in runtime
                // We could optionally set a custom cursor texture here if desired
            }
        }
    }
}
