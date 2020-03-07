using System.Diagnostics;
using UnityEngine.UI;

namespace Framework
{
    public abstract class BTDecorator<T1, T2> : BTDataHandler<T1, T2> where T1 : IBTContext
    {
        sealed protected override BTStatus Handler(T1 context, BTData btData, T2 data)
        {
            int totalCount = btData.children == null ? 0 : btData.children.Count;
            if (totalCount > 1)
            {
                CLog.LogError("BTDecorator 只允许一个子节点，当前子节点数量:"+totalCount + ",BTDecorator停止运行（默认返回fail）");
                return BTStatus.Fail;
            }

            if (totalCount == 1)
            {
                var childBTData = btData.children[0];
                return OnHandler(context, btData, data, childBTData);
            }
            return BTStatus.Success;
        }

        protected virtual BTStatus OnHandler(T1 context, BTData btData, T2 data, BTData childBTData)
        {
            var childHandler = context.GetHandler(childBTData.keyIndex);
            BTStatus childResult = childHandler.Handler(context, childBTData);
            return Decorate(context, btData, data, childBTData, childResult);
        }

        protected virtual BTStatus Decorate(T1 context, BTData btData, T2 data, BTData childBTData, BTStatus childResult)
        {
            return childResult;
        }
    }
}