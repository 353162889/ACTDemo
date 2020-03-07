using UnityEngine;

namespace Game
{
    public class FaceComponent : DataComponent
    {
        public Quaternion rotation;
        public bool immediately;
        public float desiredDegreeSpeed;
        public bool isRotating = false;
    }
}