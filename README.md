# Sledding Game Mod Framework

A modding framework for Sledding Game that allows modders to create and load mods easily.

## Installation

### Prerequisites
1. Install BepInEx:
   - Download the latest BepInEx release from [BepInEx GitHub](https://github.com/BepInEx/BepInEx/releases)
   - Extract the contents of the zip file into your Sledding Game installation directory
   - Run the game once to let BepInEx initialize
   - Close the game after BepInEx has initialized

### Framework Installation
1. Download the latest release of the framework
2. Place the `SleddingGameModFramework.dll` in the game's `BepInEx/plugins` folder
3. Create a `Mods` folder in the game's root directory
4. Place any mod DLLs in the `Mods` folder

## Creating a Mod

To create a mod using this framework:

1. Create a new .NET Standard 2.1 class library project
2. Add references to:
   - `SleddingGameModFramework.dll`
   - `UnityEngine.dll`
3. Implement the `IMod` interface
4. Build your project and place the resulting DLL in the game's `Mods` folder

### Example Mod

The `ExampleMod` project demonstrates how to create a simple mod that:
- Creates a UI element
- Updates the UI every frame
- Handles game pause/resume events
- Properly cleans up resources
- Uses the configuration system
- Handles game events

## Framework Features

- Automatic mod loading from the `Mods` folder
- Lifecycle management (load, unload, update, pause, resume)
- Error handling and logging
- Unity integration
- Configuration system
- Event system
- UI system
- Game state access
- Mod dependencies and version checking

## Configuration System

The framework includes a configuration system that allows mods to:
- Define their own configuration classes
- Save and load configurations automatically
- React to configuration changes
- Have their configurations persisted between game sessions

To use the configuration system:
1. Create a class that implements `IModConfig`
2. Register your configuration in your mod's `OnLoad` method
3. Access your configuration through the `ConfigManager`

## Event System

The framework includes an event system that allows mods to:
- Listen for game events
- React to game events
- Cancel certain events
- Communicate with other mods

To use the event system:
1. Implement `IEventHandler` in your mod
2. Register for events in your mod's `OnLoad` method
3. Handle events in the `OnEvent` method

### Available Events

- `PlayerSpawnEvent`: Triggered when a player spawns
- `PlayerDeathEvent`: Triggered when a player dies (cancellable)
- `GameStartEvent`: Triggered when a game starts
- `GameEndEvent`: Triggered when a game ends

## UI System

The framework includes a UI system that allows mods to:
- Create custom UI elements
- Position UI elements on screen
- Style UI elements
- Handle UI interactions

To use the UI system:
1. Create a class that inherits from `ModUI`
2. Override the necessary methods
3. Register your UI in your mod's `OnLoad` method

## Game State Access

The framework provides access to game state through:
- Player information
- Game settings
- Level data
- Physics state

To access game state:
1. Use the `GameState` class
2. Subscribe to state changes through events
3. Access state properties directly

## Mod Dependencies

The framework supports mod dependencies:
- Version checking
- Dependency resolution
- Load order management

To specify dependencies:
1. Add a `ModDependencies` attribute to your mod class
2. List required mods and their versions
3. The framework will handle loading order and version checking

## Mod Lifecycle

1. `OnLoad()` - Called when the mod is first loaded
2. `OnUpdate()` - Called every frame
3. `OnPause()` - Called when the game is paused
4. `OnResume()` - Called when the game is resumed
5. `OnUnload()` - Called when the mod is unloaded

## Requirements

- Sledding Game
- BepInEx 5.4.22 or later
- .NET Standard 2.1

## Troubleshooting

If you encounter issues:
1. Make sure BepInEx is properly installed
2. Check the `BepInEx/LogOutput.log` file for errors
3. Verify that all required dependencies are present
4. Ensure mods are placed in the correct folders

## License

This project is licensed under the MIT License - see the LICENSE file for details. 