using System.Collections.Generic;
using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class AIStandoffActionData
    {
        public int randomCount;
        public float minAngle = 1f;
        public float maxAngle = 90f;
        public float minRadius = 5f;
        public float maxRadius = 10f;
    }
    public class AIStandoffAction : BTAction<AIBTContext, AIStandoffActionData>
    {
        internal protected struct InnerAIStandoffActionData
        {
            public int repeatCount;
            public float sign;
        }
        private static InnerAIStandoffActionData DefaultActionData = new InnerAIStandoffActionData() { repeatCount = 0, sign = 0 };

        protected override BTStatus Handler(AIBTContext context, BTData btData, AIStandoffActionData data)
        {
            if (context.worldState.target == Entity.Null) return BTStatus.Fail;
            bool createNewPosition = false;
            var cacheData = context.executeCache.GetCache(btData.dataIndex, DefaultActionData);
            if (context.executeCache.GetExecuteStatus(btData.dataIndex) == BTExecuteStatus.Ready)
            {
                cacheData.repeatCount = data.randomCount;
                cacheData.sign = Mathf.Sign(Random.Range(0, 2) - 1);
                createNewPosition = true;
            }
            var pointsMoveComponent = context.world.GetComponent<PointsMoveComponent>(context.aiComponent.componentEntity);
            if (pointsMoveComponent == null) return BTStatus.Fail;
            var targetTransformComponent =
                context.world.GetComponent<TransformComponent>(context.worldState.target);
            var hostTransformComponent =
                context.world.GetComponent<TransformComponent>(context.aiComponent.componentEntity);
            if (createNewPosition || !pointsMoveComponent.isMoving)
            {
                if (cacheData.repeatCount > 0)
                {
                    cacheData.repeatCount--;
                    float randomAngle = Random.Range(data.minAngle, data.maxAngle) * cacheData.sign;
                    float randomRadius = Random.Range(data.minRadius, data.maxRadius);
                    var offset = hostTransformComponent.position - targetTransformComponent.position;
                    offset.Normalize();
                    var targetPosition = Quaternion.AngleAxis(randomAngle, Vector3.up) * (offset * randomRadius) +
                                         targetTransformComponent.position;
                    var pointsMoveSystem = context.world.GetExistingSystem<PointsMoveSystem>();
                    var mapSystem = context.world.GetExistingSystem<MapSystem>();
                    float y = mapSystem.GetGroundInfo(targetPosition).point.y;
                    targetPosition.y = y;
                    pointsMoveSystem?.Move(context.aiComponent.componentEntity, targetPosition, false);
                    context.executeCache.SetCache(btData.dataIndex, cacheData);
                }
                else
                {
                    return BTStatus.Success;
                }
            }
            //一直朝向玩家
            var faceSystem = context.world.GetExistingSystem<FaceSystem>();
            faceSystem?.InputFace(context.aiComponent.componentEntity, targetTransformComponent.position - hostTransformComponent.position);
            return BTStatus.Running;
        }
    }
}