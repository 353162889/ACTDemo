using UnityEngine;

namespace Game
{
    public class EntityCommonInfoComponent : DataComponent
    {
        public int cfgId;
        public EntityType entityType;
        public Vector3 bornPosition;
        public Vector3 bornForward;
    }
}