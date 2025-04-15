using UnityEngine;

namespace SleddingGameModFramework.TestGame
{
    public class TestPlayer : MonoBehaviour
    {
        [Header("Player Stats")]
        public float health = 100f;
        public int score = 0;
        public float speed = 5f;

        private Rigidbody _rb;
        private bool _isGrounded;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            // Simple movement
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontal, 0f, vertical) * speed * Time.deltaTime;
            transform.Translate(movement);

            // Ground check
            _isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);

            // Simulate score increase
            score += Mathf.RoundToInt(Time.deltaTime * 10);

            // Test health changes
            if (Input.GetKeyDown(KeyCode.H))
            {
                health -= 10f;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                health = 100f;
            }
        }

        public float Health => health;
        public int Score => score;
        public bool IsGrounded => _isGrounded;
    }
} 