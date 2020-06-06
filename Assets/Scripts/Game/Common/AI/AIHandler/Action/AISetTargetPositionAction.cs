using Framework;
using UnityEngine;

namespace Game
{
    public enum SetTargetPositionType
    {
        //出身位置为中心的半径内随机
        BornPositionRadiusRandom,
    }
    public class AISetTargetPositionActionData
    {
        public SetTargetPositionType positionType;
        public float minRadius;
        public float maxRadius;
    }
    public class AISetTargetPositionAction : BTAction<AIBTContext, AISetTargetPositionActionData>
    {
        protected override BTStatus Handler(AIBTContext context, BTData btData, AISetTargetPositionActionData data)
        {
            if (data.positionType == SetTargetPositionType.BornPositionRadiusRandom)
            {
                var entity = context.aiComponent.componentEntity;
                var entityCommonInfoComponent = context.world.GetComponent<EntityCommonInfoComponent>(entity);
                if (entityCommonInfoComponent == null) return BTStatus.Fail;
                var position = entityCommonInfoComponent.bornPosition;
                var randomRadius = Random.Range(data.minRadius, data.maxRadius);
                var randomRadians = Random.Range(0, Mathf.PI * 2);
                position.x = position.x + Mathf.Cos(randomRadians) * randomRadius;
                position.z = position.z + Mathf.Sin(randomRadians) * randomRadius;
                var mapSystem = context.world.GetExistingSystem<MapSystem>();
                float y = mapSystem.GetGroundInfo(position).point.y;
                position.y = y;
                context.blackBoard.SetData(AIBlackBoardKeys.TargetPosition, position);
            }
            return BTStatus.Success;
        }
    }
}