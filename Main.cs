using MelonLoader;
using SleddingEngineTweaks.Mods;

namespace SleddingEngineTweaks
{
    public static class BuildInfo
    {
        public const string Name = "Sledding Engine Tweaks"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "SET"; // Description for the Mod.  (Set as null if none)
        public const string Author = "Xndr"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "0.1"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }
    public class SleddingEngineTweaks : MelonMod
    {
        public static SleddingEngineTweaks Instance;
        
        private static FirstPerson firstPersonMod;


        public SleddingEngineTweaks() : base()
        {
            Instance = this;
        }

        public override void OnEarlyInitializeMelon()
        {
            // Initialize the first person mod
            firstPersonMod = new FirstPerson();
        }

        public override void OnLateUpdate()
        {
            // Delegate update calls to the first person mod
            firstPersonMod?.OnLateUpdate();
        }

        public override void OnDeinitializeMelon()
        {
            // Delegate cleanup to the first person mod
            FirstPerson.OnDeinitialize();
        }
    }

}
