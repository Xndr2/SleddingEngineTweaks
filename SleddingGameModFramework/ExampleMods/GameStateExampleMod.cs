using BepInEx;
using SleddingGameModFramework.Core;
using SleddingGameModFramework.GameState;
using UnityEngine;

namespace SleddingGameModFramework.ExampleMods
{
    [BepInPlugin("com.example.gamestate", "Game State Example Mod", "1.0.0")]
    public class GameStateExampleMod : BaseUnityPlugin
    {
        private void Awake()
        {
            // Subscribe to game state updates
            Plugin.EventManager.Subscribe("GameStateUpdated", OnGameStateUpdated);
        }

        private void OnGameStateUpdated(object sender, object data)
        {
            // Access current game state
            var gameState = Plugin.GameState;
            
            // Log player position and velocity
            Logger.LogInfo($"Player Position: {gameState.PlayerPosition}");
            Logger.LogInfo($"Player Velocity: {gameState.PlayerVelocity}");
            
            // Check if player is grounded
            if (gameState.IsPlayerGrounded)
            {
                Logger.LogInfo("Player is grounded");
            }
            
            // Display game time
            Logger.LogInfo($"Game Time: {gameState.GameTime:F2}");
            
            // Check if game is paused
            if (gameState.IsGamePaused)
            {
                Logger.LogInfo("Game is paused");
            }
            
            // Display current level and score
            Logger.LogInfo($"Current Level: {gameState.CurrentLevel}");
            Logger.LogInfo($"Player Score: {gameState.PlayerScore}");
            
            // Display player health and speed
            Logger.LogInfo($"Player Health: {gameState.PlayerHealth}");
            Logger.LogInfo($"Player Speed: {gameState.PlayerSpeed:F2}");
        }

        private void OnDestroy()
        {
            // Unsubscribe from game state updates
            Plugin.EventManager.Unsubscribe("GameStateUpdated", OnGameStateUpdated);
        }
    }
} 