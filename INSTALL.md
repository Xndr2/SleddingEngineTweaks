# Quick Installation Guide

## Prerequisites

**⚠️ Important**: You must install MelonLoader first!

### Installing MelonLoader

1. **Download**: Go to https://melonwiki.xyz/ and download MelonLoader for your platform
2. **Install**: Run the installer and select your Sledding Game Demo folder
3. **Verify**: Launch the game - you should see a MelonLoader console window

## Option 1: Use the Build Script (Recommended)

### Linux/macOS:
```bash
cd SleddingEngineTweaks
./build.sh
```

### Windows:
```cmd
cd SleddingEngineTweaks
build.bat
```

The build script will automatically:
- Build the mod
- Find your game directory (checks Steam locations)
- Create Mods folder if needed
- Install the mod
- Verify the installation

## Option 2: Manual Installation

1. **Build the mod**:
   ```bash
   dotnet build --configuration Release
   ```

2. **Find your game directory**:
   - Look for `Sledding Game Demo` in your Steam library
   - Right-click → Properties → Local Files → Browse

3. **Copy the mod**:
   - Copy `bin/Release/SleddingEngineTweaks.dll`
   - To `Sledding Game Demo/Mods/SleddingEngineTweaks.dll`

4. **Launch the game** and press **F10** to toggle first-person mode!

## Troubleshooting

- **Mod not loading?** 
  - First, verify MelonLoader is installed (console window should appear when launching game)
  - Check that `SleddingEngineTweaks.dll` is in the `Mods` folder
  - Look for error messages in the MelonLoader console
- **Can't find game directory?** The script checks Steam default installation paths
  - Or provide a custom path: `./build.sh "/path/to/Sledding Game Demo"`
- **Build errors?** Make sure you have .NET 6.0 SDK installed
- **MelonLoader issues?** Visit https://melonwiki.xyz/ for support

## Support

See the main README.md for detailed troubleshooting and feature information.
