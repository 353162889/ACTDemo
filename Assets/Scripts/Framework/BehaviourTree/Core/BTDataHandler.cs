using System;

namespace Framework
{
    //行为树数据处理类
    public interface IBTDataHandler
    {
        BTStatus Handler(IBTContext context, BTData btData);
        void Clear(IBTContext context, BTData btData);
    }

    public abstract class BTDataHandler<T1, T2> : IBTDataHandler where T1 : IBTContext
    {
        public BTStatus Handler(IBTContext context, BTData btData)
        {
            if (context.executeCache.GetExecuteStatus(btData.dataIndex) == BTExecuteStatus.Init)
            {
                context.executeCache.SetExecuteStatus(btData.dataIndex, BTExecuteStatus.Ready);
            }

            BTStatus result = Handler((T1)context, btData, (T2)btData.data);
            if (result == BTStatus.Running)
            {
                context.executeCache.SetExecuteStatus(btData.dataIndex, BTExecuteStatus.Running);
            }
            else
            {
                context.executeCache.SetExecuteStatus(btData.dataIndex, BTExecuteStatus.Finish);
                this.Clear(context, btData);
            }
            return result;
        }

        public void Clear(IBTContext context, BTData btData)
        {
            try
            {
                Clear((T1)context, btData, (T2)btData.data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
//            Clear((T1)context, btData,(T2)btData.data);
        }

        protected virtual BTStatus Handler(T1 context, BTData btData, T2 data)
        {
            return BTStatus.Success;
        }

        protected virtual void Clear(T1 context, BTData btData, T2 data)
        {
            int totalCount = btData.children == null ? 0 : btData.children.Count;
            for (int i = 0; i < totalCount; i++)
            {
                var childBTData = btData.children[i];
                var handler = context.GetHandler(childBTData.keyIndex);
                handler.Clear(context, childBTData);
            }
            context.executeCache.SetCache(btData.dataIndex,null);
            context.executeCache.SetExecuteStatus(btData.dataIndex, BTExecuteStatus.Init);
        }

        protected bool IsCleared(IBTContext context, BTData btData)
        {
            return context.executeCache.GetExecuteStatus(btData.dataIndex) == BTExecuteStatus.Init;
        }

    }
}