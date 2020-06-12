using System.Collections.Generic;
using Framework;

namespace Game
{
    public class AICastSkillActionData
    {

    }
    public class AICastSkillAction : BTAction<AIBTContext, AICastSkillActionData>
    {
        internal protected struct InnerAICastSkillAction
        {
            public int castSkillId;
        }
        private static InnerAICastSkillAction DefaultActionData = new InnerAICastSkillAction() { castSkillId = 0 };
        protected override BTStatus Handler(AIBTContext context, BTData btData, AICastSkillActionData data)
        {
            var cacheData =
                context.executeCache.GetCache<InnerAICastSkillAction>(btData.dataIndex, DefaultActionData);
            var status = context.executeCache.GetExecuteStatus(btData.dataIndex);
            var skillSystem = context.world.GetExistingSystem<SkillSystem>();
            if (status == BTExecuteStatus.Ready)
            {
                var lstSkillId = context.blackBoard.GetAndSetData(AIBlackBoardKeys.SkillIDList, ObjectFactory<List<int>>.Instance);
                for (int i = 0; i < lstSkillId.Count; i++)
                {
                    if (skillSystem.CastSkill(context.aiComponent.componentEntity, lstSkillId[i]))
                    {
                        cacheData.castSkillId = lstSkillId[i];
                        context.executeCache.SetCache(btData.dataIndex, cacheData);
                        break;
                    }
                }
            }

            if (cacheData.castSkillId <= 0) return BTStatus.Fail;
            var skillData = skillSystem.GetRunningSkill(context.aiComponent.componentEntity);
            if (skillData != null && skillData.skillId == cacheData.castSkillId) return BTStatus.Running;
            return BTStatus.Success;
        }
    }
}