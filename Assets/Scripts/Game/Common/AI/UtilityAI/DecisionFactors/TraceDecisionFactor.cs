using Framework;
using UnityEngine;

namespace Game
{
    public class TraceDecisionFactorData : DecisionFactorData
    {

    }
    public class TraceDecisionFactor : DecisionFactorBase<UtilityContext, TraceDecisionFactorData>
    {
        public TraceDecisionFactor()
        {
//            evaluator = new LinearTwoPointEvaluator(new Vector2(maxAttackDistance, 0), new Vector2(maxTraceDistance, 1) );
        }

        public override UtilityValue Decision(UtilityContext context)
        {
            return new UtilityValue(evaluator.Evaluate(context.targetDistance), weight);
        }
    }
}