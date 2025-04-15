using UnityEngine;

namespace SleddingGameModFramework.UI
{
    public abstract class ModUI : IModUI
    {
        protected GameObject _root;
        private bool _isVisible;

        public GameObject Root => _root;
        public bool IsVisible => _isVisible;

        protected ModUI(string name)
        {
            _root = new GameObject(name);
            _root.transform.SetParent(null);
            _isVisible = false;
        }

        public virtual void OnCreate()
        {
            // Override this to set up your UI
        }

        public virtual void OnDestroy()
        {
            if (_root != null)
            {
                Object.Destroy(_root);
            }
        }

        public virtual void OnUpdate()
        {
            // Override this to update your UI
        }

        public virtual void Show()
        {
            if (_root != null)
            {
                _root.SetActive(true);
                _isVisible = true;
            }
        }

        public virtual void Hide()
        {
            if (_root != null)
            {
                _root.SetActive(false);
                _isVisible = false;
            }
        }

        protected T AddComponent<T>() where T : Component
        {
            return _root.AddComponent<T>();
        }

        protected GameObject CreateChild(string name)
        {
            var child = new GameObject(name);
            child.transform.SetParent(_root.transform);
            return child;
        }
    }
} 