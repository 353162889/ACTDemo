using Framework;
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
        private InAirSystem inAirSystem;
        protected override void OnCreate()
        {
            stepMoveSystem = World.GetOrCreateSystem<StepMoveSystem>();
            forbidSystem = World.GetOrCreateSystem<ForbidSystem>();
            faceSystem = World.GetOrCreateSystem<FaceSystem>();
            skillSystem = World.GetOrCreateSystem<SkillSystem>();
            inAirSystem = World.GetOrCreateSystem<InAirSystem>();
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
            horizonalVelcolity.y = 0;
            jumpComponent.jumpHorizonalVelocity = horizonalVelcolity;
//            faceSystem.FaceTo(entity, jumpComponent.jumpHorizonalVelocity);
//            jumpComponent.forbidance.Forbid(ForbidType.InputMove);
//            jumpComponent.forbidance.Forbid(ForbidType.InputFace);
            //起跳的前摇不能释放技能
            jumpComponent.forbidance.Forbid(ForbidType.Ability);
        }

        public void ResetJump(JumpComponent jumpComponent)
        {
            inAirSystem.StopMove(jumpComponent.componentEntity);
            jumpComponent.isJump = false;
            jumpComponent.jumpHorizonalVelocity = Vector3.zero;
            jumpComponent.forbidance.Reset();
            jumpComponent.jumpStateType = JumpStateType.None;
            jumpComponent.startJumpGroundTime = 0;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, JumpComponent jumpComponent, GravityComponent gravityComponent, InAirComponent inAirComponent) =>
            {
                if (jumpComponent.isJump && jumpComponent.desiredHeight > 0)
                {
                    if (Time.time >= jumpComponent.startJumpGroundTime && !inAirComponent.isInAir)
                    {
                        inAirSystem.StartMoveToAir(entity, jumpComponent.jumpHorizonalVelocity, jumpComponent.desiredHeight, jumpComponent.endJumpGroundHeight);
                        //跳起来之后才可以释放技能
                        jumpComponent.forbidance.ResumeForbid(ForbidType.Ability);
                    }
                }
            });
        }

        public void UpdateState()
        {
            Entities.ForEach((Entity entity, JumpComponent jumpComponent, InAirComponent inAirComponent) =>
            {
                if (jumpComponent.isJump && jumpComponent.desiredHeight > 0)
                {
                    //更新跳跃状态
                    if (Time.time < jumpComponent.startJumpGroundTime)
                    {
                        jumpComponent.jumpStateType = JumpStateType.JumpBeforeAir;
                    }
                    else
                    {
                        if (inAirComponent.inAirStateType == InAirStateType.InAirBeforeGround)
                        {
                            jumpComponent.jumpStateType = JumpStateType.JumpBeforeGround;
                        }
                        else
                        {
                            jumpComponent.jumpStateType = JumpStateType.Jumping;
                        }
                    }
                    if (jumpComponent.jumpStateType != JumpStateType.JumpBeforeAir && !inAirComponent.isInAir)
                    {
                        ResetJump(jumpComponent);
                    }
                }
            });
        }
    }
}