namespace Framework
{
    public class BTSequenceData { }
    public class BTSequence : BTComposite<IBTContext,BTSequenceData>
    {
        protected override BTStatus Handler(IBTContext context, BTData btData, BTSequenceData data)
        {
            var selectIndex = context.executeCache.GetCache<int>(btData.dataIndex, -1);
            if (selectIndex < 0) selectIndex = 0;
            int totalCount = btData.children == null ? 0 : btData.children.Count;
            while (selectIndex < totalCount)
            {
                var childBTData = btData.children[selectIndex];
                var childHandler = context.GetHandler(childBTData.keyIndex);
                var childResult = childHandler.Handler(context, childBTData);
                if (childResult == BTStatus.Fail)
                {
                    this.Clear(context, btData);
                    return BTStatus.Fail;
                }
                else if (childResult == BTStatus.Running)
                {
                    return BTStatus.Running;
                }
                else if(childResult == BTStatus.Success)
                {
                    selectIndex++;
                    context.executeCache.SetCache(btData.dataIndex, selectIndex);
                }
            }
            this.Clear(context, btData);
            return BTStatus.Success;
        }
    }
}