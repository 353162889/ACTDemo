using Unity.Entities;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Game
{
    public class JumpSystem : ComponentSystem
    {
        private StepMoveSystem stepMoveSystem;
        private ForbidSystem forbidSystem;
        private FaceSystem faceSystem;
        private SkillSystem skillSystem;
        protected override void OnCreate()
        {
            stepMoveSystem = World.GetOrCreateSystem<StepMoveSystem>();
            forbidSystem = World.GetOrCreateSystem<ForbidSystem>();
            faceSystem = World.GetOrCreateSystem<FaceSystem>();
            skillSystem = World.GetOrCreateSystem<SkillSystem>();
        }
        public void Jump(Entity entity, Vector3 horizonalVelcolity)
        {
            if (forbidSystem.IsForbid(entity, ForbidType.Jump)) return;
            var jumpComponent = World.GetComponent<JumpComponent>(entity);
            var groundComponent = World.GetComponent<GroundComponent>(entity);
            var gravityComponent = World.GetComponent<GravityComponent>(entity);
            if (null == jumpComponent || null == groundComponent || null == gravityComponent) return;
            if (!groundComponent.isGround || jumpComponent.isJump) return;

            //技能跳跃触发技能更新
            skillSystem.OnJump(entity);

            jumpComponent.isJump = true;
            jumpComponent.startJumpGroundTime = Time.time + jumpComponent.startJumpWaitTime;

            var y = Mathf.Sqrt(2 * jumpComponent.desiredHeight * Mathf.Abs(gravityComponent.gravity));
            jumpComponent.jumpVerticalVelocity = new Vector3(0, y, 0);
            horizonalVelcolity.y = 0;
            jumpComponent.jumpHorizonalVelocity = horizonalVelcolity;
//            faceSystem.FaceTo(entity, jumpComponent.jumpHorizonalVelocity);
            jumpComponent.forbidance.Forbid(ForbidType.InputMove);
            jumpComponent.forbidance.Forbid(ForbidType.InputFace);
        }

        public void ResetJump(JumpComponent jumpComponent)
        {
            jumpComponent.isJump = false;
            jumpComponent.jumpVerticalVelocity = Vector3.zero;
            jumpComponent.jumpHorizonalVelocity = Vector3.zero;
            jumpComponent.forbidance.Reset();
            jumpComponent.jumpStateType = JumpStateType.None;
            jumpComponent.startJumpGroundTime = 0;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, JumpComponent jumpComponent, GravityComponent gravityComponent) =>
            {
                if (jumpComponent.isJump && jumpComponent.desiredHeight > 0)
                {
                    if (Time.time >= jumpComponent.startJumpGroundTime)
                    {
                        stepMoveSystem.AppendSingleFrameVelocity(entity, jumpComponent.jumpHorizonalVelocity);
                        stepMoveSystem.AppendVelocity(entity, jumpComponent.jumpVerticalVelocity);
                        jumpComponent.jumpVerticalVelocity = Vector3.zero;
                    }
                }
            });
        }

        public void UpdateState()
        {
            Entities.ForEach((Entity entity, TransformComponent transformComponent, JumpComponent jumpComponent,
                GroundComponent groundComponent, StepMoveComponent stepMoveComponent) =>
            {
                if (jumpComponent.isJump && jumpComponent.desiredHeight > 0)
                {
                    //更新跳跃状态
                    if (stepMoveComponent.velocity.y > 0 || Time.time < jumpComponent.startJumpGroundTime)
                    {
                        jumpComponent.jumpStateType = JumpStateType.Jumping;
                    }
                    else
                    {
                        if (transformComponent.position.y - groundComponent.groundPointInfo.point.y <
                            jumpComponent.endJumpGroundHeight)
                        {
                            jumpComponent.jumpStateType = JumpStateType.JumpBeforeGround;
                        }
                        else
                        {
                            jumpComponent.jumpStateType = JumpStateType.Jumping;
                        }

                        if (groundComponent.isGround)
                        {
                            ResetJump(jumpComponent);
                        }
                    }
                }
            });
        }
    }
}