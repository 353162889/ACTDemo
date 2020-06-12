using System.Collections.Generic;
using Framework;

namespace Game
{
    public class AIExistCanCastSkillIdConditionData
    {

    }
    public class AIExistCanCastSkillIdCondition : BTCondition<AIBTContext, AIExistCanCastSkillIdConditionData>
    {
        protected override bool Evaluate(AIBTContext context, BTData btData, AIExistCanCastSkillIdConditionData data)
        {
            var skillComponent = context.world.GetComponent<SkillComponent>(context.aiComponent.componentEntity);
            if (skillComponent == null) return false;
            List<int> lstSkillId = context.blackBoard.GetAndSetData(AIBlackBoardKeys.SkillIDList, ObjectFactory<List<int>>.Instance);
            var skillSystem = context.world.GetExistingSystem<SkillSystem>();
            for (int i = 0; i < lstSkillId.Count; i++)
            {
                if (skillSystem.CanCastSkill(skillComponent, lstSkillId[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}