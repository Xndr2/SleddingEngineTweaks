# Prefab System for SleddingEngineTweaks

This document describes the prefab system that allows modders to quickly import and load prefabs into the game with various parenting options.

## Features

- **AssetBundle Loading**: Load prefabs from AssetBundle files in the `Assets` folder
- **Multiple Parenting Options**: Parent prefabs to world, player head, camera, bones, or custom objects
- **Lua Scripting Support**: Full Lua API for prefab operations
- **UI Management**: Easy-to-use interface for prefab management
- **Memory Management**: Automatic cleanup and disposal of prefabs and AssetBundles

## File Structure

```
SleddingEngineTweaks/
├── API/
│   ├── PrefabManager.cs      # Core prefab management logic
│   └── PrefabAPI.cs          # Lua-exposed API
├── UI/SleddingEngineTweaksPanel/
│   └── PrefabTab.cs          # UI for prefab management
├── Scripts/
│   └── prefab_example.lua    # Example Lua script
└── Assets/
    └── set_prefab            # Test prefab file
```

## Usage

### 1. Loading Prefabs

#### AssetBundle Requirements
- Place your AssetBundle files in the `Assets` folder
- AssetBundle files can have extensions: `.bundle`, `.assetbundle`, or no extension
- The prefab inside the AssetBundle should be named the same as the bundle file, or use common names like "prefab", "Prefab", "PREFAB", "Yes", "yes"

#### Via UI
1. Open the SleddingEngineTweaks panel
2. Go to the "Prefabs" tab
3. Enter the AssetBundle name (without extension) in the "Load Prefab" field
4. Click "Load" or use "Load Test Prefab" for the test prefab

#### Via Lua
```lua
-- Load a prefab from AssetBundle
prefab.LoadPrefab("set_prefab")

-- Check if a prefab is loaded
if prefab.IsPrefabLoaded("set_prefab") then
    log("Prefab is loaded!")
end
```

### 2. Instantiating Prefabs

#### Spawning in World
```lua
-- Spawn at specific world position
local pos = Vector3(0, 5, 0)
local rot = Quaternion(0, 0, 0, 1) -- x, y, z, w
local instance = prefab.InstantiatePrefab("set_prefab", pos, rot)

-- Or use Vector3 for rotation (automatically converted to Quaternion)
local rotVec = Vector3(0, 0, 0) -- Will be converted to Quaternion.Euler(rotVec)
local instance2 = prefab.InstantiatePrefab("set_prefab", pos, rotVec)
```

#### Spawning on Player Head
```lua
-- Spawn attached to player's head
local pos = Vector3(0, 0.5, 0) -- Slightly above head
local rot = Quaternion(0, 0, 0, 1) -- x, y, z, w
local instance = prefab.InstantiatePrefabOnPlayerHead("set_prefab", pos, rot)
```

#### Spawning on Camera
```lua
-- Spawn attached to main camera
local pos = Vector3(0, 0, 1) -- In front of camera
local rot = Quaternion(0, 0, 0, 1) -- x, y, z, w
local instance = prefab.InstantiatePrefabOnCamera("set_prefab", pos, rot)
```

#### Spawning on Player Bone
```lua
-- Spawn attached to specific bone in player skeleton
local boneName = "Head" -- or "Neck", "Spine", "LeftHand", etc.
local pos = Vector3(0, 0, 0)
local rot = Quaternion(0, 0, 0, 1) -- x, y, z, w
local instance = prefab.InstantiatePrefabOnPlayerBone("set_prefab", boneName, pos, rot)
```

#### Spawning as Child of Custom Object
```lua
-- Find an object and parent the prefab to it
local parent = game.FindGameObject("SomeObjectName")
if parent then
    local pos = Vector3(0, 0, 0)
    local rot = Quaternion(0, 0, 0, 1) -- x, y, z, w
    local instance = prefab.InstantiatePrefabAsChild("set_prefab", parent, pos, rot)
end
```

### 3. Managing Instances

#### Listing Instances
```lua
-- List all loaded prefabs
local prefabs = prefab.GetLoadedPrefabNames()
for i, name in ipairs(prefabs) do
    log("Loaded prefab " .. i .. ": " .. name)
end

-- List all instantiated prefabs
local instances = prefab.GetInstantiatedPrefabNames()
for i, name in ipairs(instances) do
    log("Instance " .. i .. ": " .. name)
end
```

#### Destroying Instances
```lua
-- Destroy a specific instance
prefab.DestroyPrefabInstance("instance_name")

-- Clean up all instances
prefab.CleanupAllInstances()
```

