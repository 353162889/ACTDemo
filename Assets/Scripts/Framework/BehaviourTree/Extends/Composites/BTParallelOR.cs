namespace Framework
{
    public class BTParallelORData { }
    public class BTParallelOR : BTComposite<IBTContext, BTParallelORData>
    {
        protected override BTStatus Handler(IBTContext context, BTData btData, BTParallelORData data)
        {
            int totalCount = btData.children == null ? 0 : btData.children.Count;
            for (int i = 0; i < totalCount; i++)
            {
                var childBTData = btData.children[i];
                var childHandler = context.GetHandler(childBTData.keyIndex);
                BTStatus childResult = childHandler.Handler(context, childBTData);
                if (childResult != BTStatus.Running)
                {
                    this.Clear(context, btData);
                    return childResult;
                }
            }

            return BTStatus.Running;
        }

    }
}