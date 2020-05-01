using System.Security.Policy;
using UnityEngine;

namespace Game
{
    public class PhysicComponent : DataComponent
    {
        public Rigidbody rigidbody;
        public CollisionListener collisionListener;
        public Transform attackColliderParent;
    }
}