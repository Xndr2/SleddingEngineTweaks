# Sledding Engine Tweaks

A powerful modding framework for Unity games that enables rapid development of custom mods through Lua scripting.

## ⚠️ Development Status

**Currently in active development** - This framework is being developed for the upcoming **Sledding Game** on Steam, with current testing on **Lethal Company** and compatibility with **Unity 2022.3.9** games.

## 🚀 Quick Start

### Installation
1. Install [BepInEx](https://github.com/BepInEx/BepInEx) for your game
2. Download `SleddingEngineTweaks.dll` from [releases](../../releases)
3. Place the DLL in `BepInEx/plugins/`
4. Run the game once to generate the mod folder
5. Place `.lua` scripts in `BepInEx/plugins/SleddingEngineTweaks/scripts/`

**Toggle UI:** Press `Delete` key in-game

### Your First Script
```lua
-- MyFirstMod.lua
local modName = "MyFirstMod"

function OnScriptLoad()
    log("Hello from " .. modName .. "!")
    
    -- Create UI
    set.RegisterModPanel(modName)
    set.RegisterModTab(modName, "Main")
    set.RegisterButtonOption(modName, "Main", "test_btn", "Click Me!", function()
        local pos = game.GetPlayerPosition()
        log("Player at: " .. tostring(pos))
    end)
end

OnScriptLoad()
```

## ✨ Key Features

- **🟩 Lua Scripting**: Safe sandboxed environment with MoonSharp
- **🟩 Dynamic UI**: Create panels, tabs, buttons, and controls from Lua
- **🟩 Game Integration**: Player control, teleportation, noclip, scene management
- **🟩 Persistent Config**: Save settings between sessions
- **🟩 Prefab System**: Load and spawn AssetBundle prefabs
- **🟩 Dual API**: Separate `game` and `set` APIs for different purposes

## 📚 Documentation

For comprehensive documentation, visit our [Wiki](../../wiki):

- **[Getting Started](../../wiki/Home)** - Overview and quick start guide
- **[Installation Guide](../../wiki/Installation)** - Detailed setup instructions
- **[Lua Scripting Guide](../../wiki/Lua-Scripting-Guide)** - Learn to write mods
- **[API Reference](../../wiki/API-Reference)** - Complete API documentation
- **[Prefab System](../../wiki/Prefab-System)** - AssetBundle and prefab management
- **[Examples](../../wiki/Examples)** - Practical mod examples
- **[Troubleshooting](../../wiki/Troubleshooting)** - Common issues and solutions

## 🛠️ Development

### For Modders
- **Lua Scripts**: Write `.lua` files for rapid development
- **BepInEx Plugins**: Use SET as a dependency for advanced C# mods

### For Contributors
See the [Development Guide](../../wiki/Development) for building and contributing to the framework.

## 📄 License

MIT License - see [LICENSE](LICENSE) for details.
