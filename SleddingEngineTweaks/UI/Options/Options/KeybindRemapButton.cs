using BepInEx.Configuration;
using SleddingEngineTweaks.UI.Options.Base;
using UnityEngine.InputSystem;

namespace SleddingEngineTweaks.UI.Options.Options
{
    // Reusable keybind remap button for any ConfigEntry<Key>
    public class KeybindRemapButton : ModOption_Button
    {
        private readonly ConfigEntry<Key> _config;

        // label is what shows next to the button, defaults to the config key name
        public KeybindRemapButton(ConfigEntry<Key> config, string label = null)
            : base(config.Value.ToString(), config.Value.ToString(), label)
        {
            _config = config;
        }

        public override void Render()
        {
            // draw base label + button
            base.Render();

            if (!IsPressed)
                return;

            // Listening state
            SetName("Listening...");

            if (Keyboard.current == null)
                return;

            // Allow cancel with mouse left click or Escape
            if ((Mouse.current?.leftButton.wasPressedThisFrame ?? false) ||
                Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                ResetButtonState();
                return;
            }

            // Capture first pressed key this frame
            foreach (var keyControl in Keyboard.current.allKeys)
            {
                if (!keyControl.wasPressedThisFrame)
                    continue;

                _config.Value = keyControl.keyCode;
                Plugin.StaticLogger?.LogInfo($"Keybind '{_config.Definition.Key}' set to: {keyControl.keyCode}");
                ResetButtonState(keyControl.keyCode.ToString());
                break;
            }
        }

        private void ResetButtonState(string newName = null)
        {
            IsPressed = false;
            SetName(newName ?? _config.Value.ToString());
        }
    }
}