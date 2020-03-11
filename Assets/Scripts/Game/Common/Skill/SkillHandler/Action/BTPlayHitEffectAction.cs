using System.Collections.Generic;
using Framework;

namespace Game
{
    public class BTPlayHitEffectActionData
    {
        public string effectName;
    }
    public class BTPlayHitEffectAction : BTAction<SkillBTContext, BTPlayHitEffectActionData>
    {
        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTPlayHitEffectActionData data)
        {
            var lst = context.blackBoard.GetData<List<EntityHitInfo>>(SkillBlackBoardKeys.ListHitInfo);
            if (lst == null) return BTStatus.Fail;
            for (int i = 0; i < lst.Count; i++)
            {
                var hitInfo = lst[i];
                var go = SceneEffectPool.Instance.CreateEffect(data.effectName, true, null);
                if (go != null)
                {
                    go.transform.position = hitInfo.point;
                    go.transform.forward = hitInfo.normal;
                }
            }
            return BTStatus.Success;
        }
    }
}