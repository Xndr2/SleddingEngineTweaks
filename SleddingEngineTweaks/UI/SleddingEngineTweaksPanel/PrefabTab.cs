using System;
using System.Collections.Generic;
using UnityEngine;
using SleddingEngineTweaks.API;
using SleddingEngineTweaks.UI;
using SleddingEngineTweaks.UI.Options.Base;
using SleddingEngineTweaks.UI.Options.Options;

namespace SleddingEngineTweaks.UI.SleddingEngineTweaksPanel
{
    public class PrefabTab : ModTab, IDynamicSizedTab
    {
        private string _selectedPrefab = "";
        private string _newPrefabName = "";
        private Vector3 _spawnPosition = Vector3.zero;
        private Vector3 _spawnRotation = Vector3.zero;
        private string _parentBoneName = "";
        private string _parentObjectName = "";
        private int _parentType = 0; // 0 = World, 1 = Player Head, 2 = Camera, 3 = Player Bone, 4 = Custom Object
        private string[] _parentTypeNames = { "World", "Player Head", "Camera", "Player Bone", "Custom Object" };
        private List<string> _loadedPrefabs = new List<string>();
        private List<string> _instantiatedPrefabs = new List<string>();
        private bool _showAdvancedOptions = false;

        public PrefabTab() : base("Prefabs")
        {
            RefreshPrefabLists();
        }
        
        /// <summary>
        /// Gets the requested size for this tab based on current content
        /// </summary>
        public Vector2? GetRequestedSize()
        {
            // If advanced options are open, request additional height
            if (_showAdvancedOptions)
            {
                return new Vector2(0, 200f); // Additional height needed
            }
            
            return null; // No specific size requested
        }

