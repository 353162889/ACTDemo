using System.Collections.Generic;
using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class BTPlayHitEffectActionData
    {
        public bool followTarget;
        public string effectName;
    }
    public class BTPlayHitEffectAction : BTAction<SkillBTContext, BTPlayHitEffectActionData>
    {
        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTPlayHitEffectActionData data)
        {
            var lst = context.blackBoard.GetData<List<DamageInfo>>(SkillBlackBoardKeys.ListDamageInfo);
            if (lst == null) return BTStatus.Fail;
            for (int i = 0; i < lst.Count; i++)
            {
                var damageInfo = lst[i];
                Transform parent = null;
                if (data.followTarget)
                {
                    if (damageInfo.target != Entity.Null)
                    {
                        var gameObjectComponent = context.world.GetComponent<GameObjectComponent>(damageInfo.target);
                        parent = gameObjectComponent.transform;
                    }
                }
                var go = SceneEffectPool.Instance.CreateEffect(data.effectName, true, parent);
                if (go != null)
                {
                    go.transform.position = damageInfo.hitInfo.point;
                    go.transform.forward = damageInfo.hitInfo.normal;
                }
            }
            return BTStatus.Success;
        }
    }
}