using System;

namespace Framework
{
    [Serializable]
    public class OneEvaluatorData : IEvaluatorData
    {

    }
    public class OneEvaluator : EvaluatorBase<OneEvaluatorData>
    {
        protected override void OnInit(OneEvaluatorData data)
        {
        }

        public override float OnEvaluate(float input)
        {
            return 1;
        }
    }
}