using UnityEngine;

namespace Game
{
    public class TransformComponent : DataComponent
    {
        private Transform transform;
        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Vector3 eulerAngles
        {
            get => transform.eulerAngles;
            set => transform.eulerAngles = value;
        }

        public Quaternion rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        public Vector3 forward
        {
            get => transform.forward;
            set => transform.forward = value;
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
        }
    }
}