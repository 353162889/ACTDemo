using Cinemachine.Utility;
using Unity.Entities;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Game
{
    public class FaceSystem : ComponentSystem
    {

        public void FaceTo(FaceComponent faceComponent, Vector3 direction, bool immediately = false, bool ignoreY = true)
        {
            if (ignoreY)
            {
                direction.y = 0;
            }

            if (!direction.AlmostZero())
            {
                FaceTo(faceComponent, Quaternion.LookRotation(direction), immediately);
            }
        }

        public void FaceTo(FaceComponent faceComponent, Quaternion rotation, bool immediately = false)
        {
            faceComponent.isRotating = true;
            faceComponent.immediately = immediately;
            faceComponent.rotation = rotation;
        }

        public bool IsRotating(FaceComponent faceComponent)
        {
            if (faceComponent == null) return false;
            return faceComponent.isRotating && !faceComponent.immediately;
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
                        faceComponent.isRotating = false;
                    }
                    else
                    {
                        Quaternion curQuaternion = transformComponent.rotation;
                        Quaternion targetQuatenion = faceComponent.rotation;
                        transformComponent.rotation = Quaternion.RotateTowards(curQuaternion, targetQuatenion,
                            faceComponent.desiredDegreeSpeed * Time.deltaTime);
                        if (transformComponent.rotation == targetQuatenion)
                        {
                            faceComponent.isRotating = false;
                        }
                    }
                }
            });
        }
    }
}