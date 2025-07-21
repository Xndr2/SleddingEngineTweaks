# Sledding Engine Tweaks

## ⚠️ Important Notice
**Please be aware that Sledding Engine Tweaks is currently under active development and is not yet feature-complete.** Many functionalities may be missing or not fully operational.

This framework is primarily being developed for the upcoming **Sledding Game** on Steam. However, since the game is not yet released, current development and testing of the base framework are being conducted using **Lethal Company**. It should also be compatible with other games built with **Unity 2022.3.9**.

---

Welcome to Sledding Engine Tweaks (SET), a powerful modding framework for Unity games, built on BepInEx. SET provides an advanced Lua scripting layer that allows for the rapid development of custom mods without needing to recompile or restart the game.
Create new UI, manipulate game objects, listen for keyboard input, change game settings, and much more, all from simple `.lua` script files.

## Features
- 🟩**Lua Scripting:** Use the MoonSharp Lua interpreter to run your mods.
- 🟩**Dynamic In-Game UI:** Create your own mod panels with tabs, buttons, labels, and toggles directly from Lua. All UI elements can have their own scriptable callbacks.
- 🟩**Persistent Configuration:** Scripts can save and load their own settings, which persist between game sessions.
- 🟩**Advanced GameObject Management:** Find, manipulate, and interact with game objects with built-in caching for performance.

## Planned
- 🟧**Built-in Event System:** Hook into core game events like `OnUpdate` and `OnSceneLoaded` to trigger your script's logic.
- 🟧**Dual API System:** Two distinct APIs are exposed to Lua scripts - `game` for game interactions and `set` for UI management.
- 🟥**Vr support:** Support for VR.
- 🟥**Live Script Loading:** Drop `.lua` files into the scripts folder, and they are loaded automatically. No restart required!
- 🟥**Drag and drop file importer:** Drag and drop files.
- 🟥**Custom clothes, hats, sleds:**
- 🟥**Custom frog color:**
- 🟥**Custom maps:**
- 🟥**Dedicated server utils**
- 🟥**Admin tools**

