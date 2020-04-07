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

        public void InputFace(Entity entity, Vector3 direction, bool immediately = false, bool ignoreY = true)
        {
            var faceComponent = World.GetComponent<FaceComponent>(entity);
            if (faceComponent == null) return;
            if (forbidSystem.IsForbid(faceComponent.componentEntity, ForbidType.InputFace)) return;
            FaceTo(faceComponent, direction, immediately, ignoreY);
        }

        public void SetFace(Entity entity, Quaternion rotation)
        {
            var faceComponent = World.GetComponent<FaceComponent>(entity);
            if (faceComponent == null) return;
            var transformComponent = World.GetComponent<TransformComponent>(faceComponent.componentEntity);
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

        public void FaceTo(Entity entity, Vector3 direction, bool immediately = false, bool ignoreY = true)
        {
            var faceComponent = World.GetComponent<FaceComponent>(entity);
            if (faceComponent != null)
            {
                FaceTo(faceComponent, direction, immediately);
            }
        }

        public void StopFace(FaceComponent faceComponent)
        {
            faceComponent.isRotating = false;
            faceComponent.immediately = false;
            faceComponent.rotation = Quaternion.identity;
        }

        public bool IsRotating(Entity entity)
        {
            var faceComponent = World.GetComponent<FaceComponent>(entity);
            if (faceComponent == null) return false;
            return faceComponent.isRotating;
        }

        private void FaceTo(FaceComponent faceComponent, Vector3 direction, bool immediately = false, bool ignoreY = true)
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

        private void FaceTo(FaceComponent faceComponent, Quaternion rotation, bool immediately = false)
        {
            var transformComponent = World.GetComponent<TransformComponent>(faceComponent.componentEntity);
            if (transformComponent.rotation == rotation) return;
            faceComponent.isRotating = true;
            faceComponent.immediately = immediately;
            faceComponent.rotation = rotation;
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