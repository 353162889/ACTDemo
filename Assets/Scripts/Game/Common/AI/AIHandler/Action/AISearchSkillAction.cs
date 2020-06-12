using System.Collections.Generic;
using Framework;
using GameData;

namespace Game
{
    public class AISearchSkillActionData
    {

    }
    public class AISearchSkillAction : BTAction<AIBTContext, AISearchSkillActionData>
    {
        protected override BTStatus Handler(AIBTContext context, BTData btData, AISearchSkillActionData data)
        {
            List<int> lstSkillId = context.blackBoard.GetAndSetData(AIBlackBoardKeys.SkillIDList, ObjectFactory<List<int>>.Instance);
            lstSkillId.Clear();
            var skillComponent = context.world.GetComponent<SkillComponent>(context.aiComponent.componentEntity);
            if (skillComponent == null) return BTStatus.Success;
            var skillSystem = context.world.GetExistingSystem<SkillSystem>();
            
            var commonInfo = context.world.GetComponent<EntityCommonInfoComponent>(context.aiComponent.componentEntity);
            if (commonInfo.entityType == EntityType.Monster)
            {
                var hostTransformComponent = context.world.GetComponent<TransformComponent>(context.aiComponent.componentEntity);
                var targetTransformComponent = context.world.GetComponent<TransformComponent>(context.aiBlackBoard.target);
                float sqrDis = (hostTransformComponent.position - targetTransformComponent.position).sqrMagnitude;
                var monsterCfg = ResCfgSys.Instance.GetCfg<ResMonster>(commonInfo.cfgId);
                for (int i = 0; i < monsterCfg.skillIds.Count; i++)
                {
                    if (sqrDis < 3 * 3 && skillSystem.CanCastSkill(skillComponent, monsterCfg.skillIds[i]))
                    {
                        lstSkillId.Add(monsterCfg.skillIds[i]);
                    }
                }
            }
            return BTStatus.Success;
        }
    }
}