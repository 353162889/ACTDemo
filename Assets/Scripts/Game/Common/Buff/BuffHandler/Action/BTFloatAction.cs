using System.Runtime.Remoting.Contexts;
using Framework;
using UnityEngine;

namespace Game
{
    public enum BTFloatDirectionType
    {
        None,
        HitDireciton,
        HitRayDirection,
    }
    public class BTFloatActionData
    {
        public BTFloatDirectionType directionType;
        public float height;
        public float horizontalSpeed;
        public string anim;
    }
    public class BTFloatAction : BTAction<BuffBTContext, BTFloatActionData>
    {
        protected override BTStatus Handler(BuffBTContext context, BTData btData, BTFloatActionData data)
        {
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                Vector3 horizontalVelocity = Vector3.zero;
                if (data.directionType == BTFloatDirectionType.HitDireciton)
                {
                    var direct = context.buffData.damageInfo.hitInfo.direct;
                    horizontalVelocity.x = direct.x;
                    horizontalVelocity.z = direct.z;
                }
                else if (data.directionType == BTFloatDirectionType.HitRayDirection)
                {
                    var direct = context.buffData.damageInfo.hitInfo.rayDirect;
                    horizontalVelocity.x = direct.x;
                    horizontalVelocity.z = direct.z;
                }
                horizontalVelocity.Normalize();
                horizontalVelocity *= data.horizontalSpeed;
                var floatSystem = context.world.GetExistingSystem<BuffFloatSystem>();
                floatSystem.Float(context.buffComponent.componentEntity, horizontalVelocity, data.height);
                //播放buff动画
                if (!string.IsNullOrEmpty(data.anim))
                {
                    var animationSystem = context.world.GetExistingSystem<AnimationSystem>();
                    animationSystem.SetAnimatorParam(context.buffComponent.componentEntity, data.anim);
                }

            }

            var floatCompoent = context.world.GetComponent<BuffFloatComponent>(context.buffComponent.componentEntity);
            if (floatCompoent == null || !floatCompoent.isFloat)
            {
                return BTStatus.Success;
            }
            return BTStatus.Running;
        }

        protected override void Clear(BuffBTContext context, BTData btData, BTFloatActionData data)
        {
            if (!IsCleared(context, btData))
            {
                var floatSystem = context.world.GetExistingSystem<BuffFloatSystem>();
                floatSystem.ResetFloat(context.buffComponent.componentEntity);
            }
            base.Clear(context, btData, data);
        }
    }
}