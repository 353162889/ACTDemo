using Cinemachine.Utility;
using Framework;

namespace Game
{
    public enum FaceType
    {
        InputDirection,
        TargetDirection,
        TargetPosition,
        Target,
    }
    public class BTFaceData
    {
        public FaceType faceType;
        public bool immediately;
    }
    public class BTFaceAction : BTAction<SkillBTContext, BTFaceData>
    {
        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTFaceData data)
        {
            var entity = context.skillComponent.entity;
            var faceSystem = context.world.GetExistingSystem<FaceSystem>();
            var faceComponent = context.world.GetComponent<FaceComponent>(entity);
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                if (faceSystem != null && faceComponent != null)
                {
                    if (data.faceType == FaceType.InputDirection)
                    {
                        var directionMoveComonent = context.world.GetComponent<DirectionMoveComponent>(entity);
                        if (directionMoveComonent != null)
                        {
                            faceSystem.FaceTo(faceComponent, directionMoveComonent.inputDirection, data.immediately);
                        }
                    }
                    else if (data.faceType == FaceType.TargetDirection)
                    {
                        faceSystem.FaceTo(faceComponent, context.skillData.targetDirection, data.immediately);
                    }
                    else if (data.faceType == FaceType.TargetPosition)
                    {
                        var transformComponent = context.world.GetComponent<TransformComponent>(entity);
                        var direction = context.skillData.targetPosition - transformComponent.position;
                        faceSystem.FaceTo(faceComponent, direction, data.immediately);
                    }
                    else if (data.faceType == FaceType.Target)
                    {
                        var transformComponent = context.world.GetComponent<TransformComponent>(entity);
                        var targetTransformComponent =
                            context.world.GetComponent<TransformComponent>(context.skillData.target);
                        if (targetTransformComponent != null)
                        {
                            var direction = targetTransformComponent.position - transformComponent.position;
                            faceSystem.FaceTo(faceComponent, direction, data.immediately);
                        }
                    }
                }
            }
            if (faceSystem != null && faceSystem.IsRotating(faceComponent))
            {
                return BTStatus.Running;
            }
            this.Clear(context, btData);
            return BTStatus.Success;
        }
    }
}