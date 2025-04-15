using UnityEngine;
using SleddingGameModFramework.GameState;

namespace SleddingGameModFramework.TestProject
{
    public class TestSceneManager : MonoBehaviour
    {
        [Header("Level Settings")]
        public string[] levelNames = { "Level1", "Level2", "Level3" };
        public float levelTransitionTime = 5f;

        private int _currentLevelIndex;
        private float _levelTimer;
        private bool _isPaused;

        private void Start()
        {
            _currentLevelIndex = 0;
            _levelTimer = 0f;
            _isPaused = false;

            // Initialize game state
            Plugin.GameState.SetCurrentLevel(levelNames[_currentLevelIndex]);
        }

        private void Update()
        {
            if (_isPaused) return;

            // Update level timer
            _levelTimer += Time.deltaTime;

            // Simulate level transitions
            if (_levelTimer >= levelTransitionTime)
            {
                _levelTimer = 0f;
                _currentLevelIndex = (_currentLevelIndex + 1) % levelNames.Length;
                Plugin.GameState.SetCurrentLevel(levelNames[_currentLevelIndex]);
            }

            // Toggle pause with P key
            if (Input.GetKeyDown(KeyCode.P))
            {
                _isPaused = !_isPaused;
                Plugin.GameState.SetGamePaused(_isPaused);
            }
        }
    }
} 