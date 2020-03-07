using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class JumpSystem : ComponentSystem
    {
        private StepMoveSystem stepMoveSystem;
        protected override void OnCreate()
        {
            stepMoveSystem = World.GetOrCreateSystem<StepMoveSystem>();
        }
        public void Jump(Entity entity)
        {
            var jumpComponent = World.GetComponent<JumpComponent>(entity);
            var groundComponent = World.GetComponent<GroundComponent>(entity);
            if (null == jumpComponent || null == groundComponent) return;
            if (!groundComponent.isGround || jumpComponent.isJump) return;
            jumpComponent.isJump = true;
            jumpComponent.jumpVelocity = new Vector3(0, jumpComponent.desiredJumpSpeed, 0);
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, JumpComponent jumpComponent, GravityComponent gravityComponent) =>
            {
                if (jumpComponent.isJump && jumpComponent.desiredJumpSpeed > 0)
                {
                    if (gravityComponent.useGravity)
                    {
                        stepMoveSystem.AppendMove(entity, jumpComponent.jumpVelocity);
                    }
                }
            });
        }

        public void UpdateState()
        {
            Entities.ForEach((Entity entity, TransformComponent transformComponent, JumpComponent jumpComponent,
                GroundComponent groundComponent, StepMoveComponent stepMoveComponent) =>
            {
                if (jumpComponent.isJump && jumpComponent.desiredJumpSpeed > 0)
                {
                    //更新跳跃状态
                    if (stepMoveComponent.velocity.y > 0)
                    {
                        jumpComponent.jumpStateType = JumpStateType.Jumping;
                    }
                    else
                    {
                        if (transformComponent.position.y - groundComponent.groundPointInfo.point.y < jumpComponent.startJumpHeight)
                        {
                            jumpComponent.jumpStateType = JumpStateType.JumpDown;
                        }
                        else
                        {
                            jumpComponent.jumpStateType = JumpStateType.Jumping;
                        }
                        if (groundComponent.isGround)
                        {
                            jumpComponent.isJump = false;
                            jumpComponent.jumpStateType = JumpStateType.None;
                        }
                    }
                }
            });
        }
    }
}