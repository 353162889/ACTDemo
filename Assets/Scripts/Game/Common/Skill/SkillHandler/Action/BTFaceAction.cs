﻿using Cinemachine.Utility;
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
            var entity = context.skillComponent.componentEntity;
            var faceSystem = context.world.GetExistingSystem<FaceSystem>();
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                if (faceSystem != null)
                {
                    if (data.faceType == FaceType.InputDirection)
                    {
                        var directionMoveComonent = context.world.GetComponent<DirectionMoveComponent>(entity);
                        if (directionMoveComonent != null)
                        {
                            faceSystem.FaceTo(entity, directionMoveComonent.inputDirection, data.immediately);
                        }
                    }
                    else if (data.faceType == FaceType.TargetDirection)
                    {
                        faceSystem.FaceTo(entity, context.skillData.targetDirection, data.immediately);
                    }
                    else if (data.faceType == FaceType.TargetPosition)
                    {
                        var transformComponent = context.world.GetComponent<TransformComponent>(entity);
                        var direction = context.skillData.targetPosition - transformComponent.position;
                        faceSystem.FaceTo(entity, direction, data.immediately);
                    }
                    else if (data.faceType == FaceType.Target)
                    {
                        var transformComponent = context.world.GetComponent<TransformComponent>(entity);
                        var targetTransformComponent =
                            context.world.GetComponent<TransformComponent>(context.skillData.target);
                        if (targetTransformComponent != null)
                        {
                            var direction = targetTransformComponent.position - transformComponent.position;
                            faceSystem.FaceTo(entity, direction, data.immediately);
                        }
                    }
                }
            }
            if (!data.immediately && faceSystem != null && faceSystem.IsRotating(entity))
            {
                return BTStatus.Running;
            }
            return BTStatus.Success;
        }
    }
}