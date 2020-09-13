using BTCore;
using Framework;
using Unity.Entities;

namespace Game
{
    public class AIExistTargetConditionData
    {

    }
    public class AIExistTargetCondition : BTCondition<AIBTContext, AIExistTargetConditionData>
    {
        protected override bool Evaluate(AIBTContext context, BTData btData, AIExistTargetConditionData data)
        {
            return context.worldState.target != Entity.Null;
        }
    }
}