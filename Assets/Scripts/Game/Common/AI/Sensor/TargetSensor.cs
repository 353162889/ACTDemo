using Unity.Entities;

namespace Game
{
    public class TargeSensor : EntitySensor, IUpdateSensor
    {
        private TargetTriggerComponent targetTriggerComponent;
        private TargetTriggerSystem targetTriggerSystem;

        public TargeSensor(World world, Entity entity, AIWorldState worldState) : base(world, entity, worldState)
        {
        }

        protected override void OnInit()
        {
            targetTriggerComponent = this.world.GetComponent<TargetTriggerComponent>(this.entity);
            targetTriggerSystem = this.world.GetExistingSystem<TargetTriggerSystem>();
        }

        public void Update(float deltaTime)
        {
            if (targetTriggerComponent != null)
            {
                var target = worldState.target;
                if (target == Entity.Null || !targetTriggerSystem.ValidEntity(targetTriggerComponent, target))
                {
                    worldState.ResetTarget();
                    if (targetTriggerComponent.lstEntity.Count > 0)
                    {
                        worldState.SetFilterTarget(targetTriggerComponent.lstEntity[0]);
                    }
                    worldState.targetFactor = worldState.target == Entity.Null ? 0 : 1;
                }
            }
        }
    }
}