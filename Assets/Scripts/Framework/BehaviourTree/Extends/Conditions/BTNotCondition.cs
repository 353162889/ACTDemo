namespace Framework
{
    public class BTNotConditionData { }
    public class BTNotCondition : BTCondition<IBTContext, BTNotConditionData>
    {
        protected override bool Evaluate(IBTContext context, BTData btData, BTNotConditionData data)
        {
            int totalCount = btData.children == null ? 0 : btData.children.Count;
            if (totalCount > 1)
            {
                CLog.LogError("BTNotCondition 只允许一个子节点，当前子节点数量:" + totalCount + ",BTNotCondition停止运行（默认返回false）");
                return false;
            }

            if (totalCount == 1)
            {
                var childBTData = btData.children[0];
                var childHandler = context.GetHandler(childBTData.keyIndex);
                BTStatus childResult = childHandler.Handler(context, childBTData);
                if (childResult == BTStatus.Success)
                {
                    return false;
                }
                else if (childResult == BTStatus.Fail)
                {
                    return true;
                }
                else
                {
                    CLog.LogError("BTNotCondition 子节点不为BTCondition,BTNotCondition停止运行(默认返回false)");
                    return false;
                }
            }

            return true;
        }
    }
}