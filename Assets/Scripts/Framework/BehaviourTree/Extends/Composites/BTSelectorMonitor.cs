namespace Framework
{
    public class BTSelectorMonitorData { }
    public class BTSelectorMonitor : BTComposite<IBTContext, BTSelectorMonitorData>
    {
        protected override BTStatus Handler(IBTContext context, BTData btData, BTSelectorMonitorData data)
        {
            var selectIndex = 0;
            BTStatus result = BTStatus.Fail;
            int totalCount = btData.children == null ? 0 : btData.children.Count;
            while (selectIndex < totalCount)
            {
                var childBTData = btData.children[selectIndex];
                var childHandler = context.GetHandler(childBTData.keyIndex);
                var childResult = childHandler.Handler(context, childBTData);
                if (childResult == BTStatus.Success)
                {
                    result = BTStatus.Success;
                    break;
                }
                else if (childResult == BTStatus.Running)
                {
                    result = BTStatus.Running;
                    break;
                }
                else if (childResult == BTStatus.Fail)
                {
                    selectIndex++;
                    context.executeCache.SetCache(btData.dataIndex, selectIndex);
                }
            }

            for (int i = selectIndex + 1; i < totalCount; i++)
            {
                var childBTData = btData.children[i];
                var handler = context.GetHandler(childBTData.keyIndex);
                handler.Clear(context, childBTData);
            }
            return result;
        }

    }
}