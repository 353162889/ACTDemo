namespace Framework
{
    public abstract class DecisionFactorBase<TContext,T> : UtilityBase<T>, IDecisionFactor where TContext : class, IUtilityContext where T : IDecisionFactorData
    {
        public float weight
        {
            get { return this.convertData.weight; }
        }

        protected IDecisionFactorEvaluator evaluator;

        public abstract UtilityValue Decision(TContext context);

        UtilityValue IDecisionFactor.Decision(IUtilityContext context)
        {
            var result = Decision((TContext)context);
            this.utilityAI.Notify(this, UtilityNotifyType.Decision, result);
            return result;
        }

        protected override void OnInit(T data)
        {
            if (data.evaluatorData != null)
            {
                evaluator = (IDecisionFactorEvaluator) this.utilityAI.CreateUtility(data.evaluatorData);
            }
        }
    }
}