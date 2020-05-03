using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class InAirSystem : ComponentSystem
    {
        private StepMoveSystem stepMoveSystem;
        protected override void OnCreate()
        {
            stepMoveSystem = World.GetOrCreateSystem<StepMoveSystem>();
        }
        public void MoveToAir(Entity entity, Vector3 horizonalVelcolity, float height)
        {
            if (height <= 0) return;
            var inAirComponent = World.GetComponent<InAirComponent>(entity);
            var groundComponent = World.GetComponent<GroundComponent>(entity);
            var gravityComponent = World.GetComponent<GravityComponent>(entity);
            if (null == inAirComponent || null == groundComponent || null == gravityComponent) return;

            inAirComponent.isInAir = true;

            var y = Mathf.Sqrt(2 * height * Mathf.Abs(gravityComponent.gravity));
            inAirComponent.inAirVerticalVelocity = new Vector3(0, y, 0);
            horizonalVelcolity.y = 0;
            inAirComponent.inAirHorizonalVelocity = horizonalVelcolity;
        }

        private void ResetInAir(InAirComponent inAirComponent)
        {
            inAirComponent.isInAir = false;
            inAirComponent.inAirVerticalVelocity = Vector3.zero;
            inAirComponent.inAirHorizonalVelocity = Vector3.zero;
        }

        public void ResetAirSpeed(Entity entity)
        {
            var inAirComponent = World.GetComponent<InAirComponent>(entity);
            if (inAirComponent != null && inAirComponent.isInAir)
            {
                stepMoveSystem.StopMove(entity);
                inAirComponent.inAirVerticalVelocity = Vector3.zero;
                inAirComponent.inAirHorizonalVelocity = Vector3.zero;
            }
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, InAirComponent inAirComponent, GravityComponent gravityComponent) =>
            {
                if (inAirComponent.isInAir)
                {
                    stepMoveSystem.AppendSingleFrameVelocity(entity, inAirComponent.inAirHorizonalVelocity);
                    stepMoveSystem.AppendVelocity(entity, inAirComponent.inAirVerticalVelocity);
                    inAirComponent.inAirVerticalVelocity = Vector3.zero;
                }
            });
        }

        public void UpdateState()
        {
            Entities.ForEach((Entity entity, TransformComponent transformComponent, InAirComponent inAirComponent,
                GroundComponent groundComponent, StepMoveComponent stepMoveComponent) =>
            {
                if (inAirComponent.isInAir)
                {
                    //更新空中状态
                    if (stepMoveComponent.velocity.y <= 0)
                    {
                        if (groundComponent.isGround)
                        {
                            ResetInAir(inAirComponent);
                        }
                    }
                }
                else
                {
                    if (!groundComponent.isGround)
                    {
                        inAirComponent.isInAir = true;
                    }
                }
            });
        }
    }
}