# Sledding Engine Tweaks
Welcome to Sledding Engine Tweaks (SET), a powerful modding framework for Unity games, built on BepInEx. SET provides an advanced Lua scripting layer that allows for the rapid development of custom mods without needing to recompile or restart the game.
Create new UI, manipulate game objects, listen for keyboard input, change game settings, and much more, all from simple `.lua` script files.

## Features
- **Powerful Lua Scripting:** Leverage the lightweight and fast MoonSharp Lua interpreter to run your mods.
- **Live Script Loading:** Drop `.lua` files into the scripts folder, and they are loaded automatically. No restart required!
- **Dynamic In-Game UI:** Create your own mod panels with tabs, buttons, labels, and toggles directly from Lua. All UI elements can have their own scriptable callbacks.
- **Rich Game API:** A secure and extensive `game` API is exposed to Lua, giving you control over the player, game objects, input, configuration, and more.
- **Persistent Configuration:** Scripts can save and load their own settings, which persist between game sessions.
- **Built-in Event System:** Hook into core game events like `OnUpdate` and `OnSceneLoaded` to trigger your script's logic.

## For Players: Installation
1. Make sure you have [BepInEx](https://github.com/BepInEx/BepInEx) installed for your game.
2. Download the latest release of `SleddingEngineTweaks.dll` from the releases page.
3. Place the `SleddingEngineTweaks.dll` file into your `BepInEx/plugins/` folder.
4. Run the game once. This will generate a new folder: `BepInEx/plugins/SleddingEngineTweaks/`.
5. Inside the `SleddingEngineTweaks` folder, you will find a `scripts` subfolder. This is where you will place any `.lua` mods you download.

By default, press the **Delete** key to toggle the main UI panel in-game.

## For Developers: Creating Your First Script
Creating mods with SET is designed to be straightforward. All you need is a text editor.
### Getting Started: File Structure
1. Navigate to your game's `BepInEx/plugins/SleddingEngineTweaks/` folder.
2. Create a new folder inside called `scripts`.
3. Create a new file inside `scripts` with a `.lua` extension (e.g., `MyFirstMod.lua`).
4. Open this file in your editor and start scripting!

### Core Scripting Events
Your script can implement special global functions that SET will call automatically at certain times.
- `OnUpdate(deltaTime)`: Called every single frame. `deltaTime` is the time in seconds since the last frame. Ideal for continuous checks or actions.
- `OnSceneLoaded(sceneName)`: Called whenever a new scene is loaded in the game. `sceneName` is the name of the new scene.

**Example:**
```lua
-- ExampleMod.lua

-- This function will run every frame
function OnUpdate(deltaTime)
    -- The built-in log function prints to the BepInEx console.
    log("Game is updating! Delta time: " .. tostring(deltaTime))
end

-- This function will run when the main menu loads
function OnSceneLoaded(sceneName)
    if sceneName == "MainMenu" then
        log("Welcome to the main menu!")
    end
end
```

### The `game` API: Your Toolbox
SET exposes a global table (object) named `game` to all Lua scripts. This is your primary tool for interacting with the game.

#### **Logging & Debugging**

| Function Signature | Description |
| --- | --- |
| `game.Log(message)` | Prints a message to the BepInEx console, prefixed with `[Lua]`. |
| `game.DrawDebugLine(start, end, time)` | Draws a yellow line in the game world from a start `Vector3` to an end `Vector3`. |
#### **UI Management**
Create your own dedicated panel in the main UI window.

| Function Signature | Description |
| --- | --- |
| `game.RegisterModPanel(modName)` | Creates a new, draggable window for your mod. Returns `true` on success. |
| `game.RegisterModTab(modName, tabName)` | Adds a new tab to your mod's panel. Returns `true` on success. |
| `game.RegisterLabelOption(modName, tabName, text)` | Adds a simple text label to a tab. Returns `true` on success. |
| `game.RegisterButtonOption(modName, tabName, buttonText, callback)` | Adds a clickable button. The `callback` is a Lua function that runs when the button is pressed. |
| `game.RegisterSelectorOption(modName, tabName, text, default, cb)` | Adds a checkbox/toggle. The `callback` function is called with the new boolean value (`true`/`false`) when changed. |
#### **Player Control**

| Function Signature | Description |
| --- | --- |
| `game.GetPlayerPosition()` | Returns the player's current `Vector3` position. |
| `game.SetPlayerPosition(pos)` | Teleports the player to the given `Vector3` position `pos`. |
#### **Input Handling**

| Function Signature | Description |
| --- | --- |
| `game.IsKeyDown(keyName)` | Returns `true` if the specified key is currently held down. (e.g., "W") |
| `game.WasKeyPressedThisFrame(keyName)` | Returns `true` only on the single frame the key was first pressed down. |
_Note: `keyName` is a string and matches the values in the `UnityEngine.InputSystem.Key` enum (e.g., "Space", "LeftShift", "F5")._
#### **Configuration**
Save and load settings for your mod. All settings are stored in BepInEx's standard config files.

| Function Signature | Description |
| --- | --- |
| `game.GetConfigValue(section, key, default)` | Retrieves a string value from your mod's config. If it doesn't exist, it's created with the default value. |
| `game.SetConfigValue(section, key, value)` | Sets a string value in your mod's config and saves it to the file. |

### Full Example: "Player Teleporter" Mod
This script creates a simple UI that saves a teleport location and allows the player to teleport to it with a button press.

```lua
-- BepInEx/plugins/SleddingEngineTweaks/scripts/Teleporter.lua

local modName = "Teleporter"
local teleporterTab = "Main"
local savedX = 0
local savedY = 0
local savedZ = 0

-- A function to run when the script is first loaded.
-- We use this to set up our UI and load saved values.
function OnScriptLoad()
    log(modName .. " script has loaded!")

    -- Load our saved coordinates from the config file
    savedX = tonumber(game.GetConfigValue(modName, "PosX", "0"))
    savedY = tonumber(game.GetConfigValue(modName, "PosY", "100"))
    savedZ = tonumber(game.GetConfigValue(modName, "PosZ", "0"))

    -- Create the UI
    game.RegisterModPanel(modName)
    game.RegisterModTab(modName, teleporterTab)
    game.RegisterLabelOption(modName, teleporterTab, "Press F5 to save your position.")
    game.RegisterLabelOption(modName, teleporterTab, "Press F6 to teleport to saved spot.")

    -- Add a button that shows the currently saved position
    game.RegisterButtonOption(modName, teleporterTab, "Show Saved Position", function()
        log("Saved Position: X=" .. savedX .. ", Y=" .. savedY .. ", Z=" .. savedZ)
    end)
end

-- This function is our button callback for saving the position.
function SaveCurrentPosition()
    local playerPos = game.GetPlayerPosition()
    savedX = playerPos.x
    savedY = playerPos.y
    savedZ = playerPos.z

    -- Save the new values to the config file
    game.SetConfigValue(modName, "PosX", tostring(savedX))
    game.SetConfigValue(modName, "PosY", tostring(savedY))
    game.SetConfigValue(modName, "PosZ", tostring(savedZ))

    log("Position saved!")
end

-- The main update loop
function OnUpdate(deltaTime)
    -- Check if the F5 key was pressed to save the location
    if game.WasKeyPressedThisFrame("F5") then
        SaveCurrentPosition()
    end

    -- Check if the F6 key was pressed to teleport
    if game.WasKeyPressedThisFrame("F6") then
        -- We have to create a new Vector3 object to pass to the game API.
        -- Unfortunately, we can't do this directly in Lua yet, so we have
        -- to use a small workaround by setting the position of a temporary object.
        -- (This is a limitation we hope to address in a future API update!)
        
        -- For now, let's just log it. A future API will make this easier.
        log("Teleporting to " .. savedX .. ", " .. savedY .. ", " .. savedZ)
        
        -- When a Vector3 constructor is exposed to Lua, the code would look like this:
        -- local targetPos = Vector3.new(savedX, savedY, savedZ)
        -- game.SetPlayerPosition(targetPos)
    end
end

-- Make sure to call our setup function!
OnScriptLoad()

```

## License

This project is licensed under the MIT License - see the LICENSE file for details. 
