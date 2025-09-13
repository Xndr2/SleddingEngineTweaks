using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using SleddingEngineTweaks.UI;
using SleddingEngineTweaks.UI.Options.Base;
using UnityEngine;

namespace SleddingEngineTweaks.API
{
    /// <summary>
    /// Status codes returned by SleddingAPI operations.
    /// Used to indicate success or failure of API calls.
    /// </summary>
    public enum SleddingAPIStatus
    {
        /// <summary>Operation completed successfully</summary>
        Ok,
        /// <summary>An unknown error occurred</summary>
        UnknownError,
        /// <summary>The specified mod panel was not found</summary>
        ModPanelNotFound,
        /// <summary>A mod panel with this name is already registered</summary>
        ModPanelAlreadyRegistered,
        /// <summary>The specified mod tab was not found</summary>
        ModTabNotFound,
        /// <summary>A mod tab with this name is already registered</summary>
        ModTabAlreadyRegistered,
        /// <summary>One or more arguments were invalid (null, empty, etc.)</summary>
        InvalidArgument,
    }
    
    /// <summary>
    /// Main API class for creating and managing mod UI panels, tabs, and options.
    /// Provides a comprehensive system for modders to create custom user interfaces
    /// with panels, tabs, buttons, labels, and selectors that integrate with the game's UI.
    /// All UI elements are automatically positioned and managed by the framework.
    /// </summary>
    public class SleddingAPI : IDisposable
    {
        private readonly Plugin _plugin;
        private static Dictionary<string, ModPanel> modPanels = new();
        private static float nextPanelX = 20f;
        private const float PANEL_SPACING = 20f;

        /// <summary>
        /// Initializes a new instance of the SleddingAPI.
        /// </summary>
        /// <param name="plugin">The plugin instance that owns this API</param>
        public SleddingAPI(Plugin plugin)
        {
            _plugin = plugin;
        }

        /// <summary>
        /// Disposes of the SleddingAPI.
        /// Currently does nothing but provided for future cleanup needs.
        /// </summary>
        public void Dispose()
        {
            // we do nothing at the moment
        }
        
        /// <summary>
        /// Registers a new mod panel with the specified name.
        /// The panel will be automatically positioned and can be moved/resized by the user.
        /// Panel positions are saved and restored between game sessions.
        /// </summary>
        /// <param name="modName">Unique name for the mod panel</param>
        /// <returns>SleddingAPIStatus indicating success or failure</returns>
        /// <remarks>
        /// Panel names must be unique. If a panel with the same name exists, registration will fail.
        /// </remarks>
        public SleddingAPIStatus RegisterModPanel(string modName)
        {
            if (string.IsNullOrWhiteSpace(modName)) return SleddingAPIStatus.InvalidArgument;
            if (GetModPanel(modName) != null) return SleddingAPIStatus.ModPanelAlreadyRegistered;
            
            // Load saved position and size or use defaults
            Rect savedRect = Plugin.LoadPanelRect(modName);
            if (savedRect.x < 0)
            {
                savedRect.x = nextPanelX;
                savedRect.y = 20f;
            }
    
            var panel = new ModPanel(modName, savedRect);
            modPanels.Add(modName, panel);
    
            // Only update nextPanelX if we're not using a saved position
            if (savedRect.x < 0)
            {
                nextPanelX += ModPanel.MIN_WIDTH + PANEL_SPACING;
            }
    
            return SleddingAPIStatus.Ok;
        }
        
        /// <summary>
        /// Registers a new mod panel with custom sizing options.
        /// The panel will be automatically positioned and can be moved/resized by the user.
        /// Panel positions are saved and restored between game sessions.
        /// </summary>
        /// <param name="modName">Unique name for the mod panel</param>
        /// <param name="customMinSize">Custom minimum size (optional)</param>
        /// <param name="customRecommendedSize">Custom recommended size (optional)</param>
        /// <returns>SleddingAPIStatus indicating success or failure</returns>
        /// <remarks>
        /// Panel names must be unique. If a panel with the same name exists, registration will fail.
        /// </remarks>
        public SleddingAPIStatus RegisterModPanel(string modName, Vector2? customMinSize = null, Vector2? customRecommendedSize = null)
        {
            if (string.IsNullOrWhiteSpace(modName)) return SleddingAPIStatus.InvalidArgument;
            if (GetModPanel(modName) != null) return SleddingAPIStatus.ModPanelAlreadyRegistered;
            
            // Load saved position and size or use defaults
            Rect savedRect = Plugin.LoadPanelRect(modName);
            if (savedRect.x < 0)
            {
                savedRect.x = nextPanelX;
                savedRect.y = 20f;
            }
    
            var panel = new ModPanel(modName, savedRect, customMinSize, customRecommendedSize);
            modPanels.Add(modName, panel);
    
            // Only update nextPanelX if we're not using a saved position
            if (savedRect.x < 0)
            {
                var minWidth = customMinSize?.x ?? ModPanel.MIN_WIDTH;
                nextPanelX += minWidth + PANEL_SPACING;
            }
    
            return SleddingAPIStatus.Ok;
        }

