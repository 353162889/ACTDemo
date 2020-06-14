using System;
using Framework;
using GameData;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class SkillSystem : ComponentSystem, IBTExecutor
    {
        private ForbidSystem forbidSystem;
        private InAirSystem inAirSystem;
        private SkillBTContext skillContext;
        public event Action<Entity, SkillData> OnSkillStart;
        public event Action<Entity, SkillData, bool> OnSkillEnd; 

        protected override void OnCreate()
        {
            ObjectPool<SkillData>.Instance.Init(2);
            skillContext = new SkillBTContext();
            forbidSystem = World.GetOrCreateSystem<ForbidSystem>();
            inAirSystem = World.GetOrCreateSystem<InAirSystem>();
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

        public int GetReplaceSkill(Entity entity, int skillId)
        {
            var resSkill = ResCfgSys.Instance.GetCfg<ResSkill>(skillId);
            if (resSkill == null) return skillId;
            if (inAirSystem.IsInAir(entity))
            {
                if (resSkill.inAirReplace > 0) return resSkill.inAirReplace;
            }
            else
            {
                if (resSkill.inGroundReplace > 0) return resSkill.inGroundReplace;
            }

            return skillId;
        }

        public bool CastSkill(Entity entity, int skillId)
        {
            var skillComponent = World.GetComponent<SkillComponent>(entity);
            if (null == skillComponent) return false;
            SkillTargetInfo targetInfo;
            if (!CanCastSkill(skillComponent, skillId, out targetInfo)) return false;
            UpdateSkillBreak(entity);
            var skilldata = CreateSkillData(entity, skillId, targetInfo);
            skillComponent.skillData = skilldata;
            CLog.LogArgs("CastSkill", skillId);
            return true;
        }

        public bool CanCastSkill(Entity entity, int skillId)
        {
            var skillComponent = World.GetComponent<SkillComponent>(entity);
            if (null == skillComponent) return false;
            return CanCastSkill(skillComponent, skillId);
        }

        public bool CanCastSkill(SkillComponent skillComponent, int skillId)
        {
            SkillTargetInfo targetInfo;
            return CanCastSkill(skillComponent, skillId, out targetInfo);
        }

        private bool CanCastSkill(SkillComponent skillComponent, int skillId, out SkillTargetInfo targetInfo)
        {
            targetInfo = default(SkillTargetInfo);
            if (forbidSystem.IsForbid(skillComponent.componentEntity, ForbidType.Ability)) return false;
            if (IsCastingSkill(skillComponent) && skillComponent.skillData.phase != SkillPhaseType.Backswing) return false;
            if (!CheckSkillCasterState(skillComponent, skillId)) return false;
            if (!CheckSkillCD(skillComponent, skillId)) return false;
            if (!SkillTargetSelector.GetSkillTargetInfo(World, skillComponent, skillId, out targetInfo))
            {
                return false;
            }
            return true;
        }

        private bool CheckSkillCasterState(SkillComponent skillComponent, int skillId)
        {
            var resSkill = ResCfgSys.Instance.GetCfg<ResSkill>(skillId);
            if (inAirSystem.IsInAir(skillComponent.componentEntity))
            {
                return (resSkill.casterState & (byte)SkillCasterStateType.Air) != 0;
            }
            else
            {
                return (resSkill.casterState & (byte)SkillCasterStateType.Ground) != 0;
            }
        }

        private bool CheckSkillCD(SkillComponent skillComponent, int skillId)
        {
            float canCastTime;
            if (skillComponent.dicCdInfo.TryGetValue(skillId, out canCastTime))
            {
                return Time.time >= canCastTime;
            }

            return true;
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

            var btTreeData = SkillManager.Instance.GetSkillBTTreeData(skillComponent.skillData.skillId);
            if (btTreeData != null)
            {
                skillContext.Reset();
                skillContext.Init(World, skillComponent, skillComponent.skillData, btTreeData, this, 0);
                Clear(skillContext);
            }
           
            var skillData = skillComponent.skillData;
            forbidSystem.RemoveForbiddance(entity, skillData.forbidance);
            skillData.forbidance = null;
            OnSkillEnd?.Invoke(entity, skillData, isBreak);
            skillComponent.skillData = null;
            if (skillData != null)
            {
                ObjectPool<SkillData>.Instance.SaveObject(skillData);
            }
        }

        private SkillData CreateSkillData(Entity entity, int skillId, SkillTargetInfo targetInfo)
        {
            var skillData = ObjectPool<SkillData>.Instance.GetObject();
            skillData.skillId = skillId;
            skillData.skillTime = -1;
            skillData.skillTimeScale = 1;
            skillData.phase = SkillPhaseType.Normal;
            skillData.forbidance = forbidSystem.AddForbiddance(entity, "skill:"+skillId);
            skillData.targetInfo = targetInfo;
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
                        var resSkill = ResCfgSys.Instance.GetCfg<ResSkill>(skillData.skillId);
                        float canCastTime = Time.time + resSkill.cd;
                        //技能进入cd
                        if (!skillComponent.dicCdInfo.ContainsKey(skillData.skillId))
                        {
                            skillComponent.dicCdInfo.Add(skillData.skillId, canCastTime);
                        }
                        else
                        {
                            skillComponent.dicCdInfo[skillData.skillId] = canCastTime;
                        }
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

            if (treeData != null && treeData.rootData != null)
            {
                BTDataHandlerInitialize.GetHandler(treeData.rootData.keyIndex).Clear(context, treeData.rootData);
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