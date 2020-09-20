namespace Framework
{
    /// <summary>
    /// 效用目标（包含效用因子与基础行为）
    /// </summary>
    public interface IUtilityGoal : IUtility
    {
        UtilityValue Decision(IUtilityContext context);
        IUtilityGoal Selector(IUtilityContext context);
        float weight { get; }
        IUtilityAction action { get; }
        ICompositeUtilityGoal parent { get; set; }
        float startTime { get; set; }
        bool selected { get; set; }
    }
}