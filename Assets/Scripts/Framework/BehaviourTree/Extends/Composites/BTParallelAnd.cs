using System.Threading;
using NodeEditor;

namespace Framework
{
    public class BTParallelAndData { }
    /// <summary>
    /// 并行与，需要所有子对象都运行完成才返回
    /// </summary>
    public class BTParallelAnd : BTComposite<IBTContext, BTParallelAndData>
    {
        internal protected struct ParallelAndData
        {
            public int count;
            public BTStatusArray stateArray;
        } 

        private static ParallelAndData DefaultParallelAndData = new ParallelAndData(){count = 0, stateArray = BTStatusArray.DefaultRunning};

        protected override BTStatus Handler(IBTContext context, BTData btData, BTParallelAndData data)
        {
            var cacheData = context.executeCache.GetCache<ParallelAndData>(btData.dataIndex, DefaultParallelAndData);
            int totalCount = btData.children == null ? 0 : btData.children.Count;
            for (int i = 0; i < totalCount; i++)
            {
                var childBTData = btData.children[i];
                var childHandler = context.GetHandler(childBTData.keyIndex);
                BTStatus childResult = childHandler.Handler(context, childBTData);
                if (childResult != BTStatus.Running)
                {
                    cacheData.stateArray.SetState(i,childResult);
                    cacheData.count = cacheData.count + 1;
                }
            }
            context.executeCache.SetCache(btData.dataIndex, cacheData);
            if (cacheData.count == totalCount)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    if (cacheData.stateArray.GetState(i) == BTStatus.Success)
                    {
                        this.Clear(context, btData);
                        return BTStatus.Success;
                    }
                }
                this.Clear(context, btData);
                return BTStatus.Fail;
            }

            return BTStatus.Running;
        }

    }
}