using Framework;
using Unity.Entities;

namespace Game
{
    public class AITraceTargetActionData
    {

    }
    public class AITraceTargetAction : BTAction<AIBTContext, AITraceTargetActionData>
    {
        protected override BTStatus Handler(AIBTContext context, BTData btData, AITraceTargetActionData data)
        {
            Entity target = context.worldState.target;
            if (target == Entity.Null) return BTStatus.Fail;
            var traceComponent = context.world.GetComponent<TraceComponent>(context.aiComponent.componentEntity);
            if (traceComponent == null) return BTStatus.Fail;
            if (traceComponent.isTrace) return BTStatus.Running;
            var traceSystem = context.world.GetExistingSystem<TraceSystem>();
            if (traceSystem == null) return BTStatus.Fail;
            if (!traceComponent.isTrace || traceComponent.target != target)
            {
                traceSystem.StartTrace(context.aiComponent.componentEntity, target);
            }
            return BTStatus.Running;
        }

        protected override void Clear(AIBTContext context, BTData btData, AITraceTargetActionData data)
        {
            if (!IsCleared(context, btData))
            {
                var traceSystem = context.world.GetExistingSystem<TraceSystem>();
                if (traceSystem != null)
                {
                    traceSystem.StopTrace(context.aiComponent.componentEntity);
                }
            }

            base.Clear(context, btData, data);
        }
    }
}