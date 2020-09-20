using Framework;

namespace Game
{

    public class AttackDesiredDecisionFactorData : DecisionFactorData
    {
    }

    public class AttackDesiredDecisionFactor : DecisionFactorBase<UtilityContext, AttackDesiredDecisionFactorData>
    {
        public override UtilityValue Decision(UtilityContext context)
        {
            return new UtilityValue(this.evaluator.Evaluate(context.worldState.attackDesired), this.weight);
        }
    }
}