using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 将多个效用值合并为一个效用值
    /// </summary>
    public interface IUtilityValueCombine : IUtility
    {
        float Combine(ICollection<UtilityValue> elements);
    }
}