using Framework;
using UnityEngine;

namespace Game
{
    public class BTAirSpeedAdjustActionData
    {
        public float horizontalX;
        public float horizontalZ;
        public float height;

    }
    public class BTAirSpeedAdjustAction : BTAction<SkillBTContext, BTAirSpeedAdjustActionData>
    {
        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTAirSpeedAdjustActionData data)
        {
            var inAirSystem = context.world.GetExistingSystem<InAirSystem>();
            if (inAirSystem != null && inAirSystem.IsInAir(context.skillComponent.componentEntity))
            {
                float groundHeight = 0f;
                var inAirComponent = context.world.GetComponent<InAirComponent>(context.skillComponent.componentEntity);
                if (inAirComponent != null)
                {
                    groundHeight = inAirComponent.endGroundHeight;
                }
                inAirSystem.StopMove(context.skillComponent.componentEntity);
                inAirSystem.StartMoveToAir(context.skillComponent.componentEntity, new Vector3(data.horizontalX, 0, data.horizontalZ),data.height, groundHeight);
            }
            return BTStatus.Success;
        }
    }
}