using Cinemachine.Utility;
using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class StepMoveSystem : ComponentSystem
    {
        private static float slopeAngleRate = Mathf.Cos(75);
        private MapSystem mapSystem;
        private FaceSystem faceSystem;
        protected override void OnCreate()
        {
            mapSystem = World.GetOrCreateSystem<MapSystem>();
            faceSystem = World.GetOrCreateSystem<FaceSystem>();
        }

        public void AnimationMove(Entity entity, float[] points, float time, float deltaTime, Quaternion startRotation, bool ignoreRotation)
        {
            float startTime = time;
            float endTime = time + deltaTime;
            var offsetInfo = AnimationMoveUtility.GetOffset(points, startRotation, startTime, endTime);
            Vector3 velocity = offsetInfo.offsetPos / Time.deltaTime;
            AppendSingleFrameVelocity(entity, velocity, false);
            if (!ignoreRotation)
            {
                var faceSystem = World.GetExistingSystem<FaceSystem>();
                var transformComponent =
                    World.GetComponent<TransformComponent>(entity);
                faceSystem.FaceTo(entity, offsetInfo.offsetRot * transformComponent.rotation, true);
            }
        }

        public void AppendSingleFrameVelocity(Entity entity, Vector3 velocity, bool changeFace = false)
        {
            var moveComponent = World.GetComponent<StepMoveComponent>(entity);
            if (velocity.AlmostZero()) return;
            if (null == moveComponent) return;
            moveComponent.isMoving = true;
            moveComponent.innerFrameVelocity += velocity;
            if (changeFace)
            {
                moveComponent.changeFace = changeFace;
            }
        }

        public void AppendVelocity(Entity entity, Vector3 velocity, bool changeFace = false)
        {
            var moveComponent = World.GetComponent<StepMoveComponent>(entity);
            if (velocity.AlmostZero()) return;
            if (null == moveComponent) return;
            moveComponent.isMoving = true;
            moveComponent.innerVelocity += velocity;
            if (changeFace)
            {
                moveComponent.changeFace = changeFace;
            }
        }

        public void StopMove(Entity entity)
        {
            var moveComponent = World.GetComponent<StepMoveComponent>(entity);
            StopMove(moveComponent);
        }

        private void StopMove(StepMoveComponent moveComponent)
        {
            if (null == moveComponent) return;
            moveComponent.isMoving = false;
            moveComponent.changeFace = false;
            moveComponent.innerFrameVelocity = Vector3.zero;
            moveComponent.innerVelocity = Vector3.zero;
            moveComponent.velocity = Vector3.zero;
            moveComponent.desiredVelocity = Vector3.zero;
        }

        private bool CheckGroundByPosition(Vector3 pos, float offset = 0.1f)
        {
            float y = mapSystem.GetGroundInfo(pos).point.y;
            return CheckGroundByPositionAndHeight(pos, y, offset);
        }

        private bool CheckGroundByPositionAndHeight(Vector3 pos, float currentHeight, float offset = 0.1f)
        {
            return pos.y - currentHeight < offset;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, TransformComponent transformComponent, StepMoveComponent moveComponent, GroundComponent groundComponent) =>
            {
                if (moveComponent.isMoving)
                {
                    var velocity = moveComponent.innerFrameVelocity + moveComponent.innerVelocity;
                    moveComponent.desiredVelocity = velocity;
                    groundComponent.moveFlag = 0;
                    //物理碰撞更新速度
                    var physicComponent = World.GetComponent<PhysicComponent>(entity);
                    if (physicComponent != null)
                    {
                        var collisions = physicComponent.collisionListener.lstCollisionInfos;
                        if (collisions.Count > 0)
                        {
                            foreach (var collision in collisions)
                            {
                                for (int i = 0; i < collision.contacts.Count; i++)
                                {
                                    var contact = collision.contacts[i];
                                    var normal = contact.normal;
                                    //移动忽略y轴方向
                                    float normalY = normal.y;
                                    normal.y = 0;
                                    if (!normal.AlmostZero())
                                    {
                                        normal.Normalize();
                                        float dotValue = Vector3.Dot(velocity, normal);
                                        if (dotValue < 0)
                                        {
                                            velocity = velocity - dotValue * normal;
                                            moveComponent.innerVelocity -=
                                                Vector3.Dot(moveComponent.innerVelocity, normal) * normal;
                                        }

                                        groundComponent.moveFlag |= (byte)CollisionFlags.Sides;
                                    }

                                    //头顶或者脚底有障碍物时，变更垂直速度
                                    if (Mathf.Abs(normalY) > 0.01f)
                                    {
                                        if (normalY * velocity.y < 0)
                                        {
                                            velocity.y = 0;
                                            moveComponent.innerVelocity.y = 0;
                                            if (normalY < 0)
                                            {
                                                groundComponent.moveFlag |= (byte)CollisionFlags.Above;
                                            }
                                            else
                                            {
                                                groundComponent.moveFlag |= (byte)CollisionFlags.Below;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //地面移动更新速度
                    var prePos = transformComponent.position;
                    var moveOffset = velocity * Time.deltaTime;
                    var newPos = prePos + moveOffset;
                    var groundInfo = mapSystem.GetGroundInfo(newPos);
                    groundComponent.groundPointInfo = groundInfo;
                    float newPosHeight = groundInfo.point.y;
                    //精细判断当前是否已经在地面上
                    bool isGround = CheckGroundByPositionAndHeight(newPos, newPosHeight, 0.1f);
                    //如果原始Y轴的速度大于0
                    if (moveComponent.desiredVelocity.y <= 0)
                    {
                        //检测走到凹槽之后，角色默认不实用重力，直接设置当前高度
                        var oldIsGround = groundComponent.isGround;
                        //在地面上最大误差范围
                        bool isRealGround = CheckGroundByPositionAndHeight(newPos, newPosHeight, 0.5f);
                        //遇到斜坡的时候通过斜率大小判定是否isground
                        if (oldIsGround && !isGround && isRealGround)
                        {
                            if (Vector3.Dot(groundInfo.normal, Vector3.up) > slopeAngleRate)
                            {
                                isGround = true;
                            }
                        }

                        if (isGround)
                        {
                            newPos.y = newPosHeight;
                        }
                    }

                    if (isGround)
                    {
                        if (velocity.y < 0) velocity.y = 0;
                        if (moveComponent.innerVelocity.y < 0) moveComponent.innerVelocity.y = 0;
                    }

                    //更新最新位置
                    transformComponent.position = newPos;
                    //检测最新位置是否是在地面
                    groundComponent.isGround = isGround;

//                    //更新面向
                    if (moveComponent.changeFace)
                    {
                        var targetDirection = newPos - prePos;
                        faceSystem.FaceTo(entity, targetDirection, false);
                    }

                    moveComponent.velocity = velocity;
                    if (velocity.AlmostZero())
                    {
                        StopMove(moveComponent);
                    }
                }
               
                moveComponent.innerFrameVelocity = Vector3.zero;
            });
        }
    }
}