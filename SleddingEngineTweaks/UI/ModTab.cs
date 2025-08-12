using System.Collections.Generic;
using SleddingEngineTweaks.UI.Options;
using SleddingEngineTweaks.UI.Options.Base;
using UnityEngine;

namespace SleddingEngineTweaks.UI
{
    public class ModTab
    {
        private string _name;
        private Dictionary<string, ModOption> options = new();

        public ModTab(string name)
        {
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }
        
        public void AddOption(string optionId, string name, OptionType optionType)
        {
            ModOption option;
            switch (optionType)
            {
                default:
                case OptionType.Label:
                    option = new ModOption_Label(optionId, name);
                    break;
                case OptionType.Selector:
                    option = new ModOption_Selector(optionId, name);
                    break;
                case OptionType.Button:
                    option = new ModOption_Button(optionId, name);
                    break;
            }
            options[optionId] = option;
        }

        public void AddOption(ModOption option)
        {
            options[option.GetOptionId()] = option;
        }
        
        public void UpdateOption(UpdateOptionRequest request)
        {
            if (options.TryGetValue(request.OptionId, out var option))
            {
                if (!string.IsNullOrEmpty(request.NewName))
                {
                    option.SetName(request.NewName);
                }
                if (request.Visible.HasValue)
                {
                    option.SetVisible(request.Visible.Value);
                }
                if (request.Enabled.HasValue)
                {
                    option.SetEnabled(request.Enabled.Value);
                }
                // TODO: handle more fields from request
            }
        }

        public bool RemoveOption(string optionId)
        {
            return options.Remove(optionId);
        }

        public bool HasOption(string optionId)
        {
            return options.ContainsKey(optionId);
        }

        public virtual void Render()
        {
            foreach (var option in options.Values)
            {
                option.Render();
            }
        }
    }
}