### 4. UI Controls

The PrefabTab provides the following controls:

- **Load Prefab**: Text field and button to load new prefabs
- **Prefab Selection**: Grid selection of loaded prefabs
- **Spawn Options**:
  - Parent Type: World, Player Head, Camera, Player Bone, Custom Object
  - Position: X, Y, Z coordinates
  - Rotation: X, Y, Z rotation values
  - Bone Name: For bone parenting
  - Object Name: For custom object parenting
- **Spawn Buttons**: Quick spawn buttons for common scenarios
- **Advanced Options**: 
  - Refresh Lists
  - Load Test Prefab
  - Cleanup All
  - Instance Management

## API Reference

### PrefabAPI Methods

| Method | Description | Parameters | Returns |
|--------|-------------|------------|---------|
| `LoadPrefab(name)` | Load a prefab from assets | `name` (string) | `bool` |
| `InstantiatePrefab(name, pos, rot)` | Spawn prefab in world | `name`, `pos` (Vector3), `rot` (Quaternion) | `SafeGameObject` |
| `InstantiatePrefabAsChild(name, parent, pos, rot)` | Spawn as child of object | `name`, `parent` (SafeGameObject), `pos`, `rot` | `SafeGameObject` |
| `InstantiatePrefabOnPlayerHead(name, pos, rot)` | Spawn on player head | `name`, `pos`, `rot` | `SafeGameObject` |
| `InstantiatePrefabOnCamera(name, pos, rot)` | Spawn on camera | `name`, `pos`, `rot` | `SafeGameObject` |
| `InstantiatePrefabOnPlayerBone(name, bone, pos, rot)` | Spawn on player bone | `name`, `bone` (string), `pos`, `rot` | `SafeGameObject` |
| `DestroyPrefabInstance(name)` | Destroy specific instance | `name` (string) | `bool` |
| `GetLoadedPrefabNames()` | Get list of loaded prefabs | None | `string[]` |
| `GetInstantiatedPrefabNames()` | Get list of instances | None | `string[]` |
| `IsPrefabLoaded(name)` | Check if prefab is loaded | `name` (string) | `bool` |
| `UnloadPrefab(name)` | Unload prefab from memory | `name` (string) | `bool` |
| `CleanupAllInstances()` | Destroy all instances | None | `void` |

## Examples

### Basic Usage
```lua
-- Load and spawn a prefab
prefab.LoadPrefab("set_prefab")
local instance = prefab.InstantiatePrefab("set_prefab", Vector3(0, 5, 0), Vector3(0, 0, 0))
```

### Advanced Parenting
```lua
-- Spawn multiple prefabs with different parenting
prefab.LoadPrefab("set_prefab")

-- On player head
prefab.InstantiatePrefabOnPlayerHead("set_prefab", Vector3(0, 0.5, 0), Vector3(0, 0, 0))

-- On camera
prefab.InstantiatePrefabOnCamera("set_prefab", Vector3(0, 0, 1), Vector3(0, 0, 0))

-- On specific bone
prefab.InstantiatePrefabOnPlayerBone("set_prefab", "LeftHand", Vector3(0, 0, 0), Vector3(0, 0, 0))
```

### Cleanup
```lua
-- Clean up everything
prefab.CleanupAllInstances()
prefab.UnloadPrefab("set_prefab")
```

## Notes

- Prefabs are loaded from AssetBundle files in the `Assets` folder in the plugin directory
- The system uses Unity's AssetBundle system for proper prefab loading
- All instantiated prefabs and AssetBundles are tracked and can be cleaned up
- The system is designed to be safe and prevent memory leaks
- Lua scripts have full access to the prefab API through the `prefab` global
- AssetBundles are automatically unloaded when prefabs are unloaded

## Troubleshooting

- **Prefab not loading**: Check that the AssetBundle file exists in the Assets folder and contains a prefab with a recognized name
- **AssetBundle not found**: Ensure the file has a proper extension (.bundle, .assetbundle, or no extension)
- **Prefab not found in bundle**: Check that the prefab inside the AssetBundle has a recognized name (same as bundle name, or "prefab", "Prefab", "PREFAB", "Yes", "yes")
- **Parent object not found**: Verify the object name exists in the scene
- **Bone not found**: Check that the bone name exists in the player skeleton
- **Memory issues**: Use `CleanupAllInstances()` to free up memory and `UnloadPrefab()` to unload AssetBundles
