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
        public static int DirectionMoveInputDirectionSelectorType = 2;
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
            else if (selectorType == SkillTargetSelectorType.DirectionMoveInputDirectionSelectorType)
            {
                var directionMoveComponent = world.GetComponent<DirectionMoveComponent>(skillComponent.componentEntity);
                if (directionMoveComponent == null) return false;
                targetInfo.target = Entity.Null;
                var inputDirection = directionMoveComponent.inputDirection;
                inputDirection.y = 0;
                if (inputDirection != Vector3.zero)
                {
                    inputDirection.Normalize();
                }
                else
                {
                    inputDirection = transformComponent.forward;
                }

                targetInfo.targetDirection = inputDirection;
                targetInfo.targetPosition = transformComponent.position;
                return true;
            }
            return false;
        }
    }
}