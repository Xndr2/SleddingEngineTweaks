using SleddingGameModFramework.Interfaces;

namespace ExampleMod
{
    public class ExampleModConfig : IModConfig
    {
        public string ModName => "ExampleMod";

        public bool ShowUI { get; set; } = true;
        public string UIText { get; set; } = "Example Mod Running!";
        public int UpdateInterval { get; set; } = 60;

        public void OnConfigLoaded()
        {
            // Configuration loaded, update any necessary state
        }

        public void OnConfigSaved()
        {
            // Configuration saved, update any necessary state
        }

        public void OnConfigChanged(string propertyName)
        {
            // Configuration changed, update any necessary state
        }
    }
} 