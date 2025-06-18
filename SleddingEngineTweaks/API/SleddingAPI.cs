using System.Collections.Generic;
using SleddingEngineTweaks.UI;
using UnityEngine;

namespace SleddingEngineTweaks.API
{
    public static class SleddingAPI
    {
        private static Dictionary<string, ModPanel> modPanels = new();
        private static float nextPanelX = 20f;
        private const float PANEL_SPACING = 20f;

        public static SleddingAPIStatus RegisterModPanel(string modName)
        {
           
            if (GetModPanel(modName) != null) return SleddingAPIStatus.ModPanelAlreadyRegistered;
            
            // Load saved position or use next available position
            Vector2 savedPos = Plugin.LoadPanelPosition(modName);
            float x = savedPos.x >= 0 ? savedPos.x : nextPanelX;
            float y = savedPos.y >= 0 ? savedPos.y : 20f;
            
            var panel = new ModPanel(modName, new Rect(x, y, ModPanel.MIN_WIDTH, ModPanel.MIN_HEIGHT));
            modPanels.Add(modName, panel);

            
            // Only update nextPanelX if we're not using a saved position
            if (savedPos.x < 0)
            {
                nextPanelX += ModPanel.MIN_WIDTH + PANEL_SPACING;
            }

            
            return SleddingAPIStatus.Ok;
        }

        public static SleddingAPIStatus RegisterModTab(string modName, string tabName)
        {
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            panel.AddTab(tabName);
            return SleddingAPIStatus.Ok;
        }
        
        public static SleddingAPIStatus RegisterOption(string modName, string tabName, string optionName, OptionType optionType)
        {
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            return panel.AddOption(tabName, optionName, optionType);
        }

        public static SleddingAPIStatus RegisterOption(string modName, string tabName, ModOption option)
        {
            ModPanel panel = GetModPanel(modName);
            if (panel == null) return SleddingAPIStatus.ModPanelNotFound;
            return panel.AddOption(tabName, option);
        }
        
        public static SleddingAPIStatus UpdateOption(string modName, string tabName, string newText, OptionType optionType)
        {
            if (!modPanels.ContainsKey(modName))
                return SleddingAPIStatus.ModPanelNotFound;

            var panel = modPanels[modName];
            return panel.UpdateOption(tabName, newText, optionType);
        }

        public static ModPanel GetModPanel(string modName)
        {
            if (modPanels.ContainsKey(modName)) return modPanels[modName];
            return null;
        }

        public static Dictionary<string, ModPanel> GetAllModPanels()
        {
            return modPanels;
        }
    }

    public enum SleddingAPIStatus
    {
        Ok,
        UnknownError,
        ModPanelNotFound,
        ModPanelAlreadyRegistered,
        ModTabNotFound,
        ModTabAlreadyRegistered,
    }
}