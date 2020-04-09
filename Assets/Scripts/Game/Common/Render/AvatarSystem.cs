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
            var result = avatarComponent.mountPoint.GetMountpoint(name);
            if (result == null)
            {
                result = World.GetComponent<GameObjectComponent>(entity).transform;
            }

            return result;
        }

        protected override void OnUpdate()
        {
            
        }
    }
}