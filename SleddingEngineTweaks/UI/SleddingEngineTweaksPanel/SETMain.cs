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
            
            SleddingAPI.RegisterModPanel(modName);

            SleddingAPI.RegisterModTab(modName, "Options");
            SleddingAPI.RegisterOption(modName, "Options", $"SET Version: {MyPluginInfo.PLUGIN_VERSION}", OptionType.Label);
            SleddingAPI.RegisterOption(modName, "Options", $"Sledding Game Version: {Application.version}", OptionType.Label);
            SleddingAPI.RegisterOption(modName, "Options", showOnStartOption);
            
            SleddingAPI.RegisterModTab(modName, "Keybinds");
            SleddingAPI.RegisterOption(modName, "Keybinds", masterKey);
            
            //SleddingAPI.RegisterModTab(modName, "Console");
            SleddingAPI.RegisterModTab(modName, consoleTab);
            SleddingAPI.RegisterModTab(modName, "Debug");
            SleddingAPI.RegisterModTab(modName, "Extra");
        }
    }
}