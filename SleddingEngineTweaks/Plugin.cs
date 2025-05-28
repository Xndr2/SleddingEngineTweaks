using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using SleddingEngineTweaks.UI;
using SleddingEngineTweaks.UI.SleddingEngineTweaksPanel;
using UnityEngine;

namespace SleddingEngineTweaks;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource StaticLogger;
    private static ImGuiController _controller;

    private void Awake()
    {
        // Plugin startup logic
        StaticLogger = base.Logger;
        StaticLogger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} awake");
        _controller = new();
        _controller.Setup();
        SETMain main = new SETMain();
    }
}