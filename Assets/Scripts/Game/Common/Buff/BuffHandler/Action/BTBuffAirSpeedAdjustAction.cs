using Framework;
using UnityEngine;

namespace Game
{
    public class BTBuffAirSpeedAdjustActionData
    {
        public float horizontalX;
        public float horizontalZ;
        public float height;

    }
    public class BTBuffAirSpeedAdjustAction : BTAction<BuffBTContext, BTBuffAirSpeedAdjustActionData>
    {
        protected override BTStatus Handler(BuffBTContext context, BTData btData, BTBuffAirSpeedAdjustActionData data)
        {
            var inAirSystem = context.world.GetExistingSystem<InAirSystem>();
            if (inAirSystem != null && inAirSystem.IsInAir(context.buffComponent.componentEntity))
            {
                float groundHeight = 0f;
                var inAirComponent = context.world.GetComponent<InAirComponent>(context.buffComponent.componentEntity);
                if (inAirComponent != null)
                {
                    groundHeight = inAirComponent.endGroundHeight;
                }
                inAirSystem.StopMove(context.buffComponent.componentEntity);
                inAirSystem.StartMoveToAir(context.buffComponent.componentEntity, new Vector3(data.horizontalX, 0, data.horizontalZ), data.height, groundHeight);
            }
            return BTStatus.Success;
        }
    }
}