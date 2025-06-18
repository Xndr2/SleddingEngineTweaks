using UnityEngine;

namespace SleddingEngineTweaks.UI.Options.Base
{
    public class ModOption_Selector : ModOption
    {
        private bool _enabled = false;
        private bool isDown = false;

        public ModOption_Selector(string name) : base(name, OptionType.Selector)
        {
            _enabled = false;
        }

        public ModOption_Selector(string name, bool enabled) : base(name, OptionType.Selector)
        {
            _enabled = enabled;
        }
        
        public override void Render()
        {
            bool previous = _enabled;
            
            if (GUILayout.Toggle(_enabled, GetName()))
            {
                if (!isDown)
                {
                    isDown = true;
                    _enabled = !_enabled;
                }
            }
            else
            {
                isDown = false;
            }
            
            // check for change
            if (_enabled != previous)
            {
                OnToggleChanged(_enabled);
            }
        }

        public virtual void OnToggleChanged(bool value)
        {
            // child will override this
        }

        public bool IsEnabled() => _enabled;
    }
}