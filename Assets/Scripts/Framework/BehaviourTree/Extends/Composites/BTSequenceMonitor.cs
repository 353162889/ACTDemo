namespace Framework
{
    public class BTSequenceMonitorData { }
    public class BTSequenceMonitor : BTComposite<IBTContext, BTSequenceMonitorData>
    {
        protected override BTStatus Handler(IBTContext context, BTData btData, BTSequenceMonitorData data)
        {
            var selectIndex = 0;
            int totalCount = btData.children == null ? 0 : btData.children.Count;
            BTStatus result = BTStatus.Success;
            while (selectIndex < totalCount)
            {
                var childBTData = btData.children[selectIndex];
                var childHandler = context.GetHandler(childBTData.keyIndex);
                var childResult = childHandler.Handler(context, childBTData);
                if (childResult == BTStatus.Fail)
                {
                    result = BTStatus.Fail;
                    break;
                }
                else if (childResult == BTStatus.Running)
                {
                    result = BTStatus.Running;
                    break;
                }
                else if (childResult == BTStatus.Success)
                {
                    selectIndex++;
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