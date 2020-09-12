namespace Framework
{
    /// <summary>
    /// 效用AI的基础动作行为
    /// </summary>
    public interface IUtilityAction : IUtility
    {
        string name { get; }
    }
}