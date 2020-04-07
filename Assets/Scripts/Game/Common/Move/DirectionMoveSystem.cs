using Cinemachine.Utility;
using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class DirectionMoveSystem : ComponentSystem
    {
        private StepMoveSystem stepMoveSystem;
        private FaceSystem faceSystem;
        private ForbidSystem forbidSystem;
        private SkillSystem skillSystem;
        protected override void OnCreate()
        {
            stepMoveSystem = World.GetOrCreateSystem<StepMoveSystem>();
            faceSystem = World.GetOrCreateSystem<FaceSystem>();
            forbidSystem = World.GetOrCreateSystem<ForbidSystem>();
            skillSystem = World.GetOrCreateSystem<SkillSystem>();
        }
        public void Move(Entity entity, Vector3 direction, bool changeFace = false)
        {
            if (forbidSystem.IsForbid(entity, ForbidType.InputMove))
            {
                return;
            }
            if (direction.AlmostZero()) return;
            var directMoveComponent = World.GetComponent<DirectionMoveComponent>(entity);
            if (null == directMoveComponent) return;
            var groundComponent = World.GetComponent<GroundComponent>(entity);
            if (!groundComponent.isGround) return;
            direction.y = 0;
            direction.Normalize();
            directMoveComponent.inputDirection = direction;
            directMoveComponent.changeFace = changeFace;
            skillSystem.OnDirectionMove(entity);
        }

        public void StopMove(Entity entity)
        {
            var directMoveComponent = World.GetComponent<DirectionMoveComponent>(entity);
            if (directMoveComponent == null) return;
            directMoveComponent.inputDirection = Vector3.zero;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, DirectionMoveComponent directionMoveComponent, GroundComponent groundComponent) =>
            {
                if (directionMoveComponent.desiredSpeed > 0 && directionMoveComponent.inputDirection != Vector3.zero)
                {
                    if (forbidSystem.IsForbid(entity, ForbidType.InputMove))
                    {
                        StopMove(entity);
                    }
                    else
                    {
                        Vector3 velocity = Vector3.zero;
                        if (groundComponent.isGround && directionMoveComponent.desiredSpeed > 0)
                        {
//                        velocity = directionMoveComponent.inputDirection * directionMoveComponent.desiredSpeed;
                            //根据地面斜率，计算速度
                            var cos = Vector3.Dot(groundComponent.groundPointInfo.normal, Vector3.up);
                            if (cos == 0) cos = 1;
                            velocity = directionMoveComponent.desiredSpeed * directionMoveComponent.inputDirection *
                                       cos;
                        }
                        else
                        {
                            velocity = directionMoveComponent.inputDirection * directionMoveComponent.desiredSpeed;
                        }

                        stepMoveSystem.AppendSingleFrameVelocity(entity, velocity);
                        if (directionMoveComponent.changeFace)
                        {
                            faceSystem.InputFace(entity, directionMoveComponent.inputDirection);
                        }
                    }
                }
            });
        }
    }
}