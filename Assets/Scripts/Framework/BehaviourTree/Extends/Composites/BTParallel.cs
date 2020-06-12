using System.Runtime.InteropServices.ComTypes;

namespace Framework
{
    public enum ParallelFailType
    {
        FailOnAll,
        FailOnOne,
    }

    public enum ParallelSuccessType
    {
        SuccessOnAll,
        SuccessOnOne,
    }

    public class BTParallelData
    {
        public ParallelFailType failType;
        public ParallelSuccessType successType;
        public bool childFinishLoop = true;
    }
    public class BTParallel : BTComposite<IBTContext, BTParallelData>
    {
        internal protected struct InnerBTParallelData
        {
            public BTStatusArray stateArray;
        }

        private static InnerBTParallelData DefaultData = new InnerBTParallelData() { stateArray = BTStatusArray.DefaultRunning };

        protected override BTStatus Handler(IBTContext context, BTData btData, BTParallelData data)
        {
            bool sawSuccess = false;
            bool sawFail = false;
            bool sawRunning = false;
            bool sawAllFails = true;
            bool sawAllSuccess = true;

            bool bLoop = data.childFinishLoop;
            var cacheData = context.executeCache.GetCache<InnerBTParallelData>(btData.dataIndex, DefaultData);
            int count = btData.children == null ? 0 : btData.children.Count;
            for (int i = 0; i < count; ++i)
            {
                var childStatus = cacheData.stateArray.GetState(i);

                if (bLoop || (childStatus == BTStatus.Running))
                {
                    var childBTData = btData.children[i];
                    var childHandler = context.GetHandler(childBTData.keyIndex);
                    BTStatus childResult = childHandler.Handler(context, childBTData);
                    cacheData.stateArray.SetState(i, childResult);
                    if (childResult == BTStatus.Fail)
                    {
                        sawFail = true;
                        sawAllSuccess = false;

                    }
                    else if (childResult == BTStatus.Success)
                    {
                        sawSuccess = true;
                        sawAllFails = false;

                    }
                    else if (childResult == BTStatus.Running)
                    {
                        sawRunning = true;
                        sawAllFails = false;
                        sawAllSuccess = false;
                    }
                }
                else if (childStatus == BTStatus.Success)
                {
                    sawSuccess = true;
                    sawAllFails = false;

                }
                else
                {
                    sawFail = true;
                    sawAllSuccess = false;
                }
            }

            BTStatus result = sawRunning ? BTStatus.Running : BTStatus.Fail;

            if ((data.failType == ParallelFailType.FailOnAll && sawAllFails) ||
                (data.failType == ParallelFailType.FailOnOne && sawFail))
            {
                result = BTStatus.Fail;

            }
            else if ((data.successType == ParallelSuccessType.SuccessOnAll && sawAllSuccess) ||
                     (data.successType == ParallelSuccessType.SuccessOnOne && sawSuccess))
            {
                result = BTStatus.Success;
            }

            context.executeCache.SetCache(btData.dataIndex, cacheData);

            return result;
        }
    }
}