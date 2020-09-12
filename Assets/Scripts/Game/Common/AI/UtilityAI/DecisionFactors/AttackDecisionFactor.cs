using Framework;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class AttackDecisionFactorData : DecisionFactorData
    {

    }
    public class AttackDecisionFactor : DecisionFactorBase<UtilityContext, AttackDecisionFactorData>
    {

        public AttackDecisionFactor()
        {
//            evaluator = new LinearTwoPointEvaluator(new Vector2(0, 1), new Vector2(maxAttackDistance, 0));
        }

        public override UtilityValue Decision(UtilityContext context)
        {
            CLog.Log("attackDecision:"+weight);
            return new UtilityValue(evaluator.Evaluate(context.targetDistance), weight);
        }
    }
}