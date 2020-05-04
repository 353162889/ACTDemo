using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class GravitySystem : ComponentSystem
    {
        private StepMoveSystem stepMoveSystem;
        private ForbidSystem forbidSystem;
        protected override void OnCreate()
        {
            stepMoveSystem = World.GetOrCreateSystem<StepMoveSystem>();
            forbidSystem = World.GetOrCreateSystem<ForbidSystem>();
        }
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, GravityComponent gravityComponent, GroundComponent groundComponent, ForbidComponent forbidComponent) =>
            {
                if (!groundComponent.isGround && !forbidSystem.IsForbid(forbidComponent, ForbidType.Gravity))
                {
                    stepMoveSystem.AppendVelocity(entity, Vector3.up * gravityComponent.gravity * Time.deltaTime);
                }
            });
        }
    }
}