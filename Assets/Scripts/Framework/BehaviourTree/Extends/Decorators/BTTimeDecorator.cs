using UnityEngine;

namespace Framework
{
    public class BTTimeDecoratorData : IBTTimeLineData
    {
        public float exeTime;
        public float time
        {
            get { return exeTime; }
            set { exeTime = value; }
        }
    }
    public class BTTimeDecorator : BTDecorator<IBTContext, BTTimeDecoratorData>
    {
    }
}