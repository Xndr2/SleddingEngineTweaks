using System.Collections.Generic;
using SleddingEngineTweaks.UI;
using UnityEngine;

namespace SleddingEngineTweaks.API
{
    public static class SleddingAPI
    {
        private static Dictionary<string, ModPanel> modPanels = new();

        public static SleddingAPIStatus RegisterModPanel(string modName)
        {
            ModPanel panel = GetModPanel(modName);
            if (panel != null) return SleddingAPIStatus.ModPanelAlreadyRegistered;
            panel = new ModPanel(modName, new Rect(400, 10, 10, 10));
            modPanels.Add(modName, panel);
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