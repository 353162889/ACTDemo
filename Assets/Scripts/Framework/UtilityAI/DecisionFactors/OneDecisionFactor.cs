using UnityEngine;

namespace Framework
{
    public class OneDecisionFactorData : IDecisionFactorData
    {
        public float weight { get; }
        public IEvaluatorData evaluatorData
        {
            get { return null; }
        }

        [Range(0, 1f)]
        public float mWeight = 1f;
    }
    public class OneDecisionFactor : DecisionFactorBase<IUtilityContext, OneDecisionFactorData>
    {
        public override UtilityValue Decision(IUtilityContext context)
        {
            return new UtilityValue(1f, this.weight);
        }
    }
}