using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class AvatarSystem : ComponentSystem
    {

        public Transform GetMountPoint(Entity entity, string name)
        {
            var avatarComponent = World.GetComponent<AvatarComponent>(entity);
            if (avatarComponent == null || avatarComponent.mountPoint == null) return null;
            return avatarComponent.mountPoint.GetMountpoint(name);
        }

        protected override void OnUpdate()
        {
            
        }
    }
}