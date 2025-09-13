using UnityEngine;
using System;

namespace SleddingEngineTweaks.API
{
    /// <summary>
    /// Safe wrapper around Unity's GameObject that exposes only safe operations to Lua scripts.
    /// Prevents direct access to Unity internals while providing essential GameObject functionality.
    /// All methods include null checks to prevent crashes.
    /// </summary>
    public class SafeGameObject
    {
        internal GameObject Inner { get; }

        /// <summary>
        /// Initializes a new SafeGameObject wrapper.
        /// </summary>
        /// <param name="inner">The Unity GameObject to wrap</param>
        /// <exception cref="ArgumentNullException">Thrown if inner is null</exception>
        internal SafeGameObject(GameObject inner)
        {
            Inner = inner ?? throw new ArgumentNullException(nameof(inner), "GameObject cannot be null");
        }

        /// <summary>
        /// Gets the name of the GameObject.
        /// </summary>
        /// <returns>The GameObject's name, or empty string if null</returns>
        public string GetName()
        {
            return Inner != null ? Inner.name : string.Empty;
        }

        /// <summary>
        /// Gets the world position of the GameObject.
        /// </summary>
        /// <returns>The GameObject's world position, or Vector3.zero if null</returns>
        public Vector3 GetPosition()
        {
            return Inner != null ? Inner.transform.position : Vector3.zero;
        }

        /// <summary>
        /// Sets the world position of the GameObject.
        /// </summary>
        /// <param name="position">The new world position</param>
        public void SetPosition(Vector3 position)
        {
            if (Inner != null)
            {
                Inner.transform.position = position;
            }
        }

        /// <summary>
        /// Gets the rotation of the GameObject as Euler angles.
        /// </summary>
        /// <returns>The GameObject's rotation as Euler angles, or Vector3.zero if null</returns>
        public Vector3 GetRotationEuler()
        {
            return Inner != null ? Inner.transform.rotation.eulerAngles : Vector3.zero;
        }

        /// <summary>
        /// Sets the rotation of the GameObject using Euler angles.
        /// </summary>
        /// <param name="euler">The new rotation as Euler angles (x, y, z)</param>
        public void SetRotationEuler(Vector3 euler)
        {
            if (Inner != null)
            {
                Inner.transform.rotation = Quaternion.Euler(euler);
            }
        }

        /// <summary>
        /// Gets the local scale of the GameObject.
        /// </summary>
        /// <returns>The GameObject's local scale, or Vector3.one if null</returns>
        public Vector3 GetScale()
        {
            return Inner != null ? Inner.transform.localScale : Vector3.one;
        }

        /// <summary>
        /// Sets the local scale of the GameObject.
        /// </summary>
        /// <param name="scale">The new local scale</param>
        public void SetScale(Vector3 scale)
        {
            if (Inner != null)
            {
                Inner.transform.localScale = scale;
            }
        }

        /// <summary>
        /// Sets the active state of the GameObject.
        /// </summary>
        /// <param name="active">True to activate, false to deactivate</param>
        public void SetActive(bool active)
        {
            if (Inner != null)
            {
                Inner.SetActive(active);
            }
        }

        /// <summary>
        /// Gets the parent GameObject of this GameObject.
        /// </summary>
        /// <returns>A SafeGameObject wrapper for the parent, or null if no parent</returns>
        public SafeGameObject GetParent()
        {
            if (Inner == null || Inner.transform.parent == null) return null;
            return new SafeGameObject(Inner.transform.parent.gameObject);
        }

        /// <summary>
        /// Finds a child GameObject by name.
        /// </summary>
        /// <param name="childName">Name of the child to find</param>
        /// <returns>A SafeGameObject wrapper for the child, or null if not found</returns>
        public SafeGameObject GetChildByName(string childName)
        {
            if (Inner == null || string.IsNullOrEmpty(childName)) return null;
            var t = Inner.transform.Find(childName);
            return t != null ? new SafeGameObject(t.gameObject) : null;
        }

        /// <summary>
        /// Gets the forward direction vector of the GameObject.
        /// </summary>
        /// <returns>The GameObject's forward direction, or Vector3.forward if null</returns>
        public Vector3 GetForward()
        {
            return Inner != null ? Inner.transform.forward : Vector3.forward;
        }

        /// <summary>
        /// Destroys the GameObject.
        /// The object will be destroyed at the end of the current frame.
        /// </summary>
        public void Destroy()
        {
            if (Inner != null)
            {
                UnityEngine.Object.Destroy(Inner);
            }
        }

        /// <summary>
        /// Gets the full hierarchy path of the GameObject.
        /// Returns a string like "Root/Environment/Player" showing the object's position in the hierarchy.
        /// </summary>
        /// <returns>The hierarchy path as a string, or empty string if null</returns>
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

    /// <summary>
    /// Contains the results of a raycast operation.
    /// Used by GameAPI.RaycastFromCamera() to return hit information.
    /// </summary>
    public class RaycastResult
    {
        /// <summary>Whether the raycast hit something</summary>
        public bool Hit { get; set; }
        /// <summary>The world position where the ray hit</summary>
        public Vector3 Point { get; set; }
        /// <summary>The surface normal at the hit point</summary>
        public Vector3 Normal { get; set; }
        /// <summary>The GameObject that was hit, wrapped in SafeGameObject</summary>
        public SafeGameObject HitObject { get; set; }
    }
}

