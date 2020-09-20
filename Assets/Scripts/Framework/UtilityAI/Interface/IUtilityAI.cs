namespace Framework
{
    public enum UtilityNotifyType
    {
        Evaluator,
        Decision,
        Selector,
        BeSelected,
    }

    public interface IUtilityAI
    {
        void Notify<T>(IUtility utility, UtilityNotifyType notifyType, T msg);
        void Notify(IUtility utility, UtilityNotifyType notifyType);
        IUtility CreateUtility(IUtilityData utilityData);
        IUtilityGoal currentGoal { get; }
    }
}