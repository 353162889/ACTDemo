using System.Collections.Generic;
using Cinemachine.Utility;
using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class PointsMoveSystem : ComponentSystem
    {
        private StepMoveSystem stepMoveSystem;
        private FaceSystem faceSystem;
        private ForbidSystem forbidSystem;
        protected override void OnCreate()
        {
            stepMoveSystem = World.GetOrCreateSystem<StepMoveSystem>();
            faceSystem = World.GetOrCreateSystem<FaceSystem>();
            forbidSystem = World.GetOrCreateSystem<ForbidSystem>();
        }

        public bool Move(Entity entity, List<Vector3> path, bool changeFace = true)
        {
            var moveComponent = World.GetComponent<PointsMoveComponent>(entity);
            if (moveComponent == null) return false;
            var transformComponent = World.GetComponent<TransformComponent>(entity);
            if (moveComponent == null) return false;
            if (path == null || path.Count == 0) return false;
            if (forbidSystem.IsForbid(entity, ForbidType.InputMove)) return false;
            bool stopToMove = true;
            if (moveComponent.isMoving)
            {
                ClearData(moveComponent);
            }
            moveComponent.queuePath.Clear();
            moveComponent.targetPosition = transformComponent.position;
            for (int i = 0; i < path.Count; i++)
            {
                moveComponent.queuePath.Enqueue(path[i]);
                if (i == path.Count - 1) moveComponent.targetPosition = path[i];
            }

            moveComponent.nextPosition = transformComponent.position;
            Vector3 position = moveComponent.nextPosition;
            Vector3 forward = Vector3.forward;
            if (!DequeuePoint(moveComponent, ref moveComponent.nextPosition, ref position, ref forward, 0))
            {
                return false;
            }
            moveComponent.curPosition = transformComponent.position;
            moveComponent.curForward = transformComponent.forward;
            moveComponent.isMoving = true;
            moveComponent.changeFace = changeFace;
            return true;
        }

        public bool Move(Entity entity, Vector3 targetPosition, bool changeFace = true)
        {
            var moveComponent = World.GetComponent<PointsMoveComponent>(entity);
            if (moveComponent == null) return false;
            var transformComponent = World.GetComponent<TransformComponent>(entity);
            if (moveComponent == null) return false;
            if (forbidSystem.IsForbid(entity, ForbidType.InputMove)) return false;
            bool stopToMove = true;
            if (moveComponent.isMoving)
            {
                ClearData(moveComponent);
            }
            moveComponent.queuePath.Clear();
            moveComponent.targetPosition = targetPosition;
            moveComponent.queuePath.Enqueue(targetPosition);

            moveComponent.nextPosition = transformComponent.position;
            Vector3 position = moveComponent.nextPosition;
            Vector3 forward = Vector3.forward;
            if (!DequeuePoint(moveComponent, ref moveComponent.nextPosition, ref position, ref forward, 0))
            {
                return false;
            }
            moveComponent.curPosition = transformComponent.position;
            moveComponent.curForward = transformComponent.forward;
            moveComponent.isMoving = true;
            moveComponent.changeFace = changeFace;
            return true;
        }

        public void StopMove(Entity entity)
        {
            var pointsMoveComponent = World.GetComponent<PointsMoveComponent>(entity);
            if (pointsMoveComponent != null)
            {
                StopMove(pointsMoveComponent);
            }
        }

        public void StopMove(PointsMoveComponent moveComponent)
        {
            ClearData(moveComponent);
        }

        private void SetPosition(PointsMoveComponent moveComponent, Vector3 position, Vector3 forward)
        {
            moveComponent.curPosition = position;
            moveComponent.curForward = forward;
        }

        private void ClearData(PointsMoveComponent moveComponent)
        {
            moveComponent.queuePath.Clear();
            moveComponent.isMoving = false;
        }

        private bool DequeuePoint(PointsMoveComponent moveComponent, ref Vector3 curNextPosition, ref Vector3 curPosition, ref Vector3 curForward, float moveDis)
        {
            if (!(curNextPosition - curPosition).AlmostZero())
            {
                CLog.LogError("当前下一个点与当前位置不一致(上层逻辑有问题)");
                return false;
            }
            Vector3 preNextPosition;
            if (moveComponent.queuePath.Count > 0)
            {
                bool succ = false;
                while (moveDis >= 0 && moveComponent.queuePath.Count > 0)
                {
                    var tempNextPosition = moveComponent.queuePath.Dequeue();
                    if((tempNextPosition - curNextPosition).AlmostZero())continue;
                    preNextPosition = curNextPosition;
                    curNextPosition = tempNextPosition;
                    curForward = curNextPosition - preNextPosition;
                    float dis = curForward.magnitude;
                    curForward.Normalize();
                    succ = true;
                    if (moveDis < dis)
                    {
                        curPosition = preNextPosition + curForward * moveDis;
                        return true;
                    }

                    moveDis -= dis;
                }

                if (moveDis >= 0 && succ)
                {
                    curPosition = curNextPosition;
                    return true;
                }
            }
            return false;
        }

        private bool MoveFrame(PointsMoveComponent moveComponent)
        {
            Vector3 offset = moveComponent.nextPosition - moveComponent.curPosition;
            float moveDis = moveComponent.desiredSpeed * Time.deltaTime;
            if (!offset.AlmostZero())
            {
                if (offset.sqrMagnitude > moveDis * moveDis)
                {
                    Vector3 forward = offset.normalized;
                    Vector3 position = moveComponent.curPosition + forward * moveDis;
                    SetPosition(moveComponent, position, forward);
                }
                else
                {
                    moveDis -= offset.magnitude;
                    Vector3 position = moveComponent.nextPosition;
                    Vector3 forward = moveComponent.curForward;
                    if (!DequeuePoint(moveComponent, ref moveComponent.nextPosition, ref position, ref forward,
                        moveDis))
                    {
                        return false;
                    }
                    SetPosition(moveComponent, position, forward);
                }
            }
            else
            {
                Vector3 position = moveComponent.nextPosition;
                Vector3 forward = moveComponent.curForward;
                if (!DequeuePoint(moveComponent, ref moveComponent.nextPosition, ref position, ref forward,
                    moveDis))
                {
                    return false;
                }
                SetPosition(moveComponent, position, forward);
            }

            return true;
        }


        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, PointsMoveComponent moveComponent, TransformComponent transformComponent, GroundComponent groundComponent,
                InAirComponent inAirComponent, PropertyComponent propertyComponent) =>
            {
                if (moveComponent.isMoving)
                {
                    if (inAirComponent.isInAir || forbidSystem.IsForbid(entity, ForbidType.InputMove))
                    {
                        StopMove(moveComponent);
                    }
                    else
                    {
                        moveComponent.desiredSpeed = propertyComponent.moveSpeed;
                        if (moveComponent.desiredSpeed > 0 && !inAirComponent.isInAir && groundComponent.isGround)
                        {
                            //纠正当前位置
                            moveComponent.curPosition = transformComponent.position;
                            moveComponent.curForward = transformComponent.forward;
                            if (MoveFrame(moveComponent))
                            {
                                var velocity = moveComponent.curPosition - transformComponent.position;
                                stepMoveSystem.AppendSingleFrameVelocity(entity, velocity / Time.deltaTime);
                                if (moveComponent.changeFace)
                                {
                                    faceSystem.InputFace(entity, moveComponent.curForward);
                                }
                            }
                            else
                            {
                                StopMove(moveComponent);
                            }
                        }
                        else
                        {
                            StopMove(moveComponent);
                        }
                    }
                }
            });
        }
    }
}