        /// <summary>
        /// Registers a new tab in an existing mod panel.
        /// Creates a simple tab with no custom content.
        /// </summary>
        /// <param name="modName">Name of the mod panel to add the tab to</param>
        /// <param name="tabName">Name of the new tab</param>
        /// <returns>SleddingAPIStatus indicating success or failure</returns>
        /// <remarks>
        /// The mod panel must exist before adding tabs to it.
        /// </remarks>
        public SleddingAPIStatus RegisterModTab(string modName, string tabName)
        {
            if (string.IsNullOrWhiteSpace(modName) || string.IsNullOrWhiteSpace(tabName)) return SleddingAPIStatus.InvalidArgument;
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            panel.AddTab(tabName);
            return SleddingAPIStatus.Ok;
        }

        /// <summary>
        /// Registers a custom ModTab object in an existing mod panel.
        /// Allows for advanced custom UI implementations.
        /// </summary>
        /// <param name="modName">Name of the mod panel to add the tab to</param>
        /// <param name="tab">Custom ModTab object to register</param>
        /// <returns>SleddingAPIStatus indicating success or failure</returns>
        /// <remarks>
        /// Use this method when you need custom UI behavior beyond simple options.
        /// </remarks>
        public SleddingAPIStatus RegisterModTab(string modName, ModTab tab)
        {
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            panel.AddTab(tab);
            return SleddingAPIStatus.Ok;
        }
        
        private SleddingAPIStatus RegisterOption(string modName, string tabName, string optionId, string name, OptionType optionType)
        {
            if (string.IsNullOrWhiteSpace(modName) || string.IsNullOrWhiteSpace(tabName) || string.IsNullOrWhiteSpace(optionId))
                return SleddingAPIStatus.InvalidArgument;
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            return panel.AddOption(tabName, optionId, name, optionType);
        }

        /// <summary>
        /// Registers a custom ModOption object in a specific tab.
        /// Allows for advanced custom option implementations.
        /// </summary>
        /// <param name="modName">Name of the mod panel</param>
        /// <param name="tabName">Name of the tab to add the option to</param>
        /// <param name="option">Custom ModOption object to register</param>
        /// <returns>SleddingAPIStatus indicating success or failure</returns>
        /// <remarks>
        /// Use this method when you need custom option behavior beyond the built-in types.
        /// </remarks>
        public SleddingAPIStatus RegisterOption(string modName, string tabName, ModOption option)
        {
            if (option == null) return SleddingAPIStatus.InvalidArgument;
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            return panel.AddOption(tabName, option);
        }

        /// <summary>
        /// Registers a label option in a specific tab.
        /// Labels are read-only text elements used for information display.
        /// </summary>
        /// <param name="modName">Name of the mod panel</param>
        /// <param name="tabName">Name of the tab to add the label to</param>
        /// <param name="optionId">Unique identifier for the option</param>
        /// <param name="labelName">Text to display in the label</param>
        /// <returns>SleddingAPIStatus indicating success or failure</returns>
        public SleddingAPIStatus RegisterLabelOption(string modName, string tabName, string optionId, string labelName)
        {
            return RegisterOption(modName, tabName, optionId, labelName, OptionType.Label);
        }

