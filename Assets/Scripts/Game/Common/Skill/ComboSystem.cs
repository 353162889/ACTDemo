using Framework;
using GameData;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class ComboSystem : ComponentSystem
    {
        private SkillSystem skillSystem;
        protected override void OnCreate()
        {
            skillSystem = World.GetOrCreateSystem<SkillSystem>();
        }

        public bool CasterSkill(Entity entity, int skillId)
        {
            var comboComponent = World.GetComponent<ComboComponent>(entity);
            if (comboComponent == null) return false;
            //如果当前没进入combo，返回false（进入combo默认是释放技能触发的）
            if (comboComponent.comboId <= 0) return false;
            var skillCfg = ResCfgSys.Instance.GetCfg<ResSkill>(skillId);
            //当前释放的技能必须是该对应的combo
            if (skillCfg == null || skillCfg.combo != comboComponent.comboId) return false;
            //如果已经输入下一次combo了，返回true，表示已经处理了改缓存
            if (comboComponent.inputNextCombo) return true;
            var comboCfg = ResCfgSys.Instance.GetCfg<ResCombo>(comboComponent.comboId);
            //如果没有下一个combo，返回false
            if (comboComponent.comboIndex >= comboCfg.lstSkillId.Count - 1) return false;
            //如果当前技能不允许存储技能按键
            if (!skillSystem.EnableCacheInputSkill(entity)) return false;
            comboComponent.inputNextCombo = true;
            CLog.LogArgs("InputCombo", comboComponent.comboId, skillId);
            return true;
        }

        private bool StartCombo(ComboComponent comboComponent, int comboId)
        {
            var comboCfg = ResCfgSys.Instance.GetCfg<ResCombo>(comboId);
            if (comboCfg.lstSkillId.Count > 0)
            {
                ClearCombo(comboComponent);
                comboComponent.comboId = comboId;
                comboComponent.comboIndex = 0;
                return true;
            }

            return false;
        }

        public void ClearCombo(ComboComponent comboComponent)
        {
            comboComponent.comboId = 0;
            comboComponent.comboIndex = 0;
            comboComponent.inputNextCombo = false;
            comboComponent.clearComboTime = 0;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ComboComponent comboComponent, SkillComponent skillComponent) =>
            {
                if (comboComponent.comboId > 0)
                {
                    var comboCfg = ResCfgSys.Instance.GetCfg<ResCombo>(comboComponent.comboId);
                    var skillId = comboCfg.lstSkillId[comboComponent.comboIndex];
                    var skillData = skillSystem.GetRunningSkill(skillComponent);
                    if (skillData != null && skillData.skillId != skillId)
                    {
                        ClearCombo(comboComponent);
                    }
                    else if (comboComponent.inputNextCombo)
                    {
                        var nextSkillId = comboCfg.lstSkillId[comboComponent.comboIndex + 1];
                        if (skillSystem.CanCastSkill(entity, nextSkillId))
                        {
                            skillSystem.CastSkill(entity, nextSkillId);
                            comboComponent.comboIndex++;
                            comboComponent.inputNextCombo = false;
                            comboComponent.clearComboTime = 0;
                            if (comboComponent.comboIndex >= comboCfg.lstSkillId.Count - 1)
                            {
                                ClearCombo(comboComponent);
                            }
                        }
                    }
                    else
                    {
                        if (comboComponent.clearComboTime <= 0 && !skillSystem.IsCastingSkill(skillComponent))
                        {
                            comboComponent.clearComboTime = Time.time + comboComponent.comboSpaceTime;
                        }

                        if (comboComponent.clearComboTime > 0)
                        {
                            if (Time.time >= comboComponent.clearComboTime)
                            {
                                ClearCombo(comboComponent);
                            }
                        }
                    }
                }
                else
                {
                    if (skillSystem.IsCastingSkill(skillComponent))
                    {
                        var skillData = skillSystem.GetRunningSkill(skillComponent);
                        var skillCfg = ResCfgSys.Instance.GetCfg<ResSkill>(skillData.skillId);
                        if (skillCfg != null)
                        {
                            if (skillCfg.combo > 0)
                            {
                                StartCombo(comboComponent, skillCfg.combo);
                            }
                        }
                        
                    }
                }
            });
        }
    }
}