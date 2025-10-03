@echo off
setlocal enabledelayedexpansion

REM Sledding Engine Tweaks Build Script for Windows
REM This script builds the mod and copies it to the game directory

echo 🎿 Sledding Engine Tweaks - Build Script
echo ========================================
echo.

REM Check if .NET 6.0 is installed
echo [INFO] Checking .NET installation...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] .NET 6.0 SDK is not installed!
    echo [ERROR] Please install .NET 6.0 SDK from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
echo [SUCCESS] Found .NET version: %DOTNET_VERSION%
echo.

REM Find game directory
echo [INFO] Looking for Sledding Game Demo directory...

set GAME_DIR=
if not "%1"=="" (
    set GAME_DIR=%1
    if exist "%GAME_DIR%" (
        echo [SUCCESS] Using custom game directory: %GAME_DIR%
        goto :setup_mods
    ) else (
        echo [ERROR] Custom game directory not found: %GAME_DIR%
        pause
        exit /b 1
    )
)

REM Common Steam locations for Windows
set STEAM_PATHS[0]=%PROGRAMFILES(X86)%\Steam\steamapps\common\Sledding Game Demo
set STEAM_PATHS[1]=%PROGRAMFILES%\Steam\steamapps\common\Sledding Game Demo
set STEAM_PATHS[2]=%USERPROFILE%\AppData\Local\Steam\steamapps\common\Sledding Game Demo

REM Try to find the game directory
for /L %%i in (0,1,2) do (
    call :check_path !STEAM_PATHS[%%i]!
    if defined GAME_DIR goto :setup_mods
)

echo [ERROR] Could not find Sledding Game Demo directory!
echo [ERROR] Please ensure the game is installed or provide the path as an argument:
echo [ERROR] Usage: build.bat [game_directory_path]
pause
exit /b 1

:check_path
if exist "%1" (
    set GAME_DIR=%1
    echo [SUCCESS] Found game directory: %GAME_DIR%
)
goto :eof

:setup_mods
echo.
echo [INFO] Setting up Mods directory...
set MODS_DIR=%GAME_DIR%\Mods
if not exist "%MODS_DIR%" (
    mkdir "%MODS_DIR%"
    echo [SUCCESS] Created Mods directory: %MODS_DIR%
) else (
    echo [SUCCESS] Mods directory already exists: %MODS_DIR%
)
echo.

REM Build the project
echo [INFO] Building project...

REM We're already in the SleddingEngineTweaks directory
echo [INFO] Cleaning previous build...
dotnet clean --configuration Release --verbosity quiet

echo [INFO] Building with .NET...
dotnet build --configuration Release --verbosity minimal
if errorlevel 1 (
    echo [ERROR] Build failed!
    pause
    exit /b 1
)
echo [SUCCESS] Build completed successfully!
echo.

REM Copy mod to game directory
echo [INFO] Copying mod to game directory...
set SOURCE_FILE=bin\Release\SleddingEngineTweaks.dll
set TARGET_FILE=%GAME_DIR%\Mods\SleddingEngineTweaks.dll

if not exist "%SOURCE_FILE%" (
    echo [ERROR] Built mod file not found: %SOURCE_FILE%
    pause
    exit /b 1
)

copy "%SOURCE_FILE%" "%TARGET_FILE%" >nul
if exist "%TARGET_FILE%" (
    echo [SUCCESS] Mod copied successfully to: %TARGET_FILE%
) else (
    echo [ERROR] Failed to copy mod file!
    pause
    exit /b 1
)
echo.

REM Verify installation
echo [INFO] Verifying installation...
if exist "%TARGET_FILE%" (
    echo [SUCCESS] Mod file verified
) else (
    echo [ERROR] Mod file verification failed!
    pause
    exit /b 1
)

if exist "%GAME_DIR%\MelonLoader\MelonLoader.dll" (
    echo [SUCCESS] MelonLoader detected
) else (
    echo [WARNING] MelonLoader not found - mod may not load properly
)
echo.

echo [SUCCESS] 🎉 Build and installation completed successfully!
echo [INFO] You can now launch the game and press F10 to toggle first-person mode!
echo.
pause
