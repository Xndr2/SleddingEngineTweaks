using UnityEngine;

namespace SleddingGameModFramework.TestProject
{
    public class TestSceneSetup : MonoBehaviour
    {
        [Header("Scene Setup")]
        public Material groundMaterial;
        public Material playerMaterial;
        public float groundSize = 100f;

        private void Start()
        {
            SetupGround();
            SetupPlayer();
            SetupManagers();
        }

        private void SetupGround()
        {
            // Create ground plane
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(groundSize / 10f, 1f, groundSize / 10f);
            ground.transform.position = Vector3.zero;

            if (groundMaterial != null)
            {
                ground.GetComponent<Renderer>().material = groundMaterial;
            }
        }

        private void SetupPlayer()
        {
            // Create player object
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = new Vector3(0f, 1f, 0f);

            // Add required components
            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.drag = 0.5f;
            rb.angularDrag = 0.5f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            player.AddComponent<PlayerController>();

            if (playerMaterial != null)
            {
                player.GetComponent<Renderer>().material = playerMaterial;
            }
        }

        private void SetupManagers()
        {
            // Create managers
            GameObject managers = new GameObject("Managers");
            managers.AddComponent<TestSceneManager>();
            managers.AddComponent<TestGameStateManager>();
        }
    }
} 