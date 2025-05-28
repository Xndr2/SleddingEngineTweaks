using UnityEngine;

namespace SleddingEngineTweaks.UI.Options.Base
{
    public class ModOption_Button : ModOption
    {
        private bool _isPressed = false;
        private string _labelName = "";
        
        public ModOption_Button(string name, string labelName) : base(name, OptionType.Button)
        {
            _labelName = labelName;
        }

        public ModOption_Button(string name) : base(name, OptionType.Button) {}

        public override void Render()
        {
            if (!_labelName.Equals(""))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_labelName);
            }
            if (GUILayout.Button(_isPressed ? "press a button" : GetName()))
            {
                if (!_isPressed)
                    OnPress();
            }
            if(!_labelName.Equals("")) GUILayout.EndHorizontal();
        }

        public void OnPress()
        {
            _isPressed = true;
        }
    }
}