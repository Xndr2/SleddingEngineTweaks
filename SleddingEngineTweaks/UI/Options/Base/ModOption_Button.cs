using UnityEngine;
using System;
using SleddingEngineTweaks.UI;

namespace SleddingEngineTweaks.UI.Options.Base
{
    public class ModOption_Button : ModOption
    {
        protected bool IsPressed { get; set; } = false;
        private readonly string _labelName;
        
        public event Action Clicked;
        
        public ModOption_Button(string optionId, string name, string labelName = "") : base(optionId, name, OptionType.Button)
        {
            _labelName = labelName;
        }

        public override void Render()
        {
            if (!IsVisible()) return;
            bool hasLabel = !string.IsNullOrEmpty(_labelName);
            if (hasLabel)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_labelName);
            }

            using (new GUIEnabledScope(IsEnabled()))
            {
                if (GUILayout.Button(GetName()))
                {
                    OnClicked();
                }
            }

            if (hasLabel)
            {
                GUILayout.EndHorizontal();
            }
        }

        protected virtual void OnClicked()
        {
            if (!IsPressed)
            {
                IsPressed = true;
            }
            SleddingEngineTweaks.Plugin.StaticLogger?.LogInfo($"[UI] Button clicked: id={GetOptionId()}, name={GetName()}");
            Clicked?.Invoke();
        }
    }
}