using Cinemachine.Utility;
using Framework;
using GameData;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public static class SkillTargetSelectorType
    {
        public static int DefaultSelectorType = 1;
        public static int DesiredVelocityTargetSelectorType = 2;
    }

    public static class SkillTargetSelector
    {
        public static bool GetSkillTargetInfo(World world, SkillComponent skillComponent, int skillId, out SkillTargetInfo targetInfo)
        {
            targetInfo = new SkillTargetInfo();
            var cfg = ResCfgSys.Instance.GetCfg<ResSkill>(skillId);
            if (cfg == null) return false;
            int selectorType = cfg.targetSelector;
            var transformComponent = world.GetComponent<TransformComponent>(skillComponent.componentEntity);
            if (selectorType == SkillTargetSelectorType.DefaultSelectorType)
            {
                targetInfo.target = skillComponent.componentEntity;
                targetInfo.targetDirection = transformComponent.forward;
                targetInfo.targetPosition = transformComponent.position;
                return true;
            }
            else if (selectorType == SkillTargetSelectorType.DesiredVelocityTargetSelectorType)
            {
                var stepMoveComponent = world.GetComponent<StepMoveComponent>(skillComponent.componentEntity);
                if (stepMoveComponent == null) return false;
                targetInfo.target = Entity.Null;
                var desiredVeloctiy = stepMoveComponent.desiredVelocity;
                desiredVeloctiy.y = 0;
                if (desiredVeloctiy != Vector3.zero)
                {
                    desiredVeloctiy.Normalize();
                }
                else
                {
                    desiredVeloctiy = transformComponent.forward;
                }

                targetInfo.targetDirection = desiredVeloctiy;
                targetInfo.targetPosition = transformComponent.position;
                return true;
            }
            return false;
        }
    }
}