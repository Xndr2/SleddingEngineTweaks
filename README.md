# Sledding Engine Tweaks (SET)

A comprehensive MelonLoader modding platform for the Sledding Game Demo. This mod provides a foundation for advanced game modifications and enhancements.  
**Warning: This mod is in active development! Expect bugs and crashes...**

## Current Features (v0.2 - Release)

- **First-Person Camera**: Toggle between third-person and first-person view
- **Player Model Hiding**: Automatically hides the player's character model when in first-person mode
- **Simple Camera System**: Uses Unity's standard camera system for first-person view
- **Easy Toggle**: Press F10 to switch between camera modes

## Planned Features

### **Core Framework**
- **Lua Scripting Support**: Full Lua scripting API for game modification
- **Anti-Cheat Bypass**: Safe methods to bypass game restrictions
- **Game API**: Comprehensive API for accessing game functions and data
- **Hot Reload**: Live script reloading without game restart

### **Game Enhancements**
- **Advanced Camera Controls**: Free camera, cinematic camera, camera paths
- **Player Modifications**: Custom animations, model swaps, physics tweaks
- **World Modifications**: Object spawning, terrain editing, lighting controls
- **UI Overlays**: Custom HUD elements, debug panels, script interfaces

### **Developer Tools**
- **Script Editor**: Built-in Lua script editor with syntax highlighting
- **Debug Console**: Real-time debugging and command execution
- **Asset Browser**: Browse and modify game assets
- **Performance Monitor**: FPS counter, memory usage, performance metrics

### **Modding Support**
- **Plugin System**: Support for third-party mods and plugins
- **Configuration System**: Easy mod configuration and settings
- **Update System**: Automatic mod updates and compatibility checking
- **Documentation**: Comprehensive API documentation and examples

## Requirements

- **Game**: Sledding Game Demo (Steam)
- **Mod Loader**: MelonLoader (must be installed separately)
- **Platform**: Windows/Linux

## MelonLoader Installation

**Important**: You must install MelonLoader before installing this mod!

### Step 1: Download MelonLoader

1. **Go to the official MelonLoader website**: https://melonwiki.xyz/
2. **Download the latest version** for your platform (Windows/Linux)

### Step 2: Install MelonLoader
1. **Run the installer**  
   Windows: `MelonLoader.Installer.exe`  
   Linux:   `MelonLoader.Installer.Linux` but make sure you have execute permissions
2. **Select your game**: Browse to your Sledding Game Demo installation folder
   - Usually: `C:\Program Files (x86)\Steam\steamapps\common\Sledding Game Demo`
3. **Click "Install"** and wait for completion
4. **Verify installation**: Check that `MelonLoader` folder was created in your game directory

### Step 3: Verify MelonLoader Installation

1. **Launch Sledding Game Demo**
2. **Look for MelonLoader console**: A console window should appear when the game starts
3. **Check for MelonLoader text**: You should see MelonLoader initialization messages
4. **If successful**: You'll see a message like "MelonLoader loaded successfully"

### Troubleshooting MelonLoader

- **Game won't start**: Make sure you're using the correct game executable
- **No console appears**: Check that MelonLoader was installed in the correct directory
- **Installation fails**: Try running the installer as administrator (Windows) or with sudo (Linux/macOS)
- **Still having issues**: Visit the MelonLoader Discord or GitHub for support

## Installation

### Automatic Installation (Recommended)

