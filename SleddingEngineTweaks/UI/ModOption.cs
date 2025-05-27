namespace SleddingEngineTweaks.UI
{
    public abstract class ModOption
    {
        private string _name;
        private OptionType _type;

        protected ModOption(string name, OptionType type)
        {
            _name = name;
            _type = type;
        }

        public string GetName()
        {
            return _name;
        }

        public string GetTypeName()
        {
            return _type.ToString();
        }

        public abstract void Render();
    }

    public enum OptionType
    {
        Label,
        Selector,
        Button,
    }
}