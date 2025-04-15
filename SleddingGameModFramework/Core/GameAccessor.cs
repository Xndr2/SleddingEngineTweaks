using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace SleddingGameModFramework.Core
{
    public class GameAccessor
    {
        private static Harmony _harmony;
        private static Type _playerType;
        private static Type _gameManagerType;
        private static object _playerInstance;
        private static object _gameManagerInstance;

        public static void Initialize()
        {
            _harmony = new Harmony("com.sleddinggame.framework");
            FindGameTypes();
            PatchGameMethods();
        }

        private static void FindGameTypes()
        {
            // Find player type (assuming it's a MonoBehaviour)
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        // Look for common player component names
                        if (type.Name.Contains("Player") || 
                            type.Name.Contains("Character") || 
                            type.Name.Contains("Controller"))
                        {
                            _playerType = type;
                            break;
                        }
                    }
                }
            }

            // Find game manager type
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        // Look for common game manager names
                        if (type.Name.Contains("GameManager") || 
                            type.Name.Contains("LevelManager") || 
                            type.Name.Contains("SceneManager"))
                        {
                            _gameManagerType = type;
                            break;
                        }
                    }
                }
            }
        }

        private static void PatchGameMethods()
        {
            if (_playerType != null)
            {
                // Patch player update method
                var playerUpdate = _playerType.GetMethod("Update", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (playerUpdate != null)
                {
                    _harmony.Patch(playerUpdate,
                        postfix: new HarmonyMethod(typeof(GameAccessor).GetMethod("OnPlayerUpdate", BindingFlags.NonPublic | BindingFlags.Static)));
                }

                // Patch player awake/start
                var playerStart = _playerType.GetMethod("Start", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ??
                                _playerType.GetMethod("Awake", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (playerStart != null)
                {
                    _harmony.Patch(playerStart,
                        postfix: new HarmonyMethod(typeof(GameAccessor).GetMethod("OnPlayerStart", BindingFlags.NonPublic | BindingFlags.Static)));
                }
            }

            if (_gameManagerType != null)
            {
                // Patch game manager update
                var gameManagerUpdate = _gameManagerType.GetMethod("Update", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (gameManagerUpdate != null)
                {
                    _harmony.Patch(gameManagerUpdate,
                        postfix: new HarmonyMethod(typeof(GameAccessor).GetMethod("OnGameManagerUpdate", BindingFlags.NonPublic | BindingFlags.Static)));
                }
            }
        }

        private static void OnPlayerStart(MonoBehaviour __instance)
        {
            _playerInstance = __instance;
            Plugin.EventManager.Publish("PlayerInitialized", null);
        }

        private static void OnPlayerUpdate(MonoBehaviour __instance)
        {
            UpdatePlayerState(__instance);
        }

        private static void OnGameManagerUpdate(MonoBehaviour __instance)
        {
            _gameManagerInstance = __instance;
            UpdateGameState(__instance);
        }

        private static void UpdatePlayerState(MonoBehaviour player)
        {
            try
            {
                // Get player position
                var position = (Vector3)player.GetType().GetProperty("transform")?.GetValue(player)
                    ?.GetType().GetProperty("position")?.GetValue(null);

                // Get player velocity (if Rigidbody exists)
                var velocity = Vector3.zero;
                var rigidbody = player.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    velocity = rigidbody.velocity;
                }

                // Get health (assuming it's a property or field)
                var health = 100f;
                var healthProperty = player.GetType().GetProperty("Health") ?? 
                                   player.GetType().GetProperty("health");
                if (healthProperty != null)
                {
                    health = (float)healthProperty.GetValue(player);
                }

                // Get score
                var score = 0;
                var scoreProperty = player.GetType().GetProperty("Score") ?? 
                                  player.GetType().GetProperty("score");
                if (scoreProperty != null)
                {
                    score = (int)scoreProperty.GetValue(player);
                }

                // Update game state
                Plugin.GameState.SetPlayerPosition(position);
                Plugin.GameState.SetPlayerVelocity(velocity);
                Plugin.GameState.SetPlayerHealth(health);
                Plugin.GameState.SetPlayerScore(score);
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Error updating player state: {e.Message}");
            }
        }

        private static void UpdateGameState(MonoBehaviour gameManager)
        {
            try
            {
                // Get current level
                var levelProperty = gameManager.GetType().GetProperty("CurrentLevel") ?? 
                                  gameManager.GetType().GetProperty("currentLevel");
                if (levelProperty != null)
                {
                    var level = (string)levelProperty.GetValue(gameManager);
                    Plugin.GameState.SetCurrentLevel(level);
                }

                // Get game time
                var timeProperty = gameManager.GetType().GetProperty("GameTime") ?? 
                                 gameManager.GetType().GetProperty("gameTime");
                if (timeProperty != null)
                {
                    var time = (float)timeProperty.GetValue(gameManager);
                    Plugin.GameState.SetGameTime(time);
                }

                // Check if game is paused
                var pausedProperty = gameManager.GetType().GetProperty("IsPaused") ?? 
                                   gameManager.GetType().GetProperty("isPaused");
                if (pausedProperty != null)
                {
                    var isPaused = (bool)pausedProperty.GetValue(gameManager);
                    Plugin.GameState.SetGamePaused(isPaused);
                }
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Error updating game state: {e.Message}");
            }
        }
    }
} 