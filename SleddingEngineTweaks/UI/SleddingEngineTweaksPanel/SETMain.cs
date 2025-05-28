using BepInEx;
using SleddingEngineTweaks.API;
using SleddingEngineTweaks.UI.Options;

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
            Btn_MasterKey masterKey = new Btn_MasterKey("N/A", "Master Key");
            
            SleddingAPI.RegisterModPanel(modName);

            SleddingAPI.RegisterModTab(modName, "Options");
            SleddingAPI.RegisterModTab(modName, "Keybinds");
            SleddingAPI.RegisterModTab(modName, "Console");
            SleddingAPI.RegisterModTab(modName, "Debug");

            SleddingAPI.RegisterOption(modName, "Options", "Test", OptionType.Label);
            SleddingAPI.RegisterOption(modName, "Options", "Test 2", OptionType.Label);
            SleddingAPI.RegisterOption(modName, "Keybinds", masterKey);
        }
    }
}