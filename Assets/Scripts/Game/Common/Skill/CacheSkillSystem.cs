using Framework;
using Unity.Entities;

namespace Game
{
    public class CacheSkillSystem : ComponentSystem
    {
        private SkillSystem skillSystem;
        protected override void OnCreate()
        {
            skillSystem = World.GetOrCreateSystem<SkillSystem>();
        }
        public bool CastSkill(Entity entity, int skillId)
        {
            var cacheSkillComponent = World.GetComponent<CacheSkillComponent>(entity);
            if (cacheSkillComponent == null) return false;
            //需要当前有释放技能ID才能缓存技能
            if (cacheSkillComponent.cacheCurSkillId <= 0) return false;
            if (!skillSystem.EnableCacheInputSkill(entity)) return false;
            if (cacheSkillComponent.cachedNextSkillId == skillId) return true;
            cacheSkillComponent.cachedNextSkillId = skillId;
            CLog.LogArgs("CacheSkill", skillId);
            return true;
        }

        public void ClearCacheSkill(CacheSkillComponent cacheSkillComponent)
        {
            cacheSkillComponent.cacheCurSkillId = 0;
            cacheSkillComponent.cachedNextSkillId = 0;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, CacheSkillComponent cacheSkillComponent, SkillComponent skillComponent) =>
                {
                    var skillData = skillSystem.GetRunningSkill(skillComponent);
                    if (skillData != null)
                    {
                        if (skillData.skillId != cacheSkillComponent.cacheCurSkillId)
                        {
                            ClearCacheSkill(cacheSkillComponent);
                        }
                        else
                        {
                            if (cacheSkillComponent.cachedNextSkillId > 0)
                            {
                                if (skillSystem.CanCastSkill(skillComponent, cacheSkillComponent.cachedNextSkillId))
                                {
                                    skillSystem.CastSkill(entity, cacheSkillComponent.cachedNextSkillId);
                                    ClearCacheSkill(cacheSkillComponent);
                                }
                            }
                        }
                        cacheSkillComponent.cacheCurSkillId = skillData.skillId;
                    }
                    else
                    {
                        if (cacheSkillComponent.cachedNextSkillId > 0)
                        {
                            if (skillSystem.CanCastSkill(skillComponent, cacheSkillComponent.cachedNextSkillId))
                            {
                                skillSystem.CastSkill(entity, cacheSkillComponent.cachedNextSkillId);
                            }
                            ClearCacheSkill(cacheSkillComponent);
                        }
                    }
                });
        }
    }
}