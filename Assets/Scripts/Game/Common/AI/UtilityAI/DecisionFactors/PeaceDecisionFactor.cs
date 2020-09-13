using Framework;
using UnityEngine;

namespace Game
{
    public class PeaceDecisionFactorData : IDecisionFactorData
    {
        public float weight
        {
            get { return mWeight; }
        }
        public IEvaluatorData evaluatorData
        {
            get { return null; }
        }
        [Range(0, 1f)]
        public float mWeight = 1f;
    }
    public class PeaceDecisionFactor : DecisionFactorBase<UtilityContext, PeaceDecisionFactorData>
    {
        public override UtilityValue Decision(UtilityContext context)
        {
            return new UtilityValue(Mathf.Abs(context.worldState.targetFactor - 1), weight);
        }
    }
}