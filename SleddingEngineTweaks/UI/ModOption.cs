namespace SleddingEngineTweaks.UI
{
    public abstract class ModOption
    {
        private string _name;
        private OptionType _type;
        private string _optionId;
        private bool _visible = true;
        private bool _enabled = true;

        protected ModOption(string optionId, string name, OptionType type)
        {
            _optionId = optionId;
            _name = name;
            _type = type;
        }

        public string GetName()
        {
            return _name;
        }

        public void SetName(string name)
        {
            if (name == null || _name.Equals(name)) return;
            _name = name;
        }

        public OptionType GetOptionType()
        {
            return _type;
        }

        public string GetTypeName()
        {
            return _type.ToString();
        }

        public string GetOptionId()
        {
            return _optionId;
        }

        public bool IsVisible() => _visible;
        public void SetVisible(bool visible) { _visible = visible; }
        public bool IsEnabled() => _enabled;
        public void SetEnabled(bool enabled) { _enabled = enabled; }

        public abstract void Render();
    }

    public enum OptionType
    {
        Label,
        Selector,
        Button,
    }

    // Utility scope to temporarily toggle GUI.enabled
    internal readonly struct GUIEnabledScope : System.IDisposable
    {
        private readonly bool _previous;
        public GUIEnabledScope(bool enabled)
        {
            _previous = UnityEngine.GUI.enabled;
            UnityEngine.GUI.enabled = enabled;
        }
        public void Dispose()
        {
            UnityEngine.GUI.enabled = _previous;
        }
    }
}