using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class SkillSystem : ComponentSystem, IBTExecutor
    {
        private ForbidSystem forbidSystem;
        private SkillBTContext skillContext;

        protected override void OnCreate()
        {
            ObjectPool<SkillData>.Instance.Init(2);
            skillContext = new SkillBTContext();
            forbidSystem = World.GetOrCreateSystem<ForbidSystem>();
        }

        public void CastSkill(Entity entity, int skillId)
        {
            var skillComponent = World.GetComponent<SkillComponent>(entity);
            if (null == skillComponent) return;
            if (!CanCastSkill(skillComponent, skillId)) return;
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
            if (forbidSystem.IsForbid(skillComponent.entity, ForbidType.Ability)) return false;
            return skillComponent.skillData == null;
        }

        public void CancelSkill(Entity entity, bool isBreak)
        {
            var skillComponent = World.GetComponent<SkillComponent>(entity);
            if (null == skillComponent) return;
            if (skillComponent.skillData == null) return;
            CLog.LogArgs("CancelSkill","skillId",skillComponent.skillData.skillId, "isBreak", isBreak);
            var skillData = skillComponent.skillData;
            forbidSystem.RemoveForbiddance(entity, skillData.forbidance);
            skillComponent.skillData = null;
            if (skillData != null)
            {
                ObjectPool<SkillData>.Instance.SaveObject(skillData);
            }
        }

        private SkillData CreateSkillData(Entity entity, int skillId)
        {
            var skillData = ObjectPool<SkillData>.Instance.GetObject();
            skillData.skillId = skillId;
            skillData.skillTime = -1;
            skillData.skillTimeScale = 1;
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
                        //开始运行第一帧
                        skillData.skillTime = 0;
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
                    skillContext.Init(World, skillComponent,skillComponent.skillData, btTreeData, this, deltaTime);
                    if (firstRun)
                    {
                        Clear(skillContext);
                    }
                    BTStatus btState = Execute(skillContext);
                    skillContext.Reset();
                    if (btState != BTStatus.Running)
                    {
                        CancelSkill(entity, btState == BTStatus.Fail);
                        return;
                    }

                    skillData.skillTime += deltaTime;

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
            if (treeData.rootData.children.Count > 0)
            {
                var childBtData = treeData.rootData.children[0];
                BTDataHandlerInitialize.GetHandler(childBtData.keyIndex).Clear(context, childBtData);
            }
        }
    }
}