using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SleddingEngineTweaks.API;

namespace SleddingEngineTweaks.UI.SleddingEngineTweaksPanel
{
    public class HierarchyTab : ModTab, IDynamicSizedTab
    {
        private readonly Dictionary<int, bool> _expandedById = new Dictionary<int, bool>();
        private Vector2 _scroll;
        private Vector2 _inspectorScroll;
        private string _filter = "";
        private GameObject _selected;

        public HierarchyTab() : base("Hierarchy") { }
        
        /// <summary>
        /// Gets the requested size for this tab based on current content
        /// </summary>
        public Vector2? GetRequestedSize()
        {
            // Hierarchy tab needs more space for the tree view and inspector
            return new Vector2(600f, 400f); // Minimum size for hierarchy
        }

        public override void Render()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh", GUILayout.Width(90)))
            {
                // no cache to clear; we query live each frame
            }
            GUILayout.Label("Filter:", GUILayout.Width(45));
            _filter = GUILayout.TextField(_filter ?? string.Empty);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            // Left: Hierarchy
            GUILayout.BeginVertical(GUILayout.Width(350));
            _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.ExpandHeight(true));

            var scene = SceneManager.GetActiveScene();
            if (scene.IsValid())
            {
                var roots = scene.GetRootGameObjects();
                foreach (var root in roots)
                {
                    RenderNode(root, 0);
                }
            }
            else
            {
                GUILayout.Label("No active scene");
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            // Right: Inspector
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            RenderInspector();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void RenderNode(GameObject go, int depth)
        {
            if (go == null) return;
            if (!string.IsNullOrEmpty(_filter) && !go.name.ToLower().Contains(_filter.ToLower()))
            {
                // Still render children if parent filtered? Keep simple: skip subtree
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(depth * 14f);

            bool hasChildren = go.transform.childCount > 0;
            int id = go.GetInstanceID();
            bool expanded = GetExpanded(id);

            if (hasChildren)
            {
                if (GUILayout.Button(expanded ? "▼" : "▶", GUILayout.Width(20)))
                {
                    SetExpanded(id, !expanded);
                }
            }
            else
            {
                GUILayout.Space(20);
            }

            if (GUILayout.Button(go.name, GUI.skin.label))
            {
                _selected = go;
            }
            GUILayout.EndHorizontal();

            if (hasChildren && expanded)
            {
                int childCount = go.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = go.transform.GetChild(i).gameObject;
                    RenderNode(child, depth + 1);
                }
            }
        }

        private bool GetExpanded(int id)
        {
            if (_expandedById.TryGetValue(id, out bool v))
            {
                return v;
            }
            return false;
        }

        private void SetExpanded(int id, bool value)
        {
            _expandedById[id] = value;
        }

        private void RenderInspector()
        {
            GUILayout.Label("Inspector", GUI.skin.box);
            if (_selected == null)
            {
                GUILayout.Label("Select an object from the hierarchy.");
                return;
            }

            var safe = new SafeGameObject(_selected);
            GUILayout.Label($"Name: {_selected.name}");
            GUILayout.Label($"Path: {safe.GetPath()}");
            GUILayout.Label($"Active: {_selected.activeSelf}");
            GUILayout.Label($"Tag: {_selected.tag}");

            // Transform
            var pos = _selected.transform.position;
            var rot = _selected.transform.rotation.eulerAngles;
            var scale = _selected.transform.localScale;
            GUILayout.Label("Transform", GUI.skin.box);
            GUILayout.Label($"Position: {pos.x:F2}, {pos.y:F2}, {pos.z:F2}");
            GUILayout.Label($"Rotation: {rot.x:F2}, {rot.y:F2}, {rot.z:F2}");
            GUILayout.Label($"Scale: {scale.x:F2}, {scale.y:F2}, {scale.z:F2}");

            // Components
            GUILayout.Label("Components", GUI.skin.box);
            _inspectorScroll = GUILayout.BeginScrollView(_inspectorScroll, GUILayout.Height(200));
            var components = _selected.GetComponents<Component>();
            foreach (var c in components)
            {
                if (c == null) continue;
                GUILayout.Label(c.GetType().Name);
            }
            GUILayout.EndScrollView();

            // Actions
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Focus"))
            {
                var cam = Camera.main;
                if (cam != null)
                {
                    // Move camera to a position near the object and look at it
                    Vector3 direction = (_selected.transform.position - cam.transform.position).normalized;
                    Vector3 newPosition = _selected.transform.position - direction * 5f; // 5 units away
                    cam.transform.position = newPosition;
                    cam.transform.LookAt(_selected.transform);
                }
            }
            if (GUILayout.Button(_selected.activeSelf ? "Disable" : "Enable"))
            {
                _selected.SetActive(!_selected.activeSelf);
            }
            GUILayout.EndHorizontal();
        }
    }
}

