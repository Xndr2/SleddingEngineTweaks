-- Prefab Example Script for SleddingEngineTweaks
-- This script demonstrates how to use the prefab system with AssetBundles

-- Load the test prefab from AssetBundle
log("Loading test prefab from AssetBundle...")
if prefab.LoadPrefab("set_prefab") then
    log("Successfully loaded set_prefab from AssetBundle")
else
    log("Failed to load set_prefab from AssetBundle")
end

-- Function to spawn a prefab in the world
function spawnInWorld()
    local pos = Vector3(0, 5, 0)
    local rot = Quaternion(0, 0, 0, 1) -- x, y, z, w
    local instance = prefab.InstantiatePrefab("set_prefab", pos, rot)
    if instance then
        log("Spawned prefab in world at " .. tostring(pos))
    else
        log("Failed to spawn prefab in world")
    end
end

-- Function to spawn a prefab on the player's head
function spawnOnHead()
    local pos = Vector3(0, 0.5, 0) -- Slightly above the head
    local rot = Quaternion(0, 0, 0, 1) -- x, y, z, w
    local instance = prefab.InstantiatePrefabOnPlayerHead("set_prefab", pos, rot)
    if instance then
        log("Spawned prefab on player head")
    else
        log("Failed to spawn prefab on player head")
    end
end

-- Function to spawn a prefab on the camera
function spawnOnCamera()
    local pos = Vector3(0, 0, 1) -- In front of the camera
    local rot = Quaternion(0, 0, 0, 1) -- x, y, z, w
    local instance = prefab.InstantiatePrefabOnCamera("set_prefab", pos, rot)
    if instance then
        log("Spawned prefab on camera")
    else
        log("Failed to spawn prefab on camera")
    end
end

-- Function to spawn a prefab on a specific bone
function spawnOnBone(boneName)
    local pos = Vector3(0, 0, 0)
    local rot = Quaternion(0, 0, 0, 1) -- x, y, z, w
    local instance = prefab.InstantiatePrefabOnPlayerBone("set_prefab", boneName, pos, rot)
    if instance then
        log("Spawned prefab on bone: " .. boneName)
    else
        log("Failed to spawn prefab on bone: " .. boneName)
    end
end

-- Function to spawn a prefab as a child of another object
function spawnAsChild(parentName)
    local parent = game.FindGameObject(parentName)
    if parent then
        local pos = Vector3(0, 0, 0)
        local rot = Quaternion(0, 0, 0, 1) -- x, y, z, w
        local instance = prefab.InstantiatePrefabAsChild("set_prefab", parent, pos, rot)
        if instance then
            log("Spawned prefab as child of: " .. parentName)
        else
            log("Failed to spawn prefab as child of: " .. parentName)
        end
    else
        log("Parent object not found: " .. parentName)
    end
end

-- Function to list all loaded prefabs
function listLoadedPrefabs()
    local prefabs = prefab.GetLoadedPrefabNames()
    log("Loaded prefabs:")
    for i, name in ipairs(prefabs) do
        log("  " .. i .. ": " .. name)
    end
end

-- Function to list all instantiated prefabs
function listInstantiatedPrefabs()
    local instances = prefab.GetInstantiatedPrefabNames()
    log("Instantiated prefabs:")
    for i, name in ipairs(instances) do
        log("  " .. i .. ": " .. name)
    end
end

-- Function to cleanup all instances
function cleanupAll()
    prefab.CleanupAllInstances()
    log("Cleaned up all prefab instances")
end

-- Function to destroy a specific instance
function destroyInstance(instanceName)
    if prefab.DestroyPrefabInstance(instanceName) then
        log("Destroyed instance: " .. instanceName)
    else
        log("Failed to destroy instance: " .. instanceName)
    end
end

-- Example usage functions
function example1()
    log("=== Example 1: Basic prefab spawning ===")
    spawnInWorld()
    spawnOnHead()
    spawnOnCamera()
end

function example2()
    log("=== Example 2: Bone parenting ===")
    -- Try common bone names
    spawnOnBone("Head")
    spawnOnBone("Neck")
    spawnOnBone("Spine")
    spawnOnBone("LeftHand")
    spawnOnBone("RightHand")
end

function example3()
    log("=== Example 3: Object parenting ===")
    -- Try to find and parent to common objects
    spawnAsChild("Player")
    spawnAsChild("Main Camera")
    spawnAsChild("Environment")
end

-- Help function
function prefabHelp()
    log("=== Prefab System Help ===")
    log("Available functions:")
    log("  spawnInWorld() - Spawn prefab in world")
    log("  spawnOnHead() - Spawn prefab on player head")
    log("  spawnOnCamera() - Spawn prefab on camera")
    log("  spawnOnBone(boneName) - Spawn prefab on specific bone")
    log("  spawnAsChild(parentName) - Spawn prefab as child of object")
    log("  listLoadedPrefabs() - List all loaded prefabs")
    log("  listInstantiatedPrefabs() - List all instantiated prefabs")
    log("  cleanupAll() - Destroy all instances")
    log("  destroyInstance(name) - Destroy specific instance")
    log("  example1(), example2(), example3() - Run examples")
    log("  prefabHelp() - Show this help")
end

-- Auto-run example on load
log("Prefab example script loaded!")
log("Type 'prefabHelp()' for available functions")
log("Type 'example1()' to run basic examples")
log("Note: This system uses AssetBundles - place your .bundle files in the Assets folder")

-- Run a simple example automatically
spawnInWorld()
