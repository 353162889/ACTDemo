using Framework;
using UnityEngine;

namespace Game
{
    public class BTBuffDirectionInRangeData
    {
        public BuffDirectionType directionType;
        public float startAngle;
        public float endAngle;
    }
    /// <summary>
    /// 判断buff接收到的方向与本身方向的夹角是否在某个区域之内
    /// </summary>
    public class BTBuffDirectionInRangeCondition : BTCondition<BuffBTContext, BTBuffDirectionInRangeData>
    {
        protected override bool Evaluate(BuffBTContext context, BTData btData, BTBuffDirectionInRangeData data)
        {
            var direct = BuffHandlerUtility.GetBuffDirection(context, data.directionType);
            var directionRotation = Quaternion.LookRotation(direct);
            var entityTransformComponent = context.world.GetComponent<TransformComponent>(context.buffComponent.componentEntity);
            var hostRotation = entityTransformComponent.rotation;
            var quat = Quaternion.Inverse(hostRotation) * directionRotation;
            float angle = quat.eulerAngles.y;
            angle = Mathf.Repeat(angle, 360f);
            float end = Mathf.Repeat(data.endAngle - data.startAngle, 360);
            float v = Mathf.Repeat(angle - data.startAngle, 360);
            if (v <= end)
            {
                return true;
            }

            return false;
        }
    }
}