namespace Framework
{
    /// <summary>
    /// 决策因子
    /// </summary>
    public interface IDecisionFactor : IUtility
    {
        float weight { get; }
        UtilityValue Decision(IUtilityContext context);
    }
}