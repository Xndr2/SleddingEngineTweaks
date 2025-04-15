using BepInEx;
using UnityEngine;

namespace SleddingGameModFramework.TestMods
{
    [BepInPlugin("com.example.statemonitor", "State Monitor Mod", "1.0.0")]
    public class StateMonitorMod : BaseUnityPlugin
    {
        private GUIStyle _style;
        private string _statusText = "";

        private void Awake()
        {
            // Subscribe to game state events
            Plugin.EventManager.Subscribe("PlayerInitialized", OnPlayerInitialized);
            Plugin.EventManager.Subscribe("GameStateUpdated", OnGameStateUpdated);

            // Set up GUI style
            _style = new GUIStyle
            {
                fontSize = 20,
                normal = { textColor = Color.white }
            };
        }

        private void OnPlayerInitialized(object sender, object data)
        {
            Logger.LogInfo("Player was detected and initialized!");
        }

        private void OnGameStateUpdated(object sender, object data)
        {
            var gameState = Plugin.GameState;
            _statusText = $"Health: {gameState.PlayerHealth:F0}\n" +
                         $"Score: {gameState.PlayerScore}\n" +
                         $"Level: {gameState.CurrentLevel}\n" +
                         $"Time: {gameState.GameTime:F1}\n" +
                         $"Position: {gameState.PlayerPosition}\n" +
                         $"Velocity: {gameState.PlayerVelocity}\n" +
                         $"Grounded: {gameState.IsPlayerGrounded}\n" +
                         $"Paused: {gameState.IsGamePaused}";
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 300, 200), _statusText, _style);
        }

        private void OnDestroy()
        {
            Plugin.EventManager.Unsubscribe("PlayerInitialized", OnPlayerInitialized);
            Plugin.EventManager.Unsubscribe("GameStateUpdated", OnGameStateUpdated);
        }
    }
} 