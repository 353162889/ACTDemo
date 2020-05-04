using Cinemachine.Utility;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class InAirSystem : ComponentSystem
    {
        private StepMoveSystem stepMoveSystem;
        private ForbidSystem forbidSystem;
        protected override void OnCreate()
        {
            stepMoveSystem = World.GetOrCreateSystem<StepMoveSystem>();
            forbidSystem = World.GetOrCreateSystem<ForbidSystem>();
        }

        public void InputMove(Entity entity, Vector3 direction)
        {
            if (forbidSystem.IsForbid(entity, ForbidType.InputMove))
            {
                return;
            }
            if (direction.AlmostZero()) return;
            var inAirComponent = World.GetComponent<InAirComponent>(entity);
            if (!inAirComponent) return;
            if (!inAirComponent.isInAir) return;
            direction.y = 0;
            direction.Normalize();
            inAirComponent.inputDirection = direction;
        }

        public void StopInputMove(Entity entity)
        {
            var inAirComponent = World.GetComponent<InAirComponent>(entity);
            if (inAirComponent == null) return;
            if (inAirComponent.isInAir)
            {
                inAirComponent.inputDirection = Vector3.zero;
            }
        }

        public void StopMove(Entity entity)
        {
            var inAirComponent = World.GetComponent<InAirComponent>(entity);
            if (inAirComponent == null) return;
            inAirComponent.inputDirection = Vector3.zero;
            inAirComponent.inAirVerticalVelocity = Vector3.zero;
            inAirComponent.startHorizonalDirection = Vector3.zero;
            inAirComponent.startHorizonalSpeed = 0;
            inAirComponent.inputSpeed = 0;
            inAirComponent.velocity = Vector3.zero;
            if (inAirComponent.isInAir)
            {
                //这里主要是重置Y轴方向的速度
                stepMoveSystem.StopMove(entity);
            }
        }

        public void StartMoveToAir(Entity entity, Vector3 horizonalVelcolity, float height, float endGroundHeight = 0f)
        {
            var inAirComponent = World.GetComponent<InAirComponent>(entity);
            var groundComponent = World.GetComponent<GroundComponent>(entity);
            var gravityComponent = World.GetComponent<GravityComponent>(entity);
            if (null == inAirComponent || null == groundComponent || null == gravityComponent) return;
            if (!inAirComponent.isInAir && height <= 0) return;
            inAirComponent.isInAir = true;

            var y = Mathf.Sqrt(2 * height * Mathf.Abs(gravityComponent.gravity));
            inAirComponent.inAirVerticalVelocity = new Vector3(0, y, 0);
            horizonalVelcolity.y = 0;
            inAirComponent.startHorizonalDirection = horizonalVelcolity.normalized;
            inAirComponent.startHorizonalSpeed = horizonalVelcolity.magnitude;
            inAirComponent.velocity = horizonalVelcolity;
            inAirComponent.inputSpeed =
                inAirComponent.startHorizonalSpeed * inAirComponent.inputSpeedFactor;
            inAirComponent.endGroundHeight = endGroundHeight;
        }

        public bool IsInAir(Entity entity)
        {
            var inAirComponent = World.GetComponent<InAirComponent>(entity);
            return inAirComponent != null && inAirComponent.isInAir;
        }

        private void ResetInAir(InAirComponent inAirComponent)
        {
            inAirComponent.isInAir = false;
            inAirComponent.inAirStateType = InAirStateType.None;
            inAirComponent.inAirVerticalVelocity = Vector3.zero;
            inAirComponent.startHorizonalDirection = Vector3.zero;
            inAirComponent.startHorizonalSpeed = 0;
            inAirComponent.inputSpeed = 0;
            inAirComponent.velocity = Vector3.zero;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, InAirComponent inAirComponent, GravityComponent gravityComponent) =>
            {
                if (inAirComponent.isInAir)
                {
                    stepMoveSystem.AppendSingleFrameVelocity(entity, inAirComponent.velocity);
                    stepMoveSystem.AppendVelocity(entity, inAirComponent.inAirVerticalVelocity);
                    inAirComponent.inAirVerticalVelocity = Vector3.zero;

                    //更新速度
                    var offset = inAirComponent.inputDirection * inAirComponent.inputSpeed * Time.deltaTime;
                    var newVelocity = inAirComponent.velocity + offset;
                    if (newVelocity.sqrMagnitude > inAirComponent.startHorizonalSpeed * inAirComponent.startHorizonalSpeed)
                    {
                        newVelocity.Normalize();
                        newVelocity *= inAirComponent.startHorizonalSpeed;
                    }

                    inAirComponent.velocity = newVelocity;
                }
            });
        }

        public void UpdateState()
        {
            Entities.ForEach((Entity entity, TransformComponent transformComponent, InAirComponent inAirComponent,
                GroundComponent groundComponent, StepMoveComponent stepMoveComponent) =>
            {
                if (!inAirComponent.isInAir)
                {
                    if (!groundComponent.isGround)
                    {
                        inAirComponent.isInAir = true;
                    }
                }
                if (inAirComponent.isInAir)
                {
                    //更新空中状态
                    if (stepMoveComponent.velocity.y <= 0)
                    {
                        if (transformComponent.position.y - groundComponent.groundPointInfo.point.y <
                            inAirComponent.endGroundHeight)
                        {
                            inAirComponent.inAirStateType = InAirStateType.InAirBeforeGround;
                        }
                        else
                        {
                            inAirComponent.inAirStateType = InAirStateType.InAir;
                        }
                        if (groundComponent.isGround)
                        {
                            ResetInAir(inAirComponent);
                        }
                    }
                    else
                    {
                        inAirComponent.inAirStateType = InAirStateType.InAir;
                    }
                }
               
            });
        }
    }
}