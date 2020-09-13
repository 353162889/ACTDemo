using Framework;

namespace Game
{
    public class TargetDistanceDecisionFactorData : DecisionFactorData
    {
    }

    public class TargetDistanceDecisionFactor : DecisionFactorBase<UtilityContext, TargetDistanceDecisionFactorData>
    {
        public override UtilityValue Decision(UtilityContext context)
        {
            return new UtilityValue(this.evaluator.Evaluate(context.worldState.targetDistance), this.weight);
        }
    }
}