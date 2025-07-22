using BepInEx;
using SleddingEngineTweaks.API;
using SleddingEngineTweaks.UI.Options.Base;
using SleddingEngineTweaks.UI.Options.Options;
using UnityEngine;

namespace SleddingEngineTweaks.UI.SleddingEngineTweaksPanel
{
    public class SETMain
    {
        private string modName = MyPluginInfo.PLUGIN_NAME;
        
        public SETMain()
        {
            Setup();
        }

        public void Setup()
        {
            BtnMasterKey masterKey = new BtnMasterKey();
            ConsoleTab consoleTab = new ConsoleTab();
            var showOnStartOption = new ModOption_Selector("Show this UI on startup", Plugin.ShowOnStart.Value);
            showOnStartOption.ValueChanged += (value) => Plugin.ShowOnStart.Value = value;
            
            Plugin.SleddingAPI.RegisterModPanel(modName);
            
            Plugin.SleddingAPI.RegisterModTab(modName, "Options");
            Plugin.SleddingAPI.RegisterLabelOption(modName, "Options", $"SET Version: {MyPluginInfo.PLUGIN_VERSION}");
            Plugin.SleddingAPI.RegisterLabelOption(modName, "Options", $"Sledding Game Version: {Application.version}");
            Plugin.SleddingAPI.RegisterOption(modName, "Options", showOnStartOption);
            
            Plugin.SleddingAPI.RegisterModTab(modName, "Keybinds");
            Plugin.SleddingAPI.RegisterOption(modName, "Keybinds", masterKey);
            
            //SleddingAPI.RegisterModTab(modName, "Console");
            Plugin.SleddingAPI.RegisterModTab(modName, consoleTab);
            Plugin.SleddingAPI.RegisterModTab(modName, "Debug");
            Plugin.SleddingAPI.RegisterModTab(modName, "Extra");
            Plugin.SleddingAPI.RegisterOption(modName, "Extra", new BtnReloadScripts());
        }
    }
}