using UnityEngine;

namespace SleddingEngineTweaks.UI.Options
{
    public class ModOption_Selector : ModOption
    {
        public ModOption_Selector(string name) : base(name, OptionType.Selector) {}
        
        public override void Render()
        {
            GUILayout.Label(GetName());
        }
    }
}