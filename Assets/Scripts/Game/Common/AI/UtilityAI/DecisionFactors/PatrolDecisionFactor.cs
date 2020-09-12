using Framework;
using UnityEngine;

namespace Game
{
    public class PatrolDecisionFactorData : DecisionFactorData
    {

    }
    public class PatrolDecisionFactor : DecisionFactorBase<UtilityContext, PatrolDecisionFactorData>
    {
        public PatrolDecisionFactor()
        {
//            evaluator = new LinearTwoPointEvaluator(new Vector2(0, 1), new Vector2(patrolRadius, 0));
        }

        public override UtilityValue Decision(UtilityContext context)
        {
            return new UtilityValue(evaluator.Evaluate(context.targetDistance), weight);
        }
    }
}