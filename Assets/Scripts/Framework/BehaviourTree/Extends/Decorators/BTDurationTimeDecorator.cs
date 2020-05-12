namespace Framework
{
    public class BTDurationTimeDecoratorData : IBTTimelineDurationData
    {
        public float executeDuration;
        public bool failEnd = false;
        public bool successEnd = false;
        public float duration
        {
            get => executeDuration;
            set => executeDuration = value;
        }
    }
    /// <summary>
    /// timeline特殊节点，duration时间内每帧执行
    /// </summary>
    public class BTDurationTimeDecorator : BTDecorator<IBTContext, BTDurationTimeDecoratorData>
    {

        internal protected struct InnerBTDurationTimeDecoratorData
        {
            public float time;
        }

        private static InnerBTDurationTimeDecoratorData DefaultActionData = new InnerBTDurationTimeDecoratorData() { time = 0 };

        protected override BTStatus OnHandler(IBTContext context, BTData btData, BTDurationTimeDecoratorData data, BTData childBTData)
        {
            BTStatus childResult = base.OnHandler(context, btData, data, childBTData);
            var cacheData =
                context.executeCache.GetCache<InnerBTDurationTimeDecoratorData>(btData.dataIndex, DefaultActionData);
            cacheData.time += context.deltaTime;
            context.executeCache.SetCache(btData.dataIndex, cacheData);
            BTStatus result;
            if ((data.failEnd && childResult == BTStatus.Fail) || (data.successEnd & childResult == BTStatus.Success))
            {
                result = childResult;
            }
            else if (cacheData.time >= data.duration)
            {
                result = BTStatus.Success;
            }
            else
            {
                result = BTStatus.Running;
            }

            return result;
        }
    }
}