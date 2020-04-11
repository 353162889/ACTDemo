using Framework;
using UnityEngine;

namespace Game
{
    public class BTSkillDirectionInRangeData
    {
        public float startAngle;
        public float endAngle;
    }
    /// <summary>
    /// 当前技能输入方向与本身方向的夹角是否在某个区域之内
    /// </summary>
    public class BTSkillDirectionInRange : BTCondition<SkillBTContext, BTSkillDirectionInRangeData>
    {
        protected override bool Evaluate(SkillBTContext context, BTData btData, BTSkillDirectionInRangeData data)
        {
            var hostTransform = context.world.GetComponent<TransformComponent>(context.skillComponent.componentEntity);
            var hostRotation = hostTransform.rotation;
            var targetDirection = context.skillData.targetInfo.targetDirection;
            targetDirection.y = 0;
            targetDirection.Normalize();
            var targetQuat = Quaternion.LookRotation(targetDirection);
            var quat = Quaternion.Inverse(hostRotation) * targetQuat;
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