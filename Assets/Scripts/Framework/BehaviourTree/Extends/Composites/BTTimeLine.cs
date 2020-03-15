using System.Diagnostics;
using NodeEditor;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 时间轴上的数据
    /// </summary>
    public interface IBTTimeLineData
    {
        float time { get; set; }
    }
    /// <summary>
    /// 时间轴有运行期间时间的数据
    /// </summary>
    public interface IBTTimelineDurationData
    {
        float duration { get; set; }
    }
    [NENodeBtn("编辑TimeLine", "NETreeWindowBtnExtension", "OnEditorBTTimeLine")]
    public class BTTimeLineData { }
    public class BTTimeLine : BTComposite<IBTContext, BTTimeLineData>
    {
        internal protected struct TimeLineData
        {
            public int count;
            public float time;
            public BTStatusArray stateArray;
        }

        private static TimeLineData DefaultTimeLineData = new TimeLineData(){count = 0, time = 0, stateArray = BTStatusArray.DefaultRunning};  

        protected override BTStatus Handler(IBTContext context, BTData btData, BTTimeLineData data)
        {
            var cacheData = context.executeCache.GetCache<TimeLineData>(btData.dataIndex, DefaultTimeLineData);
            int totalCount = btData.children == null ? 0 : btData.children.Count;
            for (int i = 0; i < totalCount; i++)
            {
                BTStatus preChildResult = cacheData.stateArray.GetState(i);
                if (preChildResult == BTStatus.Running)
                {
                    var childBTData = btData.children[i];
                    var btTimeLineData = (IBTTimeLineData) childBTData.data;
                    if (btTimeLineData != null)
                    {
                        if (btTimeLineData.time <= cacheData.time)
                        {
                            var childHandler = context.GetHandler(childBTData.keyIndex);
                            BTStatus childResult = childHandler.Handler(context, childBTData);
                            if (childResult != BTStatus.Running)
                            {
                                cacheData.stateArray.SetState(i, childResult);
                                cacheData.count = cacheData.count + 1;
                            }
                        }
                    }
                    else
                    {
                        CLog.LogError("BTTimeLine只允许IBTTimeLineData作为子节点，BTTimeLine停止运行!");
                        this.Clear(context, btData);
                        return BTStatus.Fail;
                    }
                   
                }
            }

            if (cacheData.count == totalCount)
            {
                this.Clear(context, btData);
                return BTStatus.Success;
            }

            cacheData.time = cacheData.time + context.deltaTime;
            context.executeCache.SetCache(btData.dataIndex, cacheData);
            return BTStatus.Running;
        }

    }
}