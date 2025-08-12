using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using SleddingEngineTweaks.UI;
using SleddingEngineTweaks.UI.Options.Base;
using UnityEngine;

namespace SleddingEngineTweaks.API
{
    public enum SleddingAPIStatus
    {
        Ok,
        UnknownError,
        ModPanelNotFound,
        ModPanelAlreadyRegistered,
        ModTabNotFound,
        ModTabAlreadyRegistered,
        InvalidArgument,
    }
    
    public class SleddingAPI : IDisposable
    {
        private readonly Plugin _plugin;
        private static Dictionary<string, ModPanel> modPanels = new();
        private static float nextPanelX = 20f;
        private const float PANEL_SPACING = 20f;

        public SleddingAPI(Plugin plugin)
        {
            _plugin = plugin;
        }

        public void Dispose()
        {
            // we do nothing at the moment
        }
        
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

        public SleddingAPIStatus RegisterModTab(string modName, string tabName)
        {
            if (string.IsNullOrWhiteSpace(modName) || string.IsNullOrWhiteSpace(tabName)) return SleddingAPIStatus.InvalidArgument;
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            panel.AddTab(tabName);
            return SleddingAPIStatus.Ok;
        }

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

        public SleddingAPIStatus RegisterOption(string modName, string tabName, ModOption option)
        {
            if (option == null) return SleddingAPIStatus.InvalidArgument;
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            return panel.AddOption(tabName, option);
        }

        public SleddingAPIStatus RegisterLabelOption(string modName, string tabName, string optionId, string labelName)
        {
            return RegisterOption(modName, tabName, optionId, labelName, OptionType.Label);
        }

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
                catch (System.Exception ex)
                {
                    Plugin.GameAPI.Log($"Error executing callback for {buttonName}: {ex.Message}");
                }
            };
            return RegisterOption(modName, tabName, button);
        }
        
        // TODO: add RegisterSelectorOption method
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
        
        public SleddingAPIStatus UpdateOption(string modName, string tabName, UpdateOptionRequest request)
        {
            if (!modPanels.ContainsKey(modName))
                return SleddingAPIStatus.ModPanelNotFound;

            var panel = modPanels[modName];
            return panel.UpdateOption(tabName, request);
        }

        public SleddingAPIStatus RemoveOption(string modName, string tabName, string optionId)
        {
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            var tab = panel.GetTab(tabName);
            if (tab == null) return SleddingAPIStatus.ModTabNotFound;
            bool removed = tab.RemoveOption(optionId);
            return removed ? SleddingAPIStatus.Ok : SleddingAPIStatus.UnknownError;
        }

        public SleddingAPIStatus RemoveModTab(string modName, string tabName)
        {
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            return panel.RemoveTab(tabName) ? SleddingAPIStatus.Ok : SleddingAPIStatus.ModTabNotFound;
        }

        public ModPanel GetModPanel(string modName)
        {
            if (modPanels.ContainsKey(modName)) return modPanels[modName];
            return null;
        }

        public bool RemoveModPanel(string modName)
        {
            return modPanels.Remove(modName);
        }

        public Dictionary<string, ModPanel> GetAllModPanels()
        {
            return modPanels;
        }

        public void RemoveAllModPanels()
        {
            modPanels.Clear();
            nextPanelX = 20f;
        }
    }
}