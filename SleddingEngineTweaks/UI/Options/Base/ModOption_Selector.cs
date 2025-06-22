using System;
using UnityEngine;

namespace SleddingEngineTweaks.UI.Options.Base
{
    public class ModOption_Selector : ModOption
    {
        private bool _enabled;
        public event Action<bool> ValueChanged;

        public ModOption_Selector(string name, bool enabled = false) : base(name, OptionType.Selector)
        {
            _enabled = enabled;
        }
        
        public override void Render()
        {
            bool previous = _enabled;
            _enabled = GUILayout.Toggle(_enabled, GetName());

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

        public bool IsEnabled() => _enabled;
    }
}