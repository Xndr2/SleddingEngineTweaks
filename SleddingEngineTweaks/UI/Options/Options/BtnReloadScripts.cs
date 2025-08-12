using SleddingEngineTweaks.UI.Options.Base;
using SleddingEngineTweaks.Scripting;

namespace SleddingEngineTweaks.UI.Options.Options
{
    public class BtnReloadScripts : ModOption_Button
    {
        public BtnReloadScripts(string optionId) : base(optionId, "Reload Lua Scripts") {}

        public override void Render()
        {
            base.Render();
            if (IsPressed)
            {
                SleddingEngineTweaks.Plugin.StaticLogger?.LogInfo("[SET] Reload scripts button activated");
                LuaManager.Instance.ReloadAllScripts();
                IsPressed = false;
            }
        }
    }
} 