## For Players: Installation
1. Make sure you have [BepInEx](https://github.com/BepInEx/BepInEx) installed for your game.
2. Download the latest release of `SleddingEngineTweaks.dll` from the releases page.
3. Place the `SleddingEngineTweaks.dll` file into your `BepInEx/plugins/` folder.
4. Run the game once. This will generate a new folder: `BepInEx/plugins/SleddingEngineTweaks/`.
5. Inside the `SleddingEngineTweaks` folder, you will find a `scripts` subfolder. This is where you will place any `.lua` mods you download.

By default, press the **Delete** key to toggle the main UI panel in-game.

## For Developers
There are two primary ways to use this framework:

1.  **Via Lua Scripts:** The easiest method is to write small `.lua` scripts. This approach allows for rapid development without compiling code and is ideal for most mods.
2.  **Via Custom BepInEx Plugins:** For more advanced modifications, you can use the `SleddingEngineTweaks.dll` as a dependency in your own C# BepInEx plugin project. This gives you direct access to the framework's APIs and allows for more complex and powerful creations. You are not required to write Lua scripts if you choose this method.

### Creating Your First Script
Creating mods with SET is designed to be straightforward. All you need is a text editor.
#### Getting Started: File Structure
1. Navigate to your game's `BepInEx/plugins/SleddingEngineTweaks/` folder.
2. Create a new folder inside called `scripts`.
3. Create a new file inside `scripts` with a `.lua` extension (e.g., `MyFirstMod.lua`).
4. Open this file in your editor and start scripting!

### Core Scripting Events
Your script can implement special global functions that SET will call automatically at certain times.
- `OnUpdate(deltaTime)`: Called every single frame. `deltaTime` is the time in seconds since the last frame. Ideal for continuous checks or actions.
- `OnSceneLoaded(sceneName)`: Called whenever a new scene is loaded in the game. `sceneName` is the name of the new scene.

**Example:**
```lua -- ExampleMod.lua
-- This function will run every frame
function OnUpdate(deltaTime)
    -- The built-in log function prints to the BepInEx console. 
    log("Game is updating! Delta time: " .. tostring(deltaTime))
end

-- This function will run when the main menu loads 
function OnSceneLoaded(sceneName) 
    if sceneName == "MainMenu" then -- Or whatever scene name you want to use
        log("Welcome to the main menu!") 
    end
end
``` 

## Dual API System

SET exposes two distinct APIs to Lua scripts, each serving different purposes:

### The `game` API: Game Interaction
The `game` API provides access to game-specific functionality like player control, input handling, configuration, and game object manipulation.

#### **Logging & Debugging**

| Function Signature | Description |
| --- | --- |
| `game.Log(message)` | Prints a message to the BepInEx console, prefixed with `[GameAPI]`. |
| `game.DrawDebugLine(start, end, duration)` | Draws a white line in the game world from a start `Vector3` to an end `Vector3` for the specified duration. |

#### **Player Control**

| Function Signature | Description |
| --- | --- |
| `game.GetPlayer()` | Returns the player GameObject (or null if not found). |
| `game.GetPlayerPosition()` | Returns the player's current `Vector3` position. |
| `game.SetPlayerPosition(pos)` | Teleports the player to the given `Vector3` position `pos`. |

#### **Input Handling**

| Function Signature | Description |
| --- | --- |
| `game.IsKeyDown(keyName)` | Returns `true` if the specified key is currently held down. (e.g., "W") |
| `game.WasKeyPressedThisFrame(keyName)` | Returns `true` only on the single frame the key was first pressed down. |

*Note: `keyName` is a string and matches the values in the `UnityEngine.InputSystem.Key` enum (e.g., "Space", "LeftShift", "F5").*

#### **Configuration**
Save and load settings for your mod. All settings are stored in BepInEx's standard config files.

| Function Signature | Description |
| --- | --- |
| `game.GetConfigValue(section, key, default)` | Retrieves a string value from your mod's config. If it doesn't exist, it's created with the default value. |
| `game.SetConfigValue(section, key, value)` | Sets a string value in your mod's config and saves it to the file. |

#### **Game State & Utilities**

| Function Signature | Description |
| --- | --- |
| `game.IsGamePaused()` | Returns `true` if the game is currently paused (timeScale = 0). |
| `game.SetTimeScale(scale)` | Sets the game's time scale (0 = paused, 1 = normal, 2 = double speed, etc.). |
| `game.GetCurrentSceneName()` | Returns the name of the currently loaded scene. |

#### **GameObject Management**
Advanced game object manipulation with built-in caching for performance.

| Function Signature | Description |
| --- | --- |
| `game.FindGameObject(name)` | Finds a GameObject by name, using a cache for performance. Returns the GameObject or null. |
| `game.GetObjectPosition(obj)` | Returns the `Vector3` position of the specified GameObject. |
| `game.SetObjectPosition(obj, position)` | Sets the position of the specified GameObject to the given `Vector3`. |

### The `set` API: UI Management
The `set` API is dedicated to creating and managing custom UI panels, tabs, and controls within the main SET interface.

#### **Panel Management**

| Function Signature | Description | Return Value |
| --- | --- | --- |
| `set.RegisterModPanel(modName)` | Creates a new, draggable window for your mod. Panels remember their position and size between sessions. | `SleddingAPIStatus.Ok` on success, error status on failure |
| `set.GetModPanel(modName)` | Retrieves a mod panel by name. | `ModPanel` object or `null` if not found |
| `set.GetAllModPanels()` | Returns all registered mod panels. | Dictionary of all panels |

#### **Tab Management**

| Function Signature | Description | Return Value |
| --- | --- | --- |
| `set.RegisterModTab(modName, tabName)` | Adds a new tab to your mod's panel. | `SleddingAPIStatus.Ok` on success, error status on failure |
| `set.RegisterModTab(modName, modTab)` | Adds a custom ModTab object to your mod's panel. | `SleddingAPIStatus.Ok` on success, error status on failure |

#### **UI Controls**

| Function Signature | Description | Return Value |
| --- | --- | --- |
| `set.RegisterLabelOption(modName, tabName, labelName)` | Adds a simple text label to a tab. | `SleddingAPIStatus.Ok` on success, error status on failure |
| `set.RegisterButtonOption(modName, tabName, buttonName, callback)` | Adds a clickable button. The `callback` is a Lua function that runs when the button is pressed. | `SleddingAPIStatus.Ok` on success, error status on failure |
| `set.RegisterOption(modName, tabName, modOption)` | Adds a custom ModOption object to a tab. | `SleddingAPIStatus.Ok` on success, error status on failure |

#### **UI Updates**

| Function Signature | Description | Return Value |
| --- | --- | --- |
| `set.UpdateOption(modName, tabName, oldText, newText, optionType)` | Updates an existing option's text. | `SleddingAPIStatus.Ok` on success, error status on failure |

### API Status Codes
The `set` API returns status codes to indicate success or failure:

| Status | Description |
| --- | --- |
| `SleddingAPIStatus.Ok` | Operation completed successfully |
| `SleddingAPIStatus.UnknownError` | An unexpected error occurred |
| `SleddingAPIStatus.ModPanelNotFound` | The specified mod panel does not exist |
| `SleddingAPIStatus.ModPanelAlreadyRegistered` | A panel with that name already exists |
| `SleddingAPIStatus.ModTabNotFound` | The specified tab does not exist |
| `SleddingAPIStatus.ModTabAlreadyRegistered` | A tab with that name already exists |

## Complete Examples

### Example 1: Simple UI Mod
This script creates a basic UI panel with a button and demonstrates the dual API structure.

```lua
-- SimpleUIMod.lua
local modName = "SimpleUIMod"

function OnScriptLoad()
    log(modName .. " script has loaded!")
    
    -- Create UI using the set API
    local status = set.RegisterModPanel(modName)
    if status == SleddingAPIStatus.Ok then
        log("Panel created successfully!")
    else
        log("Failed to create panel: " .. tostring(status))
    end
    
    set.RegisterModTab(modName, "Main")
    set.RegisterLabelOption(modName, "Main", "Welcome to " .. modName)
    
    -- Add a button that uses the game API
    set.RegisterButtonOption(modName, "Main", "Get Player Position", function()
        local pos = game.GetPlayerPosition()
        log("Player position: X=" .. pos.x .. ", Y=" .. pos.y .. ", Z=" .. pos.z)
    end)
end

-- Initialize the mod
OnScriptLoad()
```

### Example 2: Advanced Teleporter Mod
This script creates a comprehensive teleporter with UI controls and demonstrates both APIs working together.

```lua
-- AdvancedTeleporter.lua
local modName = "AdvancedTeleporter"
local savedPositions = {}

function OnScriptLoad()
    log(modName .. " script has loaded!")
    
    -- Load saved positions from config
    local posCount = tonumber(game.GetConfigValue(modName, "PositionCount", "0"))
    for i = 1, posCount do
        local x = tonumber(game.GetConfigValue(modName, "Pos" .. i .. "_X", "0"))
        local y = tonumber(game.GetConfigValue(modName, "Pos" .. i .. "_Y", "100"))
        local z = tonumber(game.GetConfigValue(modName, "Pos" .. i .. "_Z", "0"))
        savedPositions[i] = Vector3(x, y, z)
    end
    
    -- Create UI
    set.RegisterModPanel(modName)
    set.RegisterModTab(modName, "Teleporter")
    set.RegisterModTab(modName, "Settings")
    
    -- Add teleporter controls
    set.RegisterLabelOption(modName, "Teleporter", "Press F5 to save current position")
    set.RegisterLabelOption(modName, "Teleporter", "Press F6 to teleport to last saved position")
    
    set.RegisterButtonOption(modName, "Teleporter", "Save Position", function()
        SaveCurrentPosition()
    end)
    
    set.RegisterButtonOption(modName, "Teleporter", "Teleport to Saved", function()
        TeleportToSaved()
    end)
    
    -- Add settings
    set.RegisterLabelOption(modName, "Settings", "Teleporter Settings")
    set.RegisterButtonOption(modName, "Settings", "Clear All Positions", function()
        ClearAllPositions()
    end)
end

function SaveCurrentPosition()
    local playerPos = game.GetPlayerPosition()
    table.insert(savedPositions, playerPos)
    
    -- Save to config
    local posCount = #savedPositions
    game.SetConfigValue(modName, "PositionCount", tostring(posCount))
    game.SetConfigValue(modName, "Pos" .. posCount .. "_X", tostring(playerPos.x))
    game.SetConfigValue(modName, "Pos" .. posCount .. "_Y", tostring(playerPos.y))
    game.SetConfigValue(modName, "Pos" .. posCount .. "_Z", tostring(playerPos.z))
    
    log("Position saved! Total positions: " .. posCount)
end

function TeleportToSaved()
    if #savedPositions > 0 then
        local lastPos = savedPositions[#savedPositions]
        game.SetPlayerPosition(lastPos)
        log("Teleported to saved position!")
    else
        log("No saved positions found!")
    end
end

function ClearAllPositions()
    savedPositions = {}
    game.SetConfigValue(modName, "PositionCount", "0")
    log("All positions cleared!")
end

function OnUpdate(deltaTime)
    -- Check for hotkeys
    if game.WasKeyPressedThisFrame("F5") then
        SaveCurrentPosition()
    end
    
    if game.WasKeyPressedThisFrame("F6") then
        TeleportToSaved()
    end
end

-- Initialize the mod
OnScriptLoad()
```

### Example 3: Game Object Manipulator
This script demonstrates advanced game object manipulation and scene management.

```lua
-- GameObjectManipulator.lua
local modName = "GameObjectManipulator"

function OnScriptLoad()
    log(modName .. " script has loaded!")
    
    set.RegisterModPanel(modName)
    set.RegisterModTab(modName, "Objects")
    set.RegisterModTab(modName, "Debug")
    
    -- Add object manipulation controls
    set.RegisterButtonOption(modName, "Objects", "Find Player", function()
        local player = game.GetPlayer()
        if player then
            log("Player found: " .. player.name)
        else
            log("Player not found!")
        end
    end)
    
    set.RegisterButtonOption(modName, "Objects", "List Scene Objects", function()
        ListSceneObjects()
    end)
    
    -- Add debug controls
    set.RegisterButtonOption(modName, "Debug", "Draw Debug Line", function()
        local playerPos = game.GetPlayerPosition()
        local endPos = playerPos + Vector3(0, 10, 0)
        game.DrawDebugLine(playerPos, endPos, 5.0)
        log("Debug line drawn for 5 seconds!")
    end)
    
    set.RegisterButtonOption(modName, "Debug", "Toggle Game Pause", function()
        if game.IsGamePaused() then
            game.SetTimeScale(1.0)
            log("Game unpaused")
        else
            game.SetTimeScale(0.0)
            log("Game paused")
        end
    end)
end

function ListSceneObjects()
    log("Current scene: " .. game.GetCurrentSceneName())
    log("Game time scale: " .. tostring(Time.timeScale))
    
    -- This is a simplified example - in practice you'd iterate through actual scene objects
    log("Scene objects would be listed here...")
end

function OnSceneLoaded(sceneName)
    log("Scene loaded: " .. sceneName)
    log("Scene objects cache cleared automatically")
end

-- Initialize the mod
OnScriptLoad()
```

## Best Practices

### API Usage Guidelines
1. **Use the appropriate API**: Use `game` for game interactions and `set` for UI management
2. **Check return values**: Always check the return status from `set` API calls
3. **Error handling**: Implement proper error handling for API calls
4. **Performance**: Use the built-in caching for GameObject operations
5. **Configuration**: Save important data using the config system

### Script Organization
1. **Initialize in OnScriptLoad**: Set up UI and load configuration when the script loads
2. **Use OnUpdate sparingly**: Only use OnUpdate for continuous monitoring, not for one-time setup
3. **Clean up resources**: Properly dispose of any resources when scenes change
4. **Modular design**: Break complex mods into smaller, focused functions

### UI Design Tips
1. **Logical grouping**: Use tabs to organize related controls
2. **Clear labeling**: Use descriptive labels and button text
3. **Feedback**: Provide user feedback through logging and UI updates
4. **Consistent naming**: Use consistent naming conventions for your mods

## Troubleshooting

### Common Issues
- **Panel not appearing**: Check that `set.RegisterModPanel()` returned `SleddingAPIStatus.Ok`
- **Button not working**: Ensure the callback function is properly defined
- **Player not found**: Verify the game has a GameObject tagged as "Player"
- **Config not saving**: Make sure to call `game.SetConfigValue()` with valid parameters

### Debugging Tips
- Use `log()` extensively to track script execution
- Check the BepInEx console for error messages
- Use `game.DrawDebugLine()` to visualize positions in the game world
- Test individual API calls in isolation

## License
This project is licensed under the MIT License - see the LICENSE file for details.
