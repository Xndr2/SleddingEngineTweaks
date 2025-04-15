using UnityEngine;

namespace SleddingGameModFramework.GameState
{
    public interface IGameState
    {
        /// <summary>
        /// The current player position
        /// </summary>
        Vector3 PlayerPosition { get; }

        /// <summary>
        /// The current player velocity
        /// </summary>
        Vector3 PlayerVelocity { get; }

        /// <summary>
        /// Whether the player is grounded
        /// </summary>
        bool IsPlayerGrounded { get; }

        /// <summary>
        /// The current game time
        /// </summary>
        float GameTime { get; }

        /// <summary>
        /// Whether the game is paused
        /// </summary>
        bool IsGamePaused { get; }

        /// <summary>
        /// The current level name
        /// </summary>
        string CurrentLevel { get; }

        /// <summary>
        /// The player's current score
        /// </summary>
        int PlayerScore { get; }

        /// <summary>
        /// The player's current health
        /// </summary>
        float PlayerHealth { get; }

        /// <summary>
        /// The player's current speed
        /// </summary>
        float PlayerSpeed { get; }
    }
} 