using System;
using UnityEngine;

namespace SleddingEngineTweaks.UI.Options.Base
{
    public class ModOption_Selector : ModOption
    {
        private bool _enabled;
        public event Action<bool> ValueChanged;

        public ModOption_Selector(string optionId, string name) : base(optionId, name, OptionType.Selector) {}
        
        public override void Render()
        {
            if (!IsVisible()) return;
            bool previous = _enabled;
            using (new GUIEnabledScope(IsEnabled()))
            {
                _enabled = GUILayout.Toggle(_enabled, GetName());
            }

            if (_enabled != previous)
            {
                OnToggleChanged(_enabled);
                ValueChanged?.Invoke(_enabled);
            }
        }

        public virtual void OnToggleChanged(bool value)
        {
            // For subclasses to override if needed
        }

        public bool GetValue() => _enabled;
    }
}