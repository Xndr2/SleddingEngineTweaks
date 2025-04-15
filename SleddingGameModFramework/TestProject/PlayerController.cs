using UnityEngine;

namespace SleddingGameModFramework.TestProject
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float maxSpeed = 20f;
        public float acceleration = 5f;
        public float deceleration = 2f;
        public float turnSpeed = 100f;
        public float gravity = 9.81f;
        public float groundCheckDistance = 0.1f;

        private Rigidbody _rb;
        private bool _isGrounded;
        private float _currentSpeed;
        private float _health = 100f;
        private int _score;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
        }

        private void Update()
        {
            // Handle input
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Update grounded state
            _isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

            // Calculate movement
            if (_isGrounded)
            {
                // Accelerate/decelerate
                if (verticalInput > 0)
                {
                    _currentSpeed = Mathf.MoveTowards(_currentSpeed, maxSpeed, acceleration * Time.deltaTime);
                }
                else if (verticalInput < 0)
                {
                    _currentSpeed = Mathf.MoveTowards(_currentSpeed, -maxSpeed * 0.5f, acceleration * Time.deltaTime);
                }
                else
                {
                    _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, deceleration * Time.deltaTime);
                }

                // Apply rotation
                transform.Rotate(Vector3.up, horizontalInput * turnSpeed * Time.deltaTime);
            }

            // Apply movement
            Vector3 movement = transform.forward * _currentSpeed;
            _rb.velocity = new Vector3(movement.x, _rb.velocity.y, movement.z);

            // Apply gravity
            if (!_isGrounded)
            {
                _rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
            }

            // Simulate score increase
            _score += Mathf.RoundToInt(_currentSpeed * Time.deltaTime);

            // Simulate health changes
            if (Input.GetKeyDown(KeyCode.H))
            {
                _health -= 10f;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                _health = 100f;
            }
        }

        public bool IsGrounded => _isGrounded;
        public float CurrentSpeed => _currentSpeed;
        public float Health => _health;
        public int Score => _score;
    }
} 