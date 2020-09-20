using Framework;
using GameData;
using Unity.Entities;

namespace Game
{
    public class AttackFactorSensor : GlobalSensor, IEventSensor
    {
        private SkillSystem skillSystem;

        public AttackFactorSensor(World world) : base(world)
        {
        }

        protected override void OnInit()
        {
            skillSystem = this.world.GetOrCreateSystem<SkillSystem>();
        }

        public void RegisterEvent()
        {
            skillSystem.OnSkillStart += SkillSystemOnOnSkillStart;
            skillSystem.OnSkillEnd += SkillSystemOnOnSkillEnd;
            skillSystem.OnSkillCDEnd += SkillSystemOnOnSkillCdEnd;
        }

        private void SkillSystemOnOnSkillStart(Entity entity, SkillData arg2)
        {
            UpdateEntity(entity);
        }

        private void SkillSystemOnOnSkillEnd(Entity entity, SkillData arg2, bool arg3)
        {
            
            UpdateEntity(entity);
        }

        private void SkillSystemOnOnSkillCdEnd(Entity entity, int arg2)
        {
            UpdateEntity(entity);
        }

        public void UnregisterEvent()
        {
            skillSystem.OnSkillStart -= SkillSystemOnOnSkillStart;
            skillSystem.OnSkillEnd -= SkillSystemOnOnSkillEnd;
            skillSystem.OnSkillCDEnd -= SkillSystemOnOnSkillCdEnd;
        }


        public override void UpdateEntity(Entity entity)
        {
            var aiComponent = world.GetComponent<AIComponent>(entity);
            if (aiComponent == null) return;
            var commonInfo = world.GetComponent<EntityCommonInfoComponent>(aiComponent.componentEntity);
            if (commonInfo.entityType == EntityType.Monster)
            {
                var monsterCfg = ResCfgSys.Instance.GetCfg<ResMonster>(commonInfo.cfgId);
                var skillComponent = world.GetComponent<SkillComponent>(entity);
                if (monsterCfg.skillIds.Count > skillComponent.dicCdInfo.Count)
                {
                    aiComponent.worldState.attackDesired = 1f;
                }
                else
                {
                    aiComponent.worldState.attackDesired = 0;
                }
            }
        }
    }
}