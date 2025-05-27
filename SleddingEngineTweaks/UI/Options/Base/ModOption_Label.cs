using UnityEngine;

namespace SleddingEngineTweaks.UI.Options.Base
{
    public class ModOption_Label : ModOption
    {
        public ModOption_Label(string name) : base(name, OptionType.Label) {}
        
        public override void Render()
        {
            GUILayout.Label(GetName());
        }
    }
}