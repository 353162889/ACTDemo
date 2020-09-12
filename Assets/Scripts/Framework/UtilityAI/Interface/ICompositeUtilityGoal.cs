namespace Framework
{
    public interface ICompositeUtilityGoal : IUtilityGoal
    {
        bool AddUtilityGoal(IUtilityGoal utilityGoal);
    }
}