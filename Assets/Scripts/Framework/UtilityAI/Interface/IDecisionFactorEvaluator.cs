namespace Framework
{
    /// <summary>
    /// 决策因子评估器
    /// </summary>
    public interface IDecisionFactorEvaluator : IUtility
    {
        float Evaluate(float input);
    }
}