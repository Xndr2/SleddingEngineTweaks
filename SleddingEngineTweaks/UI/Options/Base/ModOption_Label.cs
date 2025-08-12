using UnityEngine;

namespace SleddingEngineTweaks.UI.Options.Base
{
    public class ModOption_Label : ModOption
    {
        public ModOption_Label(string optionId, string name) : base(optionId, name, OptionType.Label) {}
        
        public override void Render()
        {
            if (!IsVisible()) return;
            using (new GUIEnabledScope(IsEnabled()))
            {
                GUILayout.Label(GetName());
            }
        }
    }
}