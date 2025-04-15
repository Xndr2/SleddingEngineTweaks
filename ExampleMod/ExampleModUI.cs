using SleddingGameModFramework.UI;
using UnityEngine;

namespace ExampleMod
{
    public class ExampleModUI : ModUI
    {
        private GUIText _text;
        private int _updateCount;

        public ExampleModUI() : base("ExampleModUI")
        {
        }

        public override void OnCreate()
        {
            _text = AddComponent<GUIText>();
            _text.text = "Example Mod UI";
            _text.transform.position = new Vector3(0.1f, 0.9f, 0);
        }

        public override void OnUpdate()
        {
            _updateCount++;
            if (_updateCount % 60 == 0)
            {
                _text.text = $"Example Mod UI\nFrame: {_updateCount}";
            }
        }
    }
} 