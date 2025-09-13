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
            ConsoleTab consoleTab = new ConsoleTab();
            HierarchyTab hierarchyTab = new HierarchyTab();
            PrefabTab prefabTab = new PrefabTab();
            var showOnStartOption = new ModOption_Selector("show_on_start_selector", "Show this UI on startup");
            showOnStartOption.ValueChanged += (value) => Plugin.ShowOnStart.Value = value;
            
            // Main panel with default sizing
            Plugin.SleddingAPI.RegisterModPanel(modName);
            
            Plugin.SleddingAPI.RegisterModTab(modName, "Options");
            Plugin.SleddingAPI.RegisterLabelOption(modName, "Options", "set_version_label", $"SET Version: {MyPluginInfo.PLUGIN_VERSION}");
            Plugin.SleddingAPI.RegisterLabelOption(modName, "Options", "game_version_label", $"Sledding Game Version: {Application.version}");
            Plugin.SleddingAPI.RegisterOption(modName, "Options", showOnStartOption);
            
            Plugin.SleddingAPI.RegisterModTab(modName, "Keybinds");
            Plugin.SleddingAPI.RegisterOption(modName, "Keybinds", new KeybindRemapButton(Plugin.MasterKey, "Master Key"));
            Plugin.SleddingAPI.RegisterOption(modName, "Keybinds", new KeybindRemapButton(Plugin.ToggleMouse, "Toggle Mouse"));
            
            Plugin.SleddingAPI.RegisterModTab(modName, consoleTab);
            Plugin.SleddingAPI.RegisterModTab(modName, hierarchyTab);
            Plugin.SleddingAPI.RegisterModTab(modName, prefabTab);
            Plugin.SleddingAPI.RegisterModTab(modName, "Debug");
            Plugin.SleddingAPI.RegisterModTab(modName, "Extra");
            Plugin.SleddingAPI.RegisterOption(modName, "Extra", new BtnReloadScripts("reload_scripts_btn"));

            // Debug info: scene, player path, position
            Plugin.SleddingAPI.RegisterLabelOption(modName, "Debug", "dbg_scene_label", "Scene: ");
            Plugin.SleddingAPI.RegisterLabelOption(modName, "Debug", "dbg_player_path_label", "Player Path: ");
            Plugin.SleddingAPI.RegisterLabelOption(modName, "Debug", "dbg_player_pos_label", "Player Pos: ");
        }
    }
}