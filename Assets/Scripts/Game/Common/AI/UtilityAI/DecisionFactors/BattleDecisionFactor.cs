using Framework;
using UnityEngine;

namespace Game
{
    public class BattleDecisionFactorData : IDecisionFactorData
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
    public class BattleDecisionFactor : DecisionFactorBase<UtilityContext, BattleDecisionFactorData>
    {
        public override UtilityValue Decision(UtilityContext context)
        {
            return new UtilityValue(context.targetFactor, weight);
        }
    }
}