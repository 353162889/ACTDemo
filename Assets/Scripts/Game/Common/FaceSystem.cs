using Cinemachine.Utility;
using Unity.Entities;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Game
{
    public class FaceSystem : ComponentSystem
    {
        private ForbidSystem forbidSystem;
        protected override void OnCreate()
        {
            forbidSystem = World.GetOrCreateSystem<ForbidSystem>();
        }

        public void FaceTo(FaceComponent faceComponent, Vector3 direction, bool immediately = false, bool ignoreY = true)
        {
            if (ignoreY)
            {
                direction.y = 0;
            }

            if (!direction.AlmostZero())
            {
                direction.Normalize();
                FaceTo(faceComponent, Quaternion.LookRotation(direction), immediately);
            }
        }

        public void FaceTo(FaceComponent faceComponent, Quaternion rotation, bool immediately = false)
        {
            if (forbidSystem.IsForbid(faceComponent.entity, ForbidType.Face)) return;
            var transformComponent = World.GetComponent<TransformComponent>(faceComponent.entity);
            if (transformComponent.rotation == rotation) return;
            faceComponent.isRotating = true;
            faceComponent.immediately = immediately;
            faceComponent.rotation = rotation;
        }

        public void SetFace(Entity entity, Quaternion rotation)
        {
            var faceComponent = World.GetComponent<FaceComponent>(entity);
            if (faceComponent == null) return;
            if (forbidSystem.IsForbid(faceComponent.entity, ForbidType.Face)) return;
            var transformComponent = World.GetComponent<TransformComponent>(faceComponent.entity);
            transformComponent.rotation = rotation;
        }

        public void FaceTo(Entity entity, Quaternion rotation, bool immediately = false)
        {
            var faceComponent = World.GetComponent<FaceComponent>(entity);
            if (faceComponent != null)
            {
                FaceTo(faceComponent, rotation, immediately);
            }
        }

        public void StopFace(FaceComponent faceComponent)
        {
            faceComponent.isRotating = false;
            faceComponent.immediately = false;
            faceComponent.rotation = Quaternion.identity;
        }

        public bool IsRotating(FaceComponent faceComponent)
        {
            if (faceComponent == null) return false;
            return faceComponent.isRotating;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((TransformComponent transformComponent, FaceComponent faceComponent) =>
            {
                if (faceComponent.isRotating)
                {
                    if (faceComponent.immediately)
                    {
                        transformComponent.rotation = faceComponent.rotation;
                        StopFace(faceComponent);
                    }
                    else
                    {
                        Quaternion curQuaternion = transformComponent.rotation;
                        Quaternion targetQuatenion = faceComponent.rotation;
                        transformComponent.rotation = Quaternion.RotateTowards(curQuaternion, targetQuatenion,
                            faceComponent.desiredDegreeSpeed * Time.deltaTime);
                        if (transformComponent.rotation == targetQuatenion)
                        {
                              StopFace(faceComponent);
                        }
                    }
                }
            });
        }
    }
}