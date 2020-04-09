using System;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class CameraLockSystem : ComponentSystem
    {
        private static string[] enableLockCameraType = new string[]
        {
            CameraStrategy.NormalWalkCamera,
        };
        private CameraSystem cameraSystem;
        public CameraLockComponent lockComponent { get; private set; }

        private RaycastHit[] cacheRaycasthits = new RaycastHit[5];
        public event Action<bool> OnEnableChange;
        public event Action<Entity> OnLockTargetChange; 

        protected override void OnCreate()
        {
            cameraSystem = World.GetOrCreateSystem<CameraSystem>();
            lockComponent = World.AddSingletonComponent<CameraLockComponent>();
            CameraSystemOnOnCameraChange(cameraSystem.CurCamera());
            cameraSystem.OnCameraChange += CameraSystemOnOnCameraChange;
        }

        protected override void OnDestroy()
        {
            OnEnableChange = null;
            OnLockTargetChange = null;
        }

        private void CameraSystemOnOnCameraChange(string camera)
        {
            lockComponent.enable = enableLockCameraType.Contains(camera);
        }

        public void SetViewport(Vector2 pos)
        {
            lockComponent.viewportPos = pos;
        }

        //获取射线与平面的焦点
        private Vector3 GetStartRay(Vector3 origin, Vector3 direction, Vector3 planePoint, Vector3 planeNormal)
        {
            float d = Vector3.Dot(planePoint - origin, planeNormal) / Vector3.Dot(direction.normalized, planeNormal);
            return d * direction.normalized + origin;
        }

        protected override void OnUpdate()
        {
            if (lockComponent.enable)
            {
                //每帧从相机中央发射射线检测是否有敌方
                var camera = cameraSystem.GetMainCamera();
                var ray = camera.ViewportPointToRay(lockComponent.viewportPos);
                var followEntity = cameraSystem.FollowEntity();
                var transformComponent = World.GetComponent<TransformComponent>(followEntity);
                var origin = GetStartRay(ray.origin, ray.direction, transformComponent.position,
                    transformComponent.rotation * Vector3.forward);
                ray.origin = origin;
                int count = Physics.RaycastNonAlloc(ray, cacheRaycasthits, lockComponent.maxDistance, LayerDefine.BehitColliderMask);
                if (count > 0)
                {
                    float minDistance = float.MaxValue;
                    Entity entity = Entity.Null;
                    for (int i = 0; i < count; i++)
                    {
                        var hitInfo = cacheRaycasthits[i];
                        var entityMono = hitInfo.collider.GetComponentInParent<EntityMonoBehaviour>();
                        if (entityMono != null)
                        {
                            if (cameraSystem.FollowEntity() != entityMono.entity)
                            {
                                if (hitInfo.distance < minDistance)
                                {
                                    entity = entityMono.entity;
                                    minDistance = hitInfo.distance;
                                }
                            }
                        }
                    }

                    if (entity != Entity.Null)
                    {
                        StartLock(entity);
                    }
                }

                if (lockComponent.lockEntity != Entity.Null && lockComponent.endLockTime <= Time.time)
                {
                    StopLock();
                }
            }
            else
            {
                StopLock();
            }
        }

        public void OnDrawGizmos()
        {
            if (lockComponent.enable)
            {
                var oldColor = Gizmos.color;
                Gizmos.color = Color.red;
                var camera = cameraSystem.GetMainCamera();
                var ray = camera.ViewportPointToRay(lockComponent.viewportPos);
                var followEntity = cameraSystem.FollowEntity();
                var transformComponent = World.GetComponent<TransformComponent>(followEntity);
                var origin = GetStartRay(ray.origin, ray.direction, transformComponent.position,
                    transformComponent.rotation * Vector3.forward);
                ray.origin = origin;
                Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * lockComponent.maxDistance);
                Gizmos.color = oldColor;
            }
        }

        public bool IsLocking()
        {
            return lockComponent.lockEntity != Entity.Null;
        }

        public bool IsEnable()
        {
            return lockComponent.enable;
        }

        public Entity LockEntity()
        {
            return lockComponent.lockEntity;
        }

        public void StartLock(Entity entity)
        {
            lockComponent.lockEntity = entity;
            lockComponent.endLockTime = Time.time + lockComponent.lockSpaceTime;
        }

        public void StopLock()
        {
            lockComponent.lockEntity = Entity.Null;
            lockComponent.endLockTime = 0;
        }
    }
}