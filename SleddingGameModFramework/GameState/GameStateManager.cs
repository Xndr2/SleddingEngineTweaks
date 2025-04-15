using System;
using UnityEngine;

namespace SleddingGameModFramework.GameState
{
    public class GameStateManager : IGameState
    {
        private GameObject _player;
        private Rigidbody _playerRigidbody;
        private float _gameTime;
        private bool _isGamePaused;
        private string _currentLevel;
        private int _playerScore;
        private float _playerHealth;
        private float _playerSpeed;

        public Vector3 PlayerPosition => _player?.transform.position ?? Vector3.zero;
        public Vector3 PlayerVelocity => _playerRigidbody?.velocity ?? Vector3.zero;
        public bool IsPlayerGrounded => Physics.Raycast(_player?.transform.position ?? Vector3.zero, Vector3.down, 0.1f);
        public float GameTime => _gameTime;
        public bool IsGamePaused => _isGamePaused;
        public string CurrentLevel => _currentLevel;
        public int PlayerScore => _playerScore;
        public float PlayerHealth => _playerHealth;
        public float PlayerSpeed => _playerSpeed;

        public void Update()
        {
            if (!_isGamePaused)
            {
                _gameTime += Time.deltaTime;
            }

            // Update player references if needed
            if (_player == null)
            {
                _player = GameObject.FindGameObjectWithTag("Player");
                if (_player != null)
                {
                    _playerRigidbody = _player.GetComponent<Rigidbody>();
                }
            }

            // Update player speed
            if (_playerRigidbody != null)
            {
                _playerSpeed = _playerRigidbody.velocity.magnitude;
            }
        }

        public void SetGamePaused(bool paused)
        {
            _isGamePaused = paused;
        }

        public void SetCurrentLevel(string levelName)
        {
            _currentLevel = levelName;
        }

        public void SetPlayerScore(int score)
        {
            _playerScore = score;
        }

        public void SetPlayerHealth(float health)
        {
            _playerHealth = health;
        }

        public void AddPlayerScore(int points)
        {
            _playerScore += points;
        }

        public void ModifyPlayerHealth(float amount)
        {
            _playerHealth = Mathf.Max(0, _playerHealth + amount);
        }
    }
} 