using System.Collections.Generic;
using UnityEngine;

namespace SleddingGameModFramework.UI
{
    public class UIManager
    {
        private readonly List<IModUI> _uiElements = new List<IModUI>();

        public void RegisterUI(IModUI ui)
        {
            if (!_uiElements.Contains(ui))
            {
                _uiElements.Add(ui);
                ui.OnCreate();
            }
        }

        public void UnregisterUI(IModUI ui)
        {
            if (_uiElements.Remove(ui))
            {
                ui.OnDestroy();
            }
        }

        public void UpdateUI()
        {
            foreach (var ui in _uiElements)
            {
                if (ui.IsVisible)
                {
                    ui.OnUpdate();
                }
            }
        }

        public void ShowAll()
        {
            foreach (var ui in _uiElements)
            {
                ui.Show();
            }
        }

        public void HideAll()
        {
            foreach (var ui in _uiElements)
            {
                ui.Hide();
            }
        }

        public void Clear()
        {
            foreach (var ui in _uiElements)
            {
                ui.OnDestroy();
            }
            _uiElements.Clear();
        }
    }
} 