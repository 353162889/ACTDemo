using Cinemachine.Utility;
using Framework;
using UnityEngine;

namespace Game
{
    public class BTInputDirectionInRangeData
    {
        public float startAngle;
        public float endAngle;
    }
    /// <summary>
    /// 当前输入方向与本身方向的夹角是否在某个区域之内
    /// </summary>
    public class BTInputDirectionInRange : BTCondition<SkillBTContext, BTInputDirectionInRangeData>
    {
        protected override bool Evaluate(SkillBTContext context, BTData btData, BTInputDirectionInRangeData data)
        {
            var directionMoveComponent = context.world.GetComponent<DirectionMoveComponent>(context.skillComponent.componentEntity);
            if (directionMoveComponent != null && !directionMoveComponent.inputDirection.AlmostZero())
            {
                var hostTransform = context.world.GetComponent<TransformComponent>(context.skillComponent.componentEntity);
                var hostRotation = hostTransform.rotation;
                var targetQuat = Quaternion.LookRotation(directionMoveComponent.inputDirection);
                var quat = Quaternion.Inverse(hostRotation) * targetQuat;
                float angle = quat.eulerAngles.y;
                angle = Mathf.Repeat(angle, 360f);
                float end = Mathf.Repeat(data.endAngle - data.startAngle, 360);
                float v = Mathf.Repeat(angle - data.startAngle, 360);
                if (v <= end)
                {
                    return true;
                }
            }
            return false;
        }
    }
}