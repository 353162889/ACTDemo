using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class BuffFloatSystem : ComponentSystem
    {
        private StepMoveSystem stepMoveSystem;
        protected override void OnCreate()
        {
            stepMoveSystem = World.GetOrCreateSystem<StepMoveSystem>();
        }
        public void Float(Entity entity, Vector3 horizonalVelcolity, float height)
        {
            if (height <= 0) return;
            var floatComponent = World.GetComponent<BuffFloatComponent>(entity);
            var groundComponent = World.GetComponent<GroundComponent>(entity);
            var gravityComponent = World.GetComponent<GravityComponent>(entity);
            if (null == floatComponent || null == groundComponent || null == gravityComponent) return;

            floatComponent.isFloat = true;

            var y = Mathf.Sqrt(2 * height * Mathf.Abs(gravityComponent.gravity));
            floatComponent.floatVerticalVelocity = new Vector3(0, y, 0);
            horizonalVelcolity.y = 0;
            floatComponent.floatHorizonalVelocity = horizonalVelcolity;
        }

        public void ResetFloat(BuffFloatComponent floatComponent)
        {
            floatComponent.isFloat = false;
            floatComponent.floatVerticalVelocity = Vector3.zero;
            floatComponent.floatHorizonalVelocity = Vector3.zero;
        }

        public void ResetFloat(Entity entity)
        {
            var floatComponent = World.GetComponent<BuffFloatComponent>(entity);
            if (floatComponent != null)
            {
                ResetFloat(floatComponent);
            }
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, BuffFloatComponent floatComponent, GravityComponent gravityComponent) =>
            {
                if (floatComponent.isFloat)
                {
                    stepMoveSystem.AppendSingleFrameVelocity(entity, floatComponent.floatHorizonalVelocity);
                    stepMoveSystem.AppendVelocity(entity, floatComponent.floatVerticalVelocity);
                    floatComponent.floatVerticalVelocity = Vector3.zero;
                }
            });
        }

        public void UpdateState()
        {
            Entities.ForEach((Entity entity, TransformComponent transformComponent, BuffFloatComponent floatComponent,
                GroundComponent groundComponent, StepMoveComponent stepMoveComponent) =>
            {
                if (floatComponent.isFloat)
                {
                    //更新浮空状态
                    if (stepMoveComponent.velocity.y <= 0)
                    {
                        if (groundComponent.isGround)
                        {
                            ResetFloat(floatComponent);
                        }
                    }
                }
            });
        }
    }
}