        /// <summary>
        /// Registers a button option with a Lua callback function.
        /// When clicked, the button will execute the provided Lua function.
        /// </summary>
        /// <param name="modName">Name of the mod panel</param>
        /// <param name="tabName">Name of the tab to add the button to</param>
        /// <param name="optionId">Unique identifier for the option</param>
        /// <param name="buttonName">Text to display on the button</param>
        /// <param name="callback">Lua function to execute when the button is clicked</param>
        /// <returns>SleddingAPIStatus indicating success or failure</returns>
        /// <remarks>
        /// The callback function must be a valid Lua function. Errors in the callback are caught and logged.
        /// </remarks>
        public SleddingAPIStatus RegisterButtonOption(string modName, string tabName, string optionId, string buttonName, DynValue callback)
        {
            if (callback == null || callback.Type != DataType.Function)
            {
                Plugin.GameAPI.Log($"{buttonName} (RegisterButtonOption) requires a valid function callback.");
                return SleddingAPIStatus.UnknownError;
            }
            ModOption_Button button = new ModOption_Button(optionId, buttonName);
            button.Clicked += () =>
            {
                try
                {
                    Plugin.StaticLogger?.LogInfo($"[SleddingAPI] Invoking Lua callback for button '{buttonName}' ({optionId})");
                    // Use protected call to avoid breaking the UI flow if callback throws
                    callback.Function.Call();
                    Plugin.StaticLogger?.LogInfo($"[SleddingAPI] Callback for '{buttonName}' finished");
                }
                catch (ScriptRuntimeException e)
                {
                    Plugin.GameAPI.Log($"Error executing callback for {buttonName}: {e.DecoratedMessage}");
                }
                catch (InvalidOperationException ex)
                {
                    Plugin.GameAPI.Log($"Invalid operation in callback for {buttonName}: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Plugin.GameAPI.Log($"Argument error in callback for {buttonName}: {ex.Message}");
                }
            };
            return RegisterOption(modName, tabName, button);
        }
        
        /// <summary>
        /// Registers a selector (dropdown) option with a Lua callback function.
        /// When the selection changes, the provided Lua function is called with the new value.
        /// </summary>
        /// <param name="modName">Name of the mod panel</param>
        /// <param name="tabName">Name of the tab to add the selector to</param>
        /// <param name="optionId">Unique identifier for the option</param>
        /// <param name="optionName">Display name for the selector</param>
        /// <param name="onChanged">Lua function to execute when the selection changes (optional)</param>
        /// <returns>SleddingAPIStatus indicating success or failure</returns>
        /// <remarks>
        /// The onChanged callback receives the new selected value as a parameter.
        /// Use UpdateOption() to modify the available options after creation.
        /// </remarks>
        public SleddingAPIStatus RegisterSelectorOption(string modName, string tabName, string optionId, string optionName, DynValue onChanged)
        {
            if (string.IsNullOrWhiteSpace(modName) || string.IsNullOrWhiteSpace(tabName) || string.IsNullOrWhiteSpace(optionId))
                return SleddingAPIStatus.InvalidArgument;
            var selector = new ModOption_Selector(optionId, optionName);
            if (onChanged != null && onChanged.Type == DataType.Function)
            {
                selector.ValueChanged += (v) =>
                {
                    try { onChanged.Function.Call(v); }
                    catch (ScriptRuntimeException e) { Plugin.GameAPI.Log($"Selector {optionId} error: {e.DecoratedMessage}"); }
                };
            }
            return RegisterOption(modName, tabName, selector);
        }
        
        /// <summary>
        /// Updates an existing option with new data.
        /// Used to modify option properties after creation (e.g., update selector options).
        /// </summary>
        /// <param name="modName">Name of the mod panel</param>
        /// <param name="tabName">Name of the tab containing the option</param>
        /// <param name="request">UpdateOptionRequest containing the new data</param>
        /// <returns>SleddingAPIStatus indicating success or failure</returns>
        public SleddingAPIStatus UpdateOption(string modName, string tabName, UpdateOptionRequest request)
        {
            if (!modPanels.ContainsKey(modName))
                return SleddingAPIStatus.ModPanelNotFound;

            var panel = modPanels[modName];
            return panel.UpdateOption(tabName, request);
        }

        /// <summary>
        /// Removes an option from a specific tab.
        /// </summary>
        /// <param name="modName">Name of the mod panel</param>
        /// <param name="tabName">Name of the tab containing the option</param>
        /// <param name="optionId">ID of the option to remove</param>
        /// <returns>SleddingAPIStatus indicating success or failure</returns>
        public SleddingAPIStatus RemoveOption(string modName, string tabName, string optionId)
        {
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            var tab = panel.GetTab(tabName);
            if (tab == null) return SleddingAPIStatus.ModTabNotFound;
            bool removed = tab.RemoveOption(optionId);
            return removed ? SleddingAPIStatus.Ok : SleddingAPIStatus.UnknownError;
        }

        /// <summary>
        /// Removes a tab from a mod panel.
        /// </summary>
        /// <param name="modName">Name of the mod panel</param>
        /// <param name="tabName">Name of the tab to remove</param>
        /// <returns>SleddingAPIStatus indicating success or failure</returns>
        public SleddingAPIStatus RemoveModTab(string modName, string tabName)
        {
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            return panel.RemoveTab(tabName) ? SleddingAPIStatus.Ok : SleddingAPIStatus.ModTabNotFound;
        }

        /// <summary>
        /// Gets a mod panel by name.
        /// </summary>
        /// <param name="modName">Name of the mod panel to retrieve</param>
        /// <returns>The ModPanel object, or null if not found</returns>
        public ModPanel GetModPanel(string modName)
        {
            if (modPanels.ContainsKey(modName)) return modPanels[modName];
            return null;
        }

        /// <summary>
        /// Removes a mod panel and all its tabs and options.
        /// </summary>
        /// <param name="modName">Name of the mod panel to remove</param>
        /// <returns>True if the panel was removed, false if it didn't exist</returns>
        public bool RemoveModPanel(string modName)
        {
            return modPanels.Remove(modName);
        }

        /// <summary>
        /// Gets all registered mod panels.
        /// </summary>
        /// <returns>Dictionary of all mod panels keyed by name</returns>
        public Dictionary<string, ModPanel> GetAllModPanels()
        {
            return modPanels;
        }

        /// <summary>
        /// Removes all mod panels and resets the positioning system.
        /// Use with caution as this will destroy all mod UI elements.
        /// </summary>
        public void RemoveAllModPanels()
        {
            modPanels.Clear();
            nextPanelX = 20f;
        }
    }
}