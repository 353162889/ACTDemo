using Unity.Entities;

namespace Game
{
    public class PrefabSystem : ComponentSystem
    {
        public PrefabComponent AddPrefabComponent(Entity entity)
        {
            var component = World.AddComponentOnce<PrefabComponent>(entity);
            var gameObjectComponent = World.GetComponent<GameObjectComponent>(entity);
            component.gameObject = gameObjectComponent.gameObject;
            component.transform = gameObjectComponent.transform;
            return component;
        }

        protected override void OnUpdate()
        {
        }
    }
}