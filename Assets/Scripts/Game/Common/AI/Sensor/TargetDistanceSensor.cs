using Unity.Entities;

namespace Game
{
    public class TargetDistanceSensor : EntitySensor, IUpdateSensor
    {
        protected TransformComponent transformComponent;
        protected TransformComponent targetTransformComponent;
        protected Entity target;

        public TargetDistanceSensor(World world, Entity entity, AIWorldState worldState) : base(world, entity, worldState)
        {
        }

        protected override void OnInit()
        {
            transformComponent = this.world.GetComponent<TransformComponent>(this.entity);
        }

        public void Update(float deltaTime)
        {
            worldState.targetDistance = 9999;
            if (worldState.targetFactor > 0)
            {
                if (worldState.target != target)
                {
                    target = worldState.target;
                    targetTransformComponent = this.world.GetComponent<TransformComponent>(worldState.target);
                }

                if (worldState.target != Entity.Null)
                {
                    worldState.targetDistance =
                        (transformComponent.position - targetTransformComponent.position).magnitude;
                }
            }
        }
    }
}