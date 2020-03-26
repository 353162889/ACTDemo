using Unity.Entities;

namespace Game
{
    public struct DamageInfo
    {
        public Entity caster;
        public Entity target;
        public float damage;
        public EntityHitInfo hitInfo;
    }
}