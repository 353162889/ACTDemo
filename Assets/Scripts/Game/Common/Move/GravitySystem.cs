using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class GravitySystem : ComponentSystem
    {
        private StepMoveSystem stepMoveSystem;
        protected override void OnCreate()
        {
            stepMoveSystem = World.GetOrCreateSystem<StepMoveSystem>();
        }
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, GravityComponent gravityComponent, GroundComponent groundComponent) =>
            {
                if (gravityComponent.useGravity && !groundComponent.isGround)
                {
                    stepMoveSystem.AppendVelocity(entity, Vector3.up * gravityComponent.gravity * Time.deltaTime);
                }
            });
        }
    }
}