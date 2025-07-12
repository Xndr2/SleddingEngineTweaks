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
        
        private SleddingAPIStatus RegisterOption(string modName, string tabName, string optionName, OptionType optionType)
        {
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            return panel.AddOption(tabName, optionName, optionType);
        }

        public SleddingAPIStatus RegisterOption(string modName, string tabName, ModOption option)
        {
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            return panel.AddOption(tabName, option);
        }

        public SleddingAPIStatus RegisterLabelOption(string modName, string tabName, string labelName)
        {
            return RegisterOption(modName, tabName, labelName, OptionType.Label);
        }

        public SleddingAPIStatus RegisterButtonOption(string modName, string tabName, string buttonName, DynValue callback)
        {
            if (callback == null || callback.Type != DataType.Function)
            {
                Plugin.GameAPI.Log($"{buttonName} (RegisterButtonOption) requires a valid function callback.");
                return SleddingAPIStatus.UnknownError;
            }
            ModOption_Button button = new ModOption_Button(buttonName);
            button.Clicked += () =>
            {
                try
                {
                    callback.Function.Call();
                }
                catch (ScriptRuntimeException e)
                {
                    Plugin.GameAPI.Log($"Error executing callback for {buttonName}: {e.DecoratedMessage}");
                }
            };
            return RegisterOption(modName, tabName, button);
        }
        
        // TODO: add RegisterSelectorOption method
        
        public SleddingAPIStatus UpdateOption(string modName, string tabName, string oldText, string newText, OptionType optionType)
        {
            if (!modPanels.ContainsKey(modName))
                return SleddingAPIStatus.ModPanelNotFound;

            var panel = modPanels[modName];
            return panel.UpdateOption(tabName, oldText, newText, optionType);
        }

        public ModPanel GetModPanel(string modName)
        {
            if (modPanels.ContainsKey(modName)) return modPanels[modName];
            return null;
        }

        public Dictionary<string, ModPanel> GetAllModPanels()
        {
            return modPanels;
        }
    }
}