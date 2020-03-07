using Framework;
using UnityEngine.UI;

namespace Game
{
    public class BTForbidActionData
    {
        public ForbidType[] forbidArray;
    }
    public class BTForbidAction : BTAction<SkillBTContext, BTForbidActionData>
    {
        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTForbidActionData data)
        {
            if (data.forbidArray != null)
            {
                for (int i = 0; i < data.forbidArray.Length; i++)
                {
                    context.skillData.forbidance.Forbid(data.forbidArray[i]);
                }
            }
            return BTStatus.Success;
        }
    }
}