        public override void Render()
        {
            GUILayout.BeginVertical();

            // Header
            GUILayout.Label("Prefab Manager", GUI.skin.box);
            GUILayout.Space(10);

            // Load Prefab Section
            GUILayout.Label("Load Prefab", GUI.skin.box);
            GUILayout.BeginHorizontal();
            GUILayout.Label("AssetBundle Name:", GUILayout.Width(120));
            _newPrefabName = GUILayout.TextField(_newPrefabName, GUILayout.Width(150));
            if (GUILayout.Button("Load", GUILayout.Width(60)))
            {
                LoadPrefab();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Test Prefab", GUILayout.Width(120)))
            {
                LoadTestPrefab();
            }
            if (GUILayout.Button("Refresh Lists", GUILayout.Width(100)))
            {
                RefreshPrefabLists();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Prefab Selection
            GUILayout.Label("Loaded Prefabs", GUI.skin.box);
            if (_loadedPrefabs.Count > 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Select Prefab:", GUILayout.Width(100));
                int selectedIndex = Mathf.Max(0, _loadedPrefabs.IndexOf(_selectedPrefab));
                selectedIndex = GUILayout.SelectionGrid(selectedIndex, _loadedPrefabs.ToArray(), Mathf.Min(3, _loadedPrefabs.Count), GUILayout.Width(400));
                if (selectedIndex >= 0 && selectedIndex < _loadedPrefabs.Count)
                {
                    _selectedPrefab = _loadedPrefabs[selectedIndex];
                }
                GUILayout.EndHorizontal();
                
                if (!string.IsNullOrEmpty(_selectedPrefab))
                {
                    GUILayout.Label($"Selected: {_selectedPrefab}", GUI.skin.box);
                }
            }
            else
            {
                GUILayout.Label("No prefabs loaded. Load an AssetBundle first.", GUI.skin.box);
            }
            GUILayout.Space(10);

            // Spawn Options
            if (!string.IsNullOrEmpty(_selectedPrefab))
            {
                GUILayout.Label("Spawn Options", GUI.skin.box);
                
                // Parent Type Selection
                GUILayout.BeginHorizontal();
                GUILayout.Label("Parent to:", GUILayout.Width(80));
                _parentType = GUILayout.SelectionGrid(_parentType, _parentTypeNames, 3, GUILayout.Width(300));
                GUILayout.EndHorizontal();

                // Position and Rotation
                GUILayout.BeginHorizontal();
                GUILayout.Label("Position:", GUILayout.Width(80));
                _spawnPosition.x = float.Parse(GUILayout.TextField(_spawnPosition.x.ToString("F2"), GUILayout.Width(60)));
                _spawnPosition.y = float.Parse(GUILayout.TextField(_spawnPosition.y.ToString("F2"), GUILayout.Width(60)));
                _spawnPosition.z = float.Parse(GUILayout.TextField(_spawnPosition.z.ToString("F2"), GUILayout.Width(60)));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Rotation:", GUILayout.Width(80));
                _spawnRotation.x = float.Parse(GUILayout.TextField(_spawnRotation.x.ToString("F2"), GUILayout.Width(60)));
                _spawnRotation.y = float.Parse(GUILayout.TextField(_spawnRotation.y.ToString("F2"), GUILayout.Width(60)));
                _spawnRotation.z = float.Parse(GUILayout.TextField(_spawnRotation.z.ToString("F2"), GUILayout.Width(60)));
                GUILayout.EndHorizontal();

                // Parent-specific options
                if (_parentType == 3) // Player Bone
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Bone Name:", GUILayout.Width(80));
                    _parentBoneName = GUILayout.TextField(_parentBoneName, GUILayout.Width(150));
                    GUILayout.EndHorizontal();
                }
                else if (_parentType == 4) // Custom Object
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Object Name:", GUILayout.Width(80));
                    _parentObjectName = GUILayout.TextField(_parentObjectName, GUILayout.Width(150));
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(10);

                // Spawn Buttons
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Spawn in World", GUILayout.Width(120)))
                {
                    SpawnInWorld();
                }
                if (GUILayout.Button("Spawn on Player Head", GUILayout.Width(150)))
                {
                    SpawnOnPlayerHead();
                }
                if (GUILayout.Button("Spawn on Camera", GUILayout.Width(130)))
                {
                    SpawnOnCamera();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Spawn on Player Bone", GUILayout.Width(150)))
                {
                    SpawnOnPlayerBone();
                }
                if (GUILayout.Button("Spawn on Custom Object", GUILayout.Width(160)))
                {
                    SpawnOnCustomObject();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
            }

            // Advanced Options
            _showAdvancedOptions = GUILayout.Toggle(_showAdvancedOptions, "Advanced Options");
            if (_showAdvancedOptions)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                
                // Quick Actions
                GUILayout.Label("Quick Actions", GUI.skin.box);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Refresh Lists", GUILayout.Width(100)))
                {
                    RefreshPrefabLists();
                }
                if (GUILayout.Button("Load Test Prefab", GUILayout.Width(120)))
                {
                    LoadTestPrefab();
                }
                if (GUILayout.Button("Cleanup All", GUILayout.Width(100)))
                {
                    CleanupAllInstances();
                }
                GUILayout.EndHorizontal();

                // Instantiated Prefabs
                GUILayout.Space(10);
                GUILayout.Label("Instantiated Prefabs", GUI.skin.box);
                if (_instantiatedPrefabs.Count > 0)
                {
                    // Create a copy to avoid collection modification during iteration
                    var instancesCopy = new List<string>(_instantiatedPrefabs);
                    foreach (string instanceName in instancesCopy)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(instanceName, GUILayout.Width(200));
                        if (GUILayout.Button("Destroy", GUILayout.Width(60)))
                        {
                            DestroyInstance(instanceName);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    GUILayout.Label("No prefabs instantiated", GUI.skin.box);
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }

        private void LoadPrefab()
        {
            if (string.IsNullOrEmpty(_newPrefabName))
            {
                Plugin.StaticLogger.LogWarning("Prefab name cannot be empty");
                return;
            }

            if (Plugin.PrefabAPI.LoadPrefab(_newPrefabName))
            {
                RefreshPrefabLists();
                _selectedPrefab = _newPrefabName;
                _newPrefabName = "";
            }
            else
            {
                Plugin.StaticLogger.LogError($"Failed to load prefab: {_newPrefabName}");
            }
        }

        private void LoadTestPrefab()
        {
            if (Plugin.PrefabAPI.LoadPrefab("set_prefab"))
            {
                RefreshPrefabLists();
                _selectedPrefab = "set_prefab";
                Plugin.StaticLogger.LogInfo("Loaded test prefab: set_prefab");
            }
            else
            {
                Plugin.StaticLogger.LogError("Failed to load test prefab: set_prefab");
            }
        }

        private void SpawnInWorld()
        {
            if (string.IsNullOrEmpty(_selectedPrefab)) return;

            var instance = Plugin.PrefabAPI.InstantiatePrefab(_selectedPrefab, _spawnPosition, Quaternion.Euler(_spawnRotation));
            if (instance != null)
            {
                RefreshPrefabLists();
                Plugin.StaticLogger.LogInfo($"Spawned {_selectedPrefab} in world at {_spawnPosition}");
            }
        }

        private void SpawnOnPlayerHead()
        {
            if (string.IsNullOrEmpty(_selectedPrefab)) return;

            var instance = Plugin.PrefabAPI.InstantiatePrefabOnPlayerHead(_selectedPrefab, _spawnPosition, Quaternion.Euler(_spawnRotation));
            if (instance != null)
            {
                RefreshPrefabLists();
                Plugin.StaticLogger.LogInfo($"Spawned {_selectedPrefab} on player head");
            }
        }

        private void SpawnOnCamera()
        {
            if (string.IsNullOrEmpty(_selectedPrefab)) return;

            var instance = Plugin.PrefabAPI.InstantiatePrefabOnCamera(_selectedPrefab, _spawnPosition, Quaternion.Euler(_spawnRotation));
            if (instance != null)
            {
                RefreshPrefabLists();
                Plugin.StaticLogger.LogInfo($"Spawned {_selectedPrefab} on camera");
            }
        }

        private void SpawnOnPlayerBone()
        {
            if (string.IsNullOrEmpty(_selectedPrefab) || string.IsNullOrEmpty(_parentBoneName)) return;

            var instance = Plugin.PrefabAPI.InstantiatePrefabOnPlayerBone(_selectedPrefab, _parentBoneName, _spawnPosition, Quaternion.Euler(_spawnRotation));
            if (instance != null)
            {
                RefreshPrefabLists();
                Plugin.StaticLogger.LogInfo($"Spawned {_selectedPrefab} on player bone: {_parentBoneName}");
            }
        }

        private void SpawnOnCustomObject()
        {
            if (string.IsNullOrEmpty(_selectedPrefab) || string.IsNullOrEmpty(_parentObjectName)) return;

            var parentObject = Plugin.GameAPI.FindGameObject(_parentObjectName);
            if (parentObject == null)
            {
                Plugin.StaticLogger.LogError($"Object not found: {_parentObjectName}");
                return;
            }

            var instance = Plugin.PrefabAPI.InstantiatePrefabAsChild(_selectedPrefab, parentObject, _spawnPosition, Quaternion.Euler(_spawnRotation));
            if (instance != null)
            {
                RefreshPrefabLists();
                Plugin.StaticLogger.LogInfo($"Spawned {_selectedPrefab} on object: {_parentObjectName}");
            }
        }

        private void DestroyInstance(string instanceName)
        {
            if (Plugin.PrefabAPI.DestroyPrefabInstance(instanceName))
            {
                RefreshPrefabLists();
                Plugin.StaticLogger.LogInfo($"Destroyed instance: {instanceName}");
            }
        }

        private void CleanupAllInstances()
        {
            Plugin.PrefabAPI.CleanupAllInstances();
            RefreshPrefabLists();
            Plugin.StaticLogger.LogInfo("Cleaned up all prefab instances");
        }

        private void RefreshPrefabLists()
        {
            _loadedPrefabs.Clear();
            _instantiatedPrefabs.Clear();

            if (Plugin.PrefabAPI != null)
            {
                var loadedNames = Plugin.PrefabAPI.GetLoadedPrefabNames();
                var instantiatedNames = Plugin.PrefabAPI.GetInstantiatedPrefabNames();

                _loadedPrefabs.AddRange(loadedNames);
                _instantiatedPrefabs.AddRange(instantiatedNames);
            }
        }
    }
}
