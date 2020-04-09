using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class CameraLockComponent : DataComponent
    {
        public Entity lockEntity;
        public float lockSpaceTime = 1.5f;
        public float endLockTime = 0;
        public bool enable = true;
        public Vector2 viewportPos = new Vector2(0.5f,0.5f);
        public float maxDistance = 100;
    }
}