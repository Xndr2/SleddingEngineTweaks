using SleddingEngineTweaks.UI.Options.Base;

namespace SleddingEngineTweaks.UI.Options.Options
{
    public class SelectShowOnStart : ModOption_Selector
    {
        public SelectShowOnStart() : base("Show this UI on startup") {}

        public override void OnToggleChanged(bool value)
        {
            Plugin.ShowOnStart.Value = value;
        }
    }
}