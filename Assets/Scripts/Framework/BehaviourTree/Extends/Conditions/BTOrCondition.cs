namespace Framework
{
    public class BTOrConditionData { }
    public class BTOrCondition : BTCondition<IBTContext, BTOrConditionData>
    {
        protected override bool Evaluate(IBTContext context, BTData btData, BTOrConditionData data)
        {
            int totalCount = btData.children == null ? 0 : btData.children.Count;
            for (int i = 0; i < totalCount; i++)
            {
                var childBTData = btData.children[i];
                var childHandler = context.GetHandler(childBTData.keyIndex);
                BTStatus childResult = childHandler.Handler(context, childBTData);
                if (childResult == BTStatus.Success)
                {
                    return true;
                }
                else if (childResult == BTStatus.Fail)
                {
                }
                else
                {
                    CLog.LogError("BTAndCondition 子节点不为BTCondition,BTAndCondition停止运行(默认返回false)");
                    return false;
                }
            }
            return false;
        }
    }
}