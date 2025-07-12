using System.Collections.Generic;
using SleddingEngineTweaks.UI.Options;
using SleddingEngineTweaks.UI.Options.Base;
using UnityEngine;

namespace SleddingEngineTweaks.UI
{
    public class ModTab
    {
        private string _name;
        private List<ModOption> options = new();

        public ModTab(string name)
        {
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }
        
        public void AddOption(string name, OptionType optionType)
        {
            ModOption option;
            switch (optionType)
            {
                default:
                    option = new ModOption_Label(name);
                    break;
                case OptionType.Label:
                    option = new ModOption_Label(name);
                    break;
                case OptionType.Selector:
                    option = new ModOption_Selector(name);
                    break;
                case OptionType.Button:
                    option = new ModOption_Button(name);
                    break;
            }
            options.Add(option);
        }

        public void AddOption(ModOption option)
        {
            options.Add(option);
        }
        
        public void UpdateOption(string oldText, string newText, OptionType optionType)
        {
            foreach (var option in options)
            {
                if (option.GetName() == oldText && option.GetOptionType() == optionType)
                {
                    option.SetName(newText);
                    break;
                }
            }
        }

        public virtual void Render()
        {
            foreach (var option in options)
            {
                option.Render();
            }
        }
    }
}