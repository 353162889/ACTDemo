using JetBrains.Annotations;

namespace Framework
{
    public interface IUtility
    {
        IUtilityAI utilityAI { get; }
        IUtilityData utilityData { get; }
        void Init(IUtilityAI utilityAI, IUtilityData data);
    }
}