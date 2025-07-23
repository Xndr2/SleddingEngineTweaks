using SleddingEngineTweaks.UI.Options.Base;
using SleddingEngineTweaks.Scripting;

namespace SleddingEngineTweaks.UI.Options.Options
{
    public class BtnReloadScripts : ModOption_Button
    {
        public BtnReloadScripts() : base("Reload Lua Scripts") {}

        public override void Render()
        {
            base.Render();
            if (IsPressed)
            {
                LuaManager.Instance.ReloadAllScripts();
                IsPressed = false;
            }
        }
    }
} 