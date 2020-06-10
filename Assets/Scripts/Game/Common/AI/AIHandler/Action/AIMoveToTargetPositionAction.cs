using Framework;
using UnityEngine;

namespace Game
{
    public class AIMoveToTargetPositionActionData
    {
    }
    public class AIMoveToTargetPositionAction : BTAction<AIBTContext, AIMoveToTargetPositionActionData>
    {
        protected override BTStatus Handler(AIBTContext context, BTData btData, AIMoveToTargetPositionActionData data)
        {
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                Vector3 targetPosition;
                var exist = context.blackBoard.GetData<Vector3>(AIBlackBoardKeys.TargetPosition, out targetPosition);
                if (exist)
                {
                    var pointsMoveSystem = context.world.GetExistingSystem<PointsMoveSystem>();
                    if (pointsMoveSystem != null)
                    {
                        pointsMoveSystem.Move(context.aiComponent.componentEntity, targetPosition, true);
                    }
                }
                else
                {
                    return BTStatus.Fail;
                }
            }

            var pointsMoveComponent = context.world.GetComponent<PointsMoveComponent>(context.aiComponent.componentEntity);
            if (pointsMoveComponent == null) return BTStatus.Fail;
            if (pointsMoveComponent.isMoving) return BTStatus.Running;
            return BTStatus.Success;
        }
    }
}