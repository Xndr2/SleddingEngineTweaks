using UnityEngine;

namespace SleddingEngineTweaks.API
{
    // Wrapper to expose a safer subset of GameObject to Lua
    public class SafeGameObject
    {
        internal GameObject Inner { get; }

        internal SafeGameObject(GameObject inner)
        {
            Inner = inner;
        }

        public string GetName()
        {
            return Inner != null ? Inner.name : string.Empty;
        }

        public Vector3 GetPosition()
        {
            return Inner != null ? Inner.transform.position : Vector3.zero;
        }

        public void SetPosition(Vector3 position)
        {
            if (Inner != null)
            {
                Inner.transform.position = position;
            }
        }

        public Vector3 GetRotationEuler()
        {
            return Inner != null ? Inner.transform.rotation.eulerAngles : Vector3.zero;
        }

        public void SetRotationEuler(Vector3 euler)
        {
            if (Inner != null)
            {
                Inner.transform.rotation = Quaternion.Euler(euler);
            }
        }

        public Vector3 GetScale()
        {
            return Inner != null ? Inner.transform.localScale : Vector3.one;
        }

        public void SetScale(Vector3 scale)
        {
            if (Inner != null)
            {
                Inner.transform.localScale = scale;
            }
        }

        public void SetActive(bool active)
        {
            if (Inner != null)
            {
                Inner.SetActive(active);
            }
        }

        public SafeGameObject GetParent()
        {
            if (Inner == null || Inner.transform.parent == null) return null;
            return new SafeGameObject(Inner.transform.parent.gameObject);
        }

        public SafeGameObject GetChildByName(string childName)
        {
            if (Inner == null || string.IsNullOrEmpty(childName)) return null;
            var t = Inner.transform.Find(childName);
            return t != null ? new SafeGameObject(t.gameObject) : null;
        }

        public Vector3 GetForward()
        {
            return Inner != null ? Inner.transform.forward : Vector3.forward;
        }

        public void Destroy()
        {
            if (Inner != null)
            {
                Object.Destroy(Inner);
            }
        }

        public string GetPath()
        {
            if (Inner == null) return string.Empty;
            var current = Inner.transform;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            while (current != null)
            {
                if (sb.Length == 0) sb.Insert(0, current.name);
                else sb.Insert(0, current.name + "/");
                current = current.parent;
            }
            return sb.ToString();
        }
    }

    public class RaycastResult
    {
        public bool Hit { get; set; }
        public Vector3 Point { get; set; }
        public Vector3 Normal { get; set; }
        public SafeGameObject HitObject { get; set; }
    }
}

