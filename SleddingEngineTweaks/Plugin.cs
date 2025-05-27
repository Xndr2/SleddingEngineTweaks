using BepInEx;
using BepInEx.Logging;
using SleddingEngineTweaks;
using UnityEngine;

namespace SleddingEngineTweaks;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource StaticLogger;

    private void Awake()
    {
        // Plugin startup logic
        StaticLogger = base.Logger;
        StaticLogger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} awake");
        ImGuiController.Setup();
    }
}