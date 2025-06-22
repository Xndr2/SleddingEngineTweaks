using UnityEngine;
using System;

namespace SleddingEngineTweaks.UI.Options.Base
{
    public class ModOption_Button : ModOption
    {
        protected bool IsPressed { get; set; } = false;
        private readonly string _labelName;
        
        public event Action Clicked;
        
        public ModOption_Button(string name, string labelName = "") : base(name, OptionType.Button)
        {
            _labelName = labelName;
        }

        public override void Render()
        {
            bool hasLabel = !string.IsNullOrEmpty(_labelName);
            if (hasLabel)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_labelName);
            }

            if (GUILayout.Button(GetName()))
            {
                OnClicked();
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
            Clicked?.Invoke();
        }
    }
}