using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using SleddingEngineTweaks.API;
using SleddingEngineTweaks.UI.Options.Base;
using SleddingEngineTweaks.UI.SleddingEngineTweaksPanel;

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
        
        // Custom panel sizing
        public Vector2? CustomMinSize { get; set; } = null;
        public Vector2? CustomRecommendedSize { get; set; } = null;

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
            var minSize = GetMinimumSize();
            rect.width = Mathf.Max(rect.width, minSize.x);
            rect.height = Mathf.Max(rect.height, minSize.y);
            WindowRect = rect;
            lastPosition = new Vector2(rect.x, rect.y);
            lastSize = new Vector2(rect.width, rect.height);
            
            // Create resize handle style
            resizeHandleStyle = new GUIStyle();
            resizeHandleStyle.normal.textColor = Color.white;
            resizeHandleStyle.alignment = TextAnchor.LowerRight;
            resizeHandleStyle.fontSize = 12;
        }
        
        /// <summary>
        /// Creates a ModPanel with custom minimum and recommended sizes
        /// </summary>
        /// <param name="name">Panel name</param>
        /// <param name="rect">Initial position and size</param>
        /// <param name="customMinSize">Custom minimum size (optional)</param>
        /// <param name="customRecommendedSize">Custom recommended size (optional)</param>
        public ModPanel(string name, Rect rect, Vector2? customMinSize = null, Vector2? customRecommendedSize = null)
        {
            Name = name;
            CustomMinSize = customMinSize;
            CustomRecommendedSize = customRecommendedSize;
            
            var minSize = GetMinimumSize();
            rect.width = Mathf.Max(rect.width, minSize.x);
            rect.height = Mathf.Max(rect.height, minSize.y);
            WindowRect = rect;
            lastPosition = new Vector2(rect.x, rect.y);
            lastSize = new Vector2(rect.width, rect.height);
            
            // Create resize handle style
            resizeHandleStyle = new GUIStyle();
            resizeHandleStyle.normal.textColor = Color.white;
            resizeHandleStyle.alignment = TextAnchor.LowerRight;
            resizeHandleStyle.fontSize = 12;
        }
        
        /// <summary>
        /// Gets the minimum size for this panel
        /// </summary>
        private Vector2 GetMinimumSize()
        {
            // Use custom minimum size if specified
            if (CustomMinSize.HasValue)
            {
                return CustomMinSize.Value;
            }
            
            // Default minimum size
            return new Vector2(MIN_WIDTH, MIN_HEIGHT);
        }
        
        /// <summary>
        /// Gets the recommended size for this panel based on current content
        /// </summary>
        private Vector2 GetRecommendedSize()
        {
            var minSize = GetMinimumSize();
            
            // Use custom recommended size if specified
            if (CustomRecommendedSize.HasValue)
            {
                return CustomRecommendedSize.Value;
            }
            
            // Check if current tab has requested a specific size
            if (currentTab >= 0 && currentTab < tabs.Count)
            {
                var currentTabObj = tabs[currentTab];
                if (currentTabObj is IDynamicSizedTab dynamicTab)
                {
                    var requestedSize = dynamicTab.GetRequestedSize();
                    if (requestedSize.HasValue)
                    {
                        // Use the requested size as the minimum for this tab
                        return new Vector2(
                            Mathf.Max(minSize.x, requestedSize.Value.x),
                            Mathf.Max(minSize.y, requestedSize.Value.y)
                        );
                    }
                }
            }
            
            return minSize;
        }
        
        /// <summary>
        /// Requests the panel to resize based on current content
        /// </summary>
        public void RequestResize()
        {
            AutoResizeBasedOnContent();
        }
        
        /// <summary>
        /// Automatically resizes the panel based on current content
        /// </summary>
        private void AutoResizeBasedOnContent()
        {
            var recommendedSize = GetRecommendedSize();
            var minSize = GetMinimumSize();
            
            // Only auto-resize if the current size is smaller than recommended
            if (WindowRect.height < recommendedSize.y)
            {
                WindowRect.height = recommendedSize.y;
                // Save the new size
                Plugin.SavePanelPosition(Name, WindowRect);
            }
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

        public bool RemoveTab(string tabName)
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                if (tabs[i].GetName() == tabName)
                {
                    tabs.RemoveAt(i);
                    if (currentTab >= tabs.Count)
                    {
                        currentTab = Mathf.Max(0, tabs.Count - 1);
                    }
                    return true;
                }
            }
            return false;
        }

        public ModTab GetTab(string tabName)
        {
            foreach (var tab in tabs)
            {
                if (tab.GetName() == tabName) return tab;
            }
            return null;
        }

        public SleddingAPIStatus AddOption(string tabName, string optionId, string name, OptionType optionType)
        {
            foreach (ModTab tab in tabs)
            {
                if (tab.GetName() == tabName)
                {
                    tab.AddOption(optionId, name, optionType);
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
        
        public SleddingAPIStatus UpdateOption(string tabName, UpdateOptionRequest request)
        {
            foreach (ModTab tab in tabs)
            {
                if (tab.GetName() == tabName)
                {
                    tab.UpdateOption(request);
                    return SleddingAPIStatus.Ok;
                }
            }
            return SleddingAPIStatus.ModTabNotFound;
        }

        public void Render(GUIStyle style)
        {
            WindowRect = GUILayout.Window(Name.GetHashCode(), WindowRect, DrawWindow, "", style);

            // Enforce minimum size
            var minSize = GetMinimumSize();
            WindowRect.width = Mathf.Max(WindowRect.width, minSize.x);
            WindowRect.height = Mathf.Max(WindowRect.height, Collapsed ? HEADER_HEIGHT : minSize.y);

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

            // Tabs (only show if more than 1 tab)
            if (tabs.Count > 1)
            {
                GUILayout.BeginHorizontal();
                for (int i = 0; i < tabs.Count; i++)
                {
                    if (GUILayout.Toggle(currentTab == i, tabs[i].GetName(), GUI.skin.button))
                    {
                        if (currentTab != i)
                        {
                            // Tab switched - reset height to minimum
                            currentTab = i;
                            var minSize = GetMinimumSize();
                            WindowRect.height = minSize.y;
                            // Save the new size immediately
                            Plugin.SavePanelPosition(Name, WindowRect);
                        }
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }

            // Render options for current tab
            if (currentTab >= 0 && currentTab < tabs.Count)
            {
                tabs[currentTab].Render();
                
                // Auto-resize based on content if needed
                AutoResizeBasedOnContent();
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
                        var minSize = GetMinimumSize();
                        WindowRect.width = Mathf.Max(minSize.x, mousePos.x + 5);
                        WindowRect.height = Mathf.Max(minSize.y, mousePos.y + 5);
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
