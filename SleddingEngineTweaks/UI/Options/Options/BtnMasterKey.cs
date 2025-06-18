using SleddingEngineTweaks.UI.Options.Base;
using UnityEngine.InputSystem;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine.InputSystem.Controls;

namespace SleddingEngineTweaks.UI.Options.Options
{
    public class BtnMasterKey : ModOption_Button
    {
        public BtnMasterKey() : base(Plugin.MasterKey.Value.ToString(), "Master Key") {}
        
        
        public override void Render()
        {
            base.Render();
            
            if (IsPressed)
            {
                SetName("listening...");
                if (Keyboard.current == null) return;
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    SetName(Plugin.MasterKey.Value.ToString());
                    IsPressed = false;
                    return;
                }
                foreach (KeyControl keyControl in Keyboard.current.allKeys)
                {
                    if (keyControl.wasPressedThisFrame)
                    {
                        if (keyControl.keyCode == Key.Escape)
                        {
                            SetName(Plugin.MasterKey.Value.ToString());
                            IsPressed = false;
                            continue;
                        }
                        Plugin.MasterKey.Value = keyControl.keyCode;
                        Plugin.StaticLogger.LogInfo($"New master key set: {keyControl.keyCode}");
                        SetName(keyControl.keyCode.ToString());
                        IsPressed = false;
                        break;
                    }
                }
            }
        }
    }
}