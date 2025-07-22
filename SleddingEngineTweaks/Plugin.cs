using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using System;
using System.IO;
using SleddingEngineTweaks.Scripting;
using SleddingEngineTweaks.UI;
using SleddingEngineTweaks.UI.SleddingEngineTweaksPanel;
using SleddingEngineTweaks.API;

using UnityEngine;
using UnityEngine.InputSystem;

namespace SleddingEngineTweaks
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        internal static ManualLogSource StaticLogger;
        private static ImGuiController _controller;
        public LuaManager LuaManager { get; private set; }
        private static LuaEventHandler _luaEventHandler { get; set; }
        internal static GameAPI GameAPI;
        internal static SleddingAPI SleddingAPI;
    
        // config
        public static ConfigEntry<Key> MasterKey;
        public static ConfigEntry<bool> ShowOnStart;
        private static ConfigFile PanelConfigFile;

        private void Awake()
        {
            Instance = this;
            // keybind setup
            MasterKey = Config.Bind("Keybinds", "MasterKey", Key.Delete, "Key to toggle the UI");
            ShowOnStart = Config.Bind("Options", "ShowOnStart", false, "Show On Start");
            
            PanelConfigFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "SleddingEngineTweaks.panels.cfg"), true);
            
            // Plugin startup logic
            StaticLogger = base.Logger;
            StaticLogger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} awake");
        
            // init lua manager
            LuaManager = LuaManager.Instance;
        
            // register logger for lua scripts
            LuaManager.RegisterGlobal("log", new Action<string>(message => 
            {
                LuaManager.OutputMessage(message);
            }));

        
            // register any game APIs
            RegisterGameAPI();
        
            // set up UI
            string AssetBundlesPath = Path.Combine(Paths.PluginPath, "SleddingEngineTweaks", "set_prefab");
            _controller = new();
            AssetBundle bundle = AssetBundle.LoadFromFile(AssetBundlesPath);
            GameObject obj = bundle.LoadAsset<GameObject>("Yes");
            _controller.prefab = obj;
            _controller.Setup();
            SETMain main = new SETMain();
        
            // load all lua scripts in the scripts folder
            LuaManager.LoadAllScripts();
            // set up all events so lua scripts can access them
            _luaEventHandler = new();
            _luaEventHandler.Setup();
        
            StaticLogger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} loaded!");
        }
    
        public void RegisterGameAPI()
        {
            // Create an API object that exposes safe game functionality to Lua
            GameAPI = new GameAPI(this);
            SleddingAPI = new SleddingAPI(this);
            LuaManager.RegisterGlobal("game", GameAPI);
            LuaManager.RegisterGlobal("set", SleddingAPI);
        }

        private void OnDestroy()
        {
            GameAPI?.Dispose();
            SleddingAPI?.Dispose();
        }
        
        public static void SavePanelPosition(string panelName, Rect position)
        {
            var xEntry = PanelConfigFile.Bind($"PanelPositions", $"{panelName}_x", position.x);
            var yEntry = PanelConfigFile.Bind($"PanelPositions", $"{panelName}_y", position.y);
            var widthEntry = PanelConfigFile.Bind($"PanelPositions", $"{panelName}_width", position.width);
            var heightEntry = PanelConfigFile.Bind($"PanelPositions", $"{panelName}_height", position.height);

            // Update the values
            xEntry.Value = position.x;
            yEntry.Value = position.y;
            widthEntry.Value = position.width;
            heightEntry.Value = position.height;

            // Force save the config file
            PanelConfigFile.Save();
        }

    
        public static Rect LoadPanelRect(string panelName)
        {
            var x = PanelConfigFile.Bind($"PanelPositions", $"{panelName}_x", -1f).Value;
            var y = PanelConfigFile.Bind($"PanelPositions", $"{panelName}_y", -1f).Value;
            var width = PanelConfigFile.Bind($"PanelPositions", $"{panelName}_width", ModPanel.MIN_WIDTH).Value;
            var height = PanelConfigFile.Bind($"PanelPositions", $"{panelName}_height", ModPanel.MIN_HEIGHT).Value;
            return new Rect(x, y, width, height);
        }

    }

}