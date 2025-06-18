using UnityEngine;

namespace SleddingEngineTweaks.UI.Options.Base
{
    public class ModOption_Button : ModOption
    {
        protected bool IsPressed = false;
        private string _labelName;
        
        public ModOption_Button(string name, string labelName = "") : base(name, OptionType.Button)
        {
            _labelName = labelName;
        }

        public ModOption_Button(string name) : base(name, OptionType.Button) {}

        public override void Render()
        {
            if (!string.IsNullOrEmpty(_labelName))
                GUILayout.BeginHorizontal();
            
            if (!_labelName.Equals(""))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_labelName);
            }
            if (GUILayout.Button(GetName()))
            {
                if (!IsPressed)
                    IsPressed = true;
            }
            if(!_labelName.Equals("")) GUILayout.EndHorizontal();
        }
    }
}