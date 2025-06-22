using SleddingEngineTweaks.UI.Options.Base;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace SleddingEngineTweaks.UI.Options.Options
{
    public class BtnMasterKey : ModOption_Button
    {
        public BtnMasterKey() : base(Plugin.MasterKey.Value.ToString(), "Master Key") {}
        
        public override void Render()
        {
            // will draw the button and label
            base.Render();
            
            // handles logic for when the button is in the "listening" state
            if (IsPressed)
            {
                HandleMasterKeyInput();
            }
        }

        private void HandleMasterKeyInput()
        {
            SetName("Listening...");
            if (Keyboard.current == null) return;
            
            // allow canceling with mouse click or escape key
            if (Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                ResetButtonState();
                return;
            }

            foreach (var keyControl in Keyboard.current.allKeys)
            {
                if (keyControl.wasPressedThisFrame)
                {
                    Plugin.MasterKey.Value = keyControl.keyCode;
                    Plugin.StaticLogger.LogInfo($"New master key set: {keyControl.keyCode}");
                    ResetButtonState(keyControl.keyCode.ToString());
                    break;
                }
            }
        }
        
        private void ResetButtonState(string newName = null)
        {
            IsPressed = false;
            SetName(newName ?? Plugin.MasterKey.Value.ToString());
        }
    }
}