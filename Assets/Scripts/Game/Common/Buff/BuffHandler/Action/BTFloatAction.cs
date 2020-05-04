using System.Runtime.Remoting.Contexts;
using Framework;
using UnityEngine;

namespace Game
{
    public class BTFloatActionData
    {
        public BuffDirectionType directionType;
        public float height;
        public float horizontalSpeed;
        public float endGroundHeight;
    }
    public class BTFloatAction : BTAction<BuffBTContext, BTFloatActionData>
    {
        protected override BTStatus Handler(BuffBTContext context, BTData btData, BTFloatActionData data)
        {
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                Vector3 horizontalVelocity = BuffHandlerUtility.GetBuffDirection(context, data.directionType);
                horizontalVelocity.Normalize();
                horizontalVelocity *= data.horizontalSpeed;
                var inAirSystem = context.world.GetExistingSystem<InAirSystem>();
                inAirSystem.StartMoveToAir(context.buffComponent.componentEntity, horizontalVelocity, data.height);
            }

            var floatCompoent = context.world.GetComponent<InAirComponent>(context.buffComponent.componentEntity);
            if (floatCompoent == null || !floatCompoent.isInAir)
            {
                return BTStatus.Success;
            }

            var stepMoveComponent =
                context.world.GetComponent<StepMoveComponent>(context.buffComponent.componentEntity);
            var transformComponent = context.world.GetComponent<TransformComponent>(context.buffComponent.componentEntity);
            var groundComponent = context.world.GetComponent<GroundComponent>(context.buffComponent.componentEntity);
            if (stepMoveComponent.velocity.y < 0 && transformComponent.position.y - groundComponent.groundPointInfo.point.y < data.endGroundHeight)
            {
                return BTStatus.Success;
            }
            return BTStatus.Running;
        }
    }
}