using UnityEngine;

namespace Game
{
    public static class BuffHandlerUtility
    {
        public static Vector3 GetBuffDirection(BuffBTContext context, BuffDirectionType directionType, bool ignoreY = true)
        {
            var entityTransformComponent = context.world.GetComponent<TransformComponent>(context.buffComponent.componentEntity);
            var hostRotation = entityTransformComponent.rotation;
            Vector3 direction = entityTransformComponent.forward;
            var damageInfo = context.buffData.damageInfo;
            if (damageInfo.hitInfo.IsEnabled())
            {
                var direct = Vector3.zero;
                if (directionType == BuffDirectionType.ColliderDirect)
                {
                    direct = damageInfo.hitInfo.direct;

                }
                else if (directionType == BuffDirectionType.ColliderCenterToHost)
                {
                    direct = damageInfo.hitInfo.rayDirect;
                }
                else if (directionType == BuffDirectionType.CasterToHost)
                {
                    var casterTransform = context.world.GetComponent<TransformComponent>(damageInfo.caster);
                    direct = entityTransformComponent.position - casterTransform.position;
                }
                if (direct != Vector3.zero)
                    direction = direct;
            }
            if (ignoreY)
                direction.y = 0;
            direction.Normalize();

            return direction;
        }
    }
}