1. **Download the mod**:
   - [Download](https://github.com/Xndr2/SleddingEngineTweaks/releases/latest) `SleddingEngineTweaks.dll` from the latest release
   - Or build it yourself using the build script (see Building section)

2. **Locate your game directory**:
   ```
   Steam: Steam/steamapps/common/Sledding Game Demo/
   ```

3. **Install MelonLoader** (if not already installed):
   - The game should come with MelonLoader pre-installed
   - If not, download MelonLoader and extract it to your game directory

4. **Install the mod**:
   - Copy `SleddingEngineTweaks.dll` to the `Mods` folder in your game directory:
   ```
   Sledding Game Demo/Mods/SleddingEngineTweaks.dll
   ```

5. **Launch the game**:
   - Start the game normally through Steam
   - You should see MelonLoader load the mod in the console

### Manual Installation

1. **Ensure MelonLoader is installed** in your game directory
2. **Create the Mods folder** if it doesn't exist:
   ```
   Sledding Game Demo/Mods/
   ```
3. **Copy the mod file** to the Mods folder
4. **Launch the game**

## Usage

### Current Usage (v0.1)

Once installed and the game is running:

- **Press F10** to toggle between third-person and first-person camera
- **In first-person mode**: The player's character model will be hidden for a better experience
- **Switch back**: Press F10 again to return to third-person view

### Future Usage (Planned)

- **Lua Scripts**: Create and load custom Lua scripts for advanced modifications
- **Debug Console**: Access developer tools and real-time game manipulation
- **Configuration**: Customize mod behavior through configuration files
- **Plugin Management**: Install and manage additional mods and plugins

## Known Issues

### Current Limitations (v0.1)

- **❌ Cannot look around while in 1st person**: Mouse look functionality is not yet implemented
- **❌ Server compatibility**: Not tested when joining existing servers, may cause camera retrieval issues
- **❌ Hat visibility**: Player hats and accessories remain visible in first-person mode
- **❌ Limited camera control**: Basic camera system without advanced controls

### Technical Issues

- **Camera switching**: Occasional camera priority conflicts when switching modes rapidly
- **Performance**: Minor FPS impact during camera transitions
- **Memory usage**: Small memory leak when repeatedly toggling first-person mode

### Planned Fixes

- **Mouse look implementation**: Full first-person camera controls with mouse input
- **Server compatibility**: Robust camera system for multiplayer environments
- **Complete model hiding**: Hide all player accessories including hats, glasses, etc.
- **Performance optimization**: Reduced memory usage and improved FPS stability

## Building from Source

### Prerequisites

- **.NET 6.0 SDK** or later
- **Visual Studio** or **Visual Studio Code** (recommended)
- **Game installed** with MelonLoader

### Quick Build

Run the build script from the SleddingEngineTweaks directory:
```bash
# Linux/macOS
cd SleddingEngineTweaks
./build.sh

# Windows
cd SleddingEngineTweaks
build.bat
```

### Manual Build

1. **Clone or download** this repository
2. **Open terminal** in the SleddingEngineTweaks directory
3. **Build the project**:
   ```bash
   dotnet build --configuration Release
   ```
4. **The built mod** will be in `bin/Release/SleddingEngineTweaks.dll`
5. **Copy to game directory** manually or let the build script handle it

### Build Script Features

The build script (`build.sh`/`build.bat`) automatically:
- Builds the project in Release mode
- Finds your game directory (checks Steam locations)
- Creates the Mods folder if it doesn't exist
- Copies the mod to your game's Mods folder
- Provides feedback on the build process
- Handles different operating systems

## Troubleshooting

### Mod Not Loading

1. **Check MelonLoader**: Ensure MelonLoader is properly installed
2. **Check file location**: Verify `SleddingEngineTweaks.dll` is in the correct Mods folder
3. **Check console**: Look for error messages in the MelonLoader console
4. **Game version**: Ensure you're using a compatible version of the game

### Build Script Issues

1. **Game directory not found**: The script checks Steam default installation paths
2. **Custom path**: Provide the game directory manually:
   ```bash
   ./build.sh "/path/to/Sledding Game Demo"
   ```

### First-Person Not Working

1. **Press F10**: Make sure you're pressing the correct key
2. **Wait for setup**: The mod needs a moment to initialize after joining a game
3. **Check console**: Look for initialization messages from the mod

### Performance Issues

1. **Disable other mods**: Test with only this mod enabled
2. **Update graphics drivers**: Ensure your graphics drivers are up to date
3. **Lower game settings**: Reduce graphics quality if needed

## File Structure

### Current Structure (v0.1)
```
SleddingEngineTweaks/
├── Main.cs                 # Main mod entry point
├── Mods/
│   └── FirstPerson.cs      # First-person camera implementation
├── Properties/
│   └── AssemblyInfo.cs     # Assembly metadata
├── SleddingEngineTweaks.csproj  # Project configuration
├── build.sh                # Linux/macOS build script
├── build.bat               # Windows build script
├── README.md               # This file
├── INSTALL.md              # Quick installation guide
└── .gitignore              # Git ignore file
```

### Planned Structure (Future)
```
SleddingEngineTweaks/
├── Core/                   # Core framework components
│   ├── LuaEngine/          # Lua scripting engine
│   ├── AntiCheat/          # Anti-cheat bypass systems
│   ├── GameAPI/            # Game API wrapper
│   └── PluginSystem/       # Plugin management
├── Features/               # Feature implementations
│   ├── Camera/             # Advanced camera controls
│   ├── Player/             # Player modifications
│   ├── World/              # World editing tools
│   └── UI/                 # User interface overlays
├── Scripts/                # Default Lua scripts
├── Plugins/                # Third-party plugins
├── Config/                 # Configuration files
└── Documentation/          # API documentation
```

## Technical Details

### Current Implementation (v0.1)
- **Framework**: .NET 6.0
- **Mod Loader**: MelonLoader
- **Game Engine**: Unity (Il2Cpp)
- **Camera System**: Unity Camera + Cinemachine
- **Input Handling**: Unity Input System

### Planned Implementation (Future)
- **Scripting Engine**: Lua 5.4 with custom API bindings
- **Memory Management**: Advanced memory patching and hooking
- **Anti-Cheat**: Safe bypass methods for EAC/BattlEye
- **Performance**: Optimized rendering and memory usage
- **Cross-Platform**: Full Windows/Linux/macOS support

## Contributing

1. **Fork the repository**
2. **Create a feature branch**
3. **Make your changes**
4. **Test thoroughly**
5. **Submit a pull request**

## License

This project is open source. Please check the license file for details.

## Support

If you encounter any issues:

1. **Check this README** for troubleshooting steps
2. **Search existing issues** on the project page
3. **Create a new issue** with detailed information about your problem

## Development Roadmap

### Phase 1: Foundation (v0.1) ✅
- [x] Basic first-person camera implementation
- [x] Player model hiding system
- [x] Build system and documentation
- [x] Core modding framework setup

### Phase 2: Core Framework (v0.2-0.3)
- [ ] Lua scripting engine integration
- [ ] Basic game API implementation
- [ ] Configuration system
- [ ] Plugin architecture foundation

### Phase 3: Advanced Features (v0.4-0.5)
- [ ] Anti-cheat bypass systems
- [ ] Advanced camera controls
- [ ] Debug console and tools
- [ ] Performance monitoring

### Phase 4: Full Platform (v0.6+)
- [ ] Complete Lua API
- [ ] World modification tools
- [ ] UI overlay system
- [ ] Plugin marketplace

## Changelog

### Version 0.1 (Current)
- ✅ Initial release
- ✅ First-person camera toggle (F10)
- ✅ Player model hiding in first-person mode
- ✅ Basic camera system implementation
- ✅ Build system and documentation
- ❌ Mouse look functionality (planned for v0.2)
- ❌ Complete accessory hiding (planned for v0.2)
- ❌ Server compatibility testing (planned for v0.2)

### Planned for v0.2
- Mouse look implementation
- Complete player model hiding (including hats)
- Server compatibility improvements
- Basic Lua scripting support
- Configuration file system

---

**Disclaimer**: This mod is not affiliated with the official Sledding Game developers. This is an independent modding project inspired by CyberEngineTweaks. Use at your own risk and be aware of potential anti-cheat implications in multiplayer environments.
