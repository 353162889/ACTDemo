namespace Framework
{
    /// <summary>
    /// 组合决策因子，由多个觉得因子组合而成
    /// </summary>
    public interface ICompositeDecisionFactor : IDecisionFactor
    {
        bool AddDecisionFactor(IDecisionFactor decisionFactor);
    }
}