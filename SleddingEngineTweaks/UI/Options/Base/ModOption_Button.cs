using UnityEngine;

namespace SleddingEngineTweaks.UI.Options.Base
{
    public class ModOption_Button : ModOption
    {
        public ModOption_Button(string name) : base(name, OptionType.Button) {}
        private bool isPressed = false;

        public override void Render()
        {
            if (GUILayout.Button(isPressed ? "press a button" : GetName()))
            {
                if (!isPressed)
                    OnPress();
            }
        }

        public void OnPress()
        {
            Plugin.StaticLogger.LogInfo($"Clicked button");
            isPressed = !isPressed;
        }
    }
}