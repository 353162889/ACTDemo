using System;
using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class SkillSystem : ComponentSystem, IBTExecutor
    {
        private ForbidSystem forbidSystem;
        private SkillBTContext skillContext;
        public event Action<Entity, SkillData> OnSkillStart;
        public event Action<Entity, SkillData, bool> OnSkillEnd; 

        protected override void OnCreate()
        {
            ObjectPool<SkillData>.Instance.Init(2);
            skillContext = new SkillBTContext();
            forbidSystem = World.GetOrCreateSystem<ForbidSystem>();
        }

        private void UpdateSkillBreak(Entity entity)
        {
            var skillComponent = World.GetComponent<SkillComponent>(entity);
            if (skillComponent != null)
            {
                if (IsCastingSkill(skillComponent))
                {
                    if (skillComponent.skillData.phase == SkillPhaseType.Backswing)
                    {
                        CancelSkill(entity, true);
                    }
                }
            }
        }

        public void OnDirectionMove(Entity entity)
        {
            UpdateSkillBreak(entity);
        }

        public void OnJump(Entity entity)
        {
            UpdateSkillBreak(entity);
        }

        public void CastSkill(Entity entity, int skillId)
        {
            var skillComponent = World.GetComponent<SkillComponent>(entity);
            if (null == skillComponent) return;
            if (!CanCastSkill(skillComponent, skillId)) return;
            UpdateSkillBreak(entity);
            var skilldata = CreateSkillData(entity, skillId);
            skillComponent.skillData = skilldata;
            CLog.LogArgs("CastSkill", skillId);
        }

        public bool CanCastSkill(Entity entity, int skillId)
        {
            var skillComponent = World.GetComponent<SkillComponent>(entity);
            if (null == skillComponent) return false;
            return CanCastSkill(skillComponent, skillId);
        }

        public bool CanCastSkill(SkillComponent skillComponent, int skillId)
        {
            if (forbidSystem.IsForbid(skillComponent.componentEntity, ForbidType.Ability)) return false;
            return !IsCastingSkill(skillComponent) || skillComponent.skillData.phase == SkillPhaseType.Backswing;
        }

        public bool IsCastingSkill(SkillComponent skillComponent)
        {
            return skillComponent.skillData != null;
        }

        public SkillData GetRunningSkill(Entity entity)
        {
            var skillComponent = World.GetComponent<SkillComponent>(entity);
            if (null == skillComponent) return null;
            return GetRunningSkill(skillComponent);
        }

        public SkillData GetRunningSkill(SkillComponent skillComponent)
        {
            return skillComponent.skillData;
        }

        public bool EnableCacheInputSkill(Entity entity)
        {
            var skillComponent = World.GetComponent<SkillComponent>(entity);
            if (null == skillComponent) return false;
            if (!IsCastingSkill(skillComponent) || skillComponent.skillData.enableInputSkill ||
                skillComponent.skillData.phase == SkillPhaseType.Backswing)
            {
                return true;
            }

            return false;
        }

        public void CancelSkill(Entity entity, bool isBreak)
        {
            var skillComponent = World.GetComponent<SkillComponent>(entity);
            if (null == skillComponent) return;
            if (skillComponent.skillData == null) return;
            CLog.LogArgs("CancelSkill","skillId",skillComponent.skillData.skillId, "isBreak", isBreak);
            if (isBreak)
                skillComponent.skillData.isBreak = true;
            Clear(skillContext);
            var skillData = skillComponent.skillData;
            forbidSystem.RemoveForbiddance(entity, skillData.forbidance);
            skillData.forbidance = null;
            OnSkillEnd?.Invoke(entity, skillData, isBreak);
            skillComponent.skillData = null;
            if (skillData != null)
            {
                ObjectPool<SkillData>.Instance.SaveObject(skillData);
            }
            skillContext.Reset();
        }

        private SkillData CreateSkillData(Entity entity, int skillId)
        {
            var skillData = ObjectPool<SkillData>.Instance.GetObject();
            skillData.skillId = skillId;
            skillData.skillTime = -1;
            skillData.skillTimeScale = 1;
            skillData.phase = SkillPhaseType.Normal;
            skillData.forbidance = forbidSystem.AddForbiddance(entity, "ability:"+skillId);

            return skillData;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, SkillComponent skillComponent) =>
            {
                var skillData = skillComponent.skillData;
                if (skillData != null)
                {
                    bool firstRun = false;
                    if (skillData.skillTime < 0)
                    {
                        OnSkillStart?.Invoke(entity, skillData);
                        //开始运行第一帧
                        skillData.skillTime = 0;
                        skillData.phase = SkillPhaseType.Normal;
                        firstRun = true;
                    }
                    var btTreeData = SkillManager.Instance.GetSkillBTTreeData(skillData.skillId);
                    if (btTreeData == null)
                    {
                        //如果没有逻辑，直接结束运行
                        CancelSkill(entity, false);
                        return;
                    }
                    float deltaTime = Time.deltaTime * skillData.skillTimeScale;
                    skillContext.Reset();
                    skillContext.Init(World, skillComponent,skillComponent.skillData, btTreeData, this, deltaTime);
                    if (firstRun)
                    {
                        Clear(skillContext);
                    }
                    BTStatus btState = Execute(skillContext);
                    bool finish = skillComponent.skillData.isBreak || btState != BTStatus.Running;
                    if (finish)
                    {
                        if (!skillComponent.skillData.isBreak && btState == BTStatus.Fail)
                        {
                            skillComponent.skillData.isBreak = true;
                        }
                        CancelSkill(entity, skillComponent.skillData.isBreak);
                    }
                    else
                    {
                        skillData.skillTime += deltaTime;
                    }
                   
                }
            });
        }

        public BTStatus Execute(IBTContext context)
        {
            var treeData = context.treeData;
            if (treeData.rootData.children.Count > 0)
            {
                var childBtData = treeData.rootData.children[0];
                return BTDataHandlerInitialize.GetHandler(childBtData.keyIndex).Handler(context, childBtData);
            }
            return BTStatus.Success;
        }

        public void Clear(IBTContext context)
        {
            var treeData = context.treeData;
            if (treeData != null && treeData.rootData != null && treeData.rootData.children.Count > 0)
            {
                var childBtData = treeData.rootData.children[0];
                BTDataHandlerInitialize.GetHandler(childBtData.keyIndex).Clear(context, childBtData);
            }
        }

        protected override void OnDestroy()
        {
            OnSkillStart = null;
            OnSkillEnd = null;
            base.OnDestroy();
        }
    }
}