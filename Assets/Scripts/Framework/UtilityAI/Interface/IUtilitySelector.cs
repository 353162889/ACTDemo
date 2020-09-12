using System.Collections.Generic;

namespace Framework
{
    public interface IUtilitySelector : IUtility
    {
        int Select(List<UtilityValue> elements);
    }
}