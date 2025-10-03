#!/bin/bash

# Sledding Engine Tweaks Build Script
# This script builds the mod and copies it to the game directory

set -e  # Exit on any error

echo "🎿 Sledding Engine Tweaks - Build Script"
echo "========================================"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if .NET 6.0 is installed
check_dotnet() {
    print_status "Checking .NET installation..."
    if ! command -v dotnet &> /dev/null; then
        print_error ".NET 6.0 SDK is not installed!"
        print_error "Please install .NET 6.0 SDK from: https://dotnet.microsoft.com/download"
        exit 1
    fi
    
    # Check version
    DOTNET_VERSION=$(dotnet --version)
    print_success "Found .NET version: $DOTNET_VERSION"
}

# Find game directory
find_game_directory() {
    print_status "Looking for Sledding Game Demo directory..."
    
    # Common Steam locations
    STEAM_PATHS=(
        "$HOME/.local/share/Steam/steamapps/common/Sledding Game Demo"
        "$HOME/.steam/steam/steamapps/common/Sledding Game Demo"
        "/home/$USER/.local/share/Steam/steamapps/common/Sledding Game Demo"
        "/home/$USER/.steam/steam/steamapps/common/Sledding Game Demo"
    )
    
    # Check if user provided a custom path
    if [ ! -z "$1" ]; then
        GAME_DIR="$1"
        if [ -d "$GAME_DIR" ]; then
            print_success "Using custom game directory: $GAME_DIR"
            return 0
        else
            print_error "Custom game directory not found: $GAME_DIR"
            exit 1
        fi
    fi
    
    # Try to find the game directory
    for path in "${STEAM_PATHS[@]}"; do
        if [ -d "$path" ]; then
            GAME_DIR="$path"
            print_success "Found game directory: $GAME_DIR"
            return 0
        fi
    done
    
    print_error "Could not find Sledding Game Demo directory!"
    print_error "Please ensure the game is installed or provide the path as an argument:"
    print_error "Usage: $0 [game_directory_path]"
    exit 1
}

# Create Mods directory if it doesn't exist
setup_mods_directory() {
    MODS_DIR="$GAME_DIR/Mods"
    if [ ! -d "$MODS_DIR" ]; then
        print_status "Creating Mods directory..."
        mkdir -p "$MODS_DIR"
        print_success "Created Mods directory: $MODS_DIR"
    else
        print_success "Mods directory already exists: $MODS_DIR"
    fi
}

# Build the project
build_project() {
    print_status "Building project..."
    
    # We're already in the SleddingEngineTweaks directory
    # Clean previous build
    print_status "Cleaning previous build..."
    dotnet clean --configuration Release --verbosity quiet
    
    # Build the project
    print_status "Building with .NET..."
    if dotnet build --configuration Release --verbosity minimal; then
        print_success "Build completed successfully!"
    else
        print_error "Build failed!"
        exit 1
    fi
}

# Copy mod to game directory
copy_mod() {
    SOURCE_FILE="bin/Release/SleddingEngineTweaks.dll"
    TARGET_FILE="$GAME_DIR/Mods/SleddingEngineTweaks.dll"
    
    if [ ! -f "$SOURCE_FILE" ]; then
        print_error "Built mod file not found: $SOURCE_FILE"
        exit 1
    fi
    
    print_status "Copying mod to game directory..."
    cp "$SOURCE_FILE" "$TARGET_FILE"
    
    if [ -f "$TARGET_FILE" ]; then
        print_success "Mod copied successfully to: $TARGET_FILE"
    else
        print_error "Failed to copy mod file!"
        exit 1
    fi
}

# Verify installation
verify_installation() {
    print_status "Verifying installation..."
    
    # Check if mod file exists
    if [ -f "$GAME_DIR/Mods/SleddingEngineTweaks.dll" ]; then
        FILE_SIZE=$(stat -c%s "$GAME_DIR/Mods/SleddingEngineTweaks.dll" 2>/dev/null || echo "unknown")
        print_success "Mod file verified (Size: ${FILE_SIZE} bytes)"
    else
        print_error "Mod file verification failed!"
        exit 1
    fi
    
    # Check if MelonLoader exists
    if [ -f "$GAME_DIR/MelonLoader/MelonLoader.dll" ]; then
        print_success "MelonLoader detected"
    else
        print_warning "MelonLoader not found - mod may not load properly"
    fi
}

# Main execution
main() {
    echo ""
    check_dotnet
    echo ""
    find_game_directory "$1"
    echo ""
    setup_mods_directory
    echo ""
    build_project
    echo ""
    copy_mod
    echo ""
    verify_installation
    echo ""
    print_success "🎉 Build and installation completed successfully!"
    print_status "You can now launch the game and press F10 to toggle first-person mode!"
    echo ""
}

# Run main function with all arguments
main "$@"
