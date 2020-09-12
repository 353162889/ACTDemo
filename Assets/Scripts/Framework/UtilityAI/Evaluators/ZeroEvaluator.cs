using System;

namespace Framework
{
    [Serializable]
    public class ZeroEvaluatorData : IEvaluatorData
    {

    }

    public class ZeroEvaluator : EvaluatorBase<ZeroEvaluatorData>
    {
        protected override void OnInit(ZeroEvaluatorData data)
        {
        }

        public override float OnEvaluate(float input)
        {
            return 0;
        }
    }
}