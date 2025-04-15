using UnityEngine;
using SleddingGameModFramework.GameState;

namespace SleddingGameModFramework.TestProject
{
    public class TestGameStateManager : MonoBehaviour
    {
        private PlayerController _player;
        private TestSceneManager _sceneManager;

        private void Start()
        {
            _player = FindObjectOfType<PlayerController>();
            _sceneManager = FindObjectOfType<TestSceneManager>();

            if (_player == null)
            {
                Debug.LogError("PlayerController not found in scene!");
            }
            if (_sceneManager == null)
            {
                Debug.LogError("TestSceneManager not found in scene!");
            }
        }

        private void Update()
        {
            if (_player == null) return;

            // Update player state
            Plugin.GameState.SetPlayerHealth(_player.Health);
            Plugin.GameState.SetPlayerScore(_player.Score);

            // Update game state
            Plugin.GameState.Update();
        }
    }
} 