using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PointsMoveComponent : DataComponent
    {
        public Vector3 curPosition;
        public Vector3 nextPosition;
        public Vector3 targetPosition;
        public Queue<Vector3> queuePath = new Queue<Vector3>();
        public Vector3 curForward;
        public bool isMoving;
        public float desiredSpeed;
        public bool changeFace;
    }
}