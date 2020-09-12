namespace Framework
{
    public abstract class EvaluatorBase<T>: UtilityBase<T>, IDecisionFactorEvaluator where T : IUtilityData
    {
        public float Evaluate(float input)
        {
            float result = OnEvaluate(input);
            this.utilityAI.Notify(this, UtilityNotifyType.Evaluator, result);
            return result;
        }

        public abstract float OnEvaluate(float input);
    }
}