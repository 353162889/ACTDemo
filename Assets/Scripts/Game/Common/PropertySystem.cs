using Unity.Entities;

namespace Game
{
    public class PropertySystem : ComponentSystem
    {
        public float GetMoveSpeed(Entity entity)
        {
            var property = World.GetComponent<PropertyComponent>(entity);
            if (property != null)
            {
                return property.moveSpeed;
            }

            return 0;
        }

        protected override void OnUpdate()
        {
        }
    }
}