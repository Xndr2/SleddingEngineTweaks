using UnityEngine;

namespace SleddingGameModFramework.TestGame
{
    public class TestGameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        public string currentLevel = "Level1";
        public float gameTime = 0f;
        public bool isPaused = false;

        private void Update()
        {
            if (!isPaused)
            {
                gameTime += Time.deltaTime;
            }

            // Toggle pause with P key
            if (Input.GetKeyDown(KeyCode.P))
            {
                isPaused = !isPaused;
                Time.timeScale = isPaused ? 0f : 1f;
            }

            // Simulate level changes
            if (Input.GetKeyDown(KeyCode.L))
            {
                currentLevel = currentLevel == "Level1" ? "Level2" : "Level1";
            }
        }

        public string CurrentLevel => currentLevel;
        public float GameTime => gameTime;
        public bool IsPaused => isPaused;
    }
} 