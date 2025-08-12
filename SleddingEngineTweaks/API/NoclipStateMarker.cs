using System.Collections.Generic;
using UnityEngine;

namespace SleddingEngineTweaks.API
{
    internal class NoclipStateMarker : MonoBehaviour
    {
        public bool WasActive;
        public List<Collider> AffectedColliders = new List<Collider>();
        public List<bool> ColliderPrevEnabled = new List<bool>();

        public List<Rigidbody> AffectedRigidbodies = new List<Rigidbody>();
        public List<bool> RbPrevUseGravity = new List<bool>();
        public List<bool> RbPrevDetectCollisions = new List<bool>();
        public List<bool> RbPrevIsKinematic = new List<bool>();

        public List<CharacterController> CharacterControllers = new List<CharacterController>();
        public List<bool> CharacterControllersPrevEnabled = new List<bool>();
    }
}

