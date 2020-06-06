using Framework;
using UnityEngine;

namespace Game
{
    public class AIRandomWaitActionData
    {
        public float minTime;
        public float maxTime;
    }
    public class AIRandomWaitAction : BTAction<AIBTContext, AIRandomWaitActionData>
    {
        internal protected struct InnerAIRandomWaitActionData
        {
            public float waitTime;
            public float time;
        }
        private static InnerAIRandomWaitActionData DefaultActionData = new InnerAIRandomWaitActionData() { time = 0, waitTime = 0};
        protected override BTStatus Handler(AIBTContext context, BTData btData, AIRandomWaitActionData data)
        {
            var cacheData =
                context.executeCache.GetCache<InnerAIRandomWaitActionData>(btData.dataIndex, DefaultActionData);
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                float min = data.minTime;
                float max = data.maxTime;
                if (min > max)
                {
                    max = min;
                }
                if (min == max)
                {
                    cacheData.waitTime = min;
                }
                else
                {
                    cacheData.waitTime = Random.Range(min, max);
                }
            }
            cacheData.time += context.deltaTime;
            context.executeCache.SetCache(btData.dataIndex, cacheData);
            if (cacheData.time >= cacheData.waitTime)
            {
                return BTStatus.Success;
            }

            return BTStatus.Running;
        }
    }
}