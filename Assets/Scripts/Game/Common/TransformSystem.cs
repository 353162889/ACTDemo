using System.Numerics;
using Unity.Entities;

namespace Game
{
    public class TransformSystem : ComponentSystem
    {
        public TransformComponent AddTransformComponent(Entity entity)
        {
            var component = World.AddComponentOnce<TransformComponent>(entity);
            var prefabComponent = World.GetComponent<PrefabComponent>(entity);
            component.SetTransform(prefabComponent.transform);
            return component;
        }

        protected override void OnUpdate()
        {
        }
    }
}