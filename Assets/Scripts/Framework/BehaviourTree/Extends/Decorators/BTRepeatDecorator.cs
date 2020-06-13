namespace Framework
{
    public class BTRepeatDecoratorData
    {
        public int times;
        public float deltaTime;
        public float spaceTime;
    }
    public class BTRepeatDecorator : BTDecorator<IBTContext, BTRepeatDecoratorData>
    {
        internal protected struct RepeatDecoratorData
        {
            public int times;
            public float executeTime;
        }

        private static RepeatDecoratorData DefaultRepeatDecoratorData = new RepeatDecoratorData() { times = 0, executeTime = 0 };

        protected override BTStatus OnHandler(IBTContext context, BTData btData, BTRepeatDecoratorData data, BTData childBTData)
        {
            var cacheData = context.executeCache.GetCache<RepeatDecoratorData>(btData.dataIndex, DefaultRepeatDecoratorData);
            cacheData.executeTime = cacheData.executeTime - context.deltaTime;
            if (cacheData.executeTime <= 0)
            {
                cacheData.executeTime += data.spaceTime;
                cacheData.times = cacheData.times + 1;
                BTStatus childResult = base.OnHandler(context, btData, data, childBTData);
                if (data.times == 0 || cacheData.times < data.times)
                {
                    childResult = BTStatus.Running;
                }
                else
                {
                    childResult = BTStatus.Success;
                }
                return childResult;
            }
            else
            {
                return BTStatus.Running;
            }
            
        }

    }
}