using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Framework
{
    [Serializable]
    public class RandomSelectorData : ISelectorData
    {

    }
    public sealed class RandomSelector : UtilityBase<RandomSelectorData>, IUtilitySelector
    {
        public int Select(List<UtilityValue> elements)
        {
            var count = elements.Count;
            return count == 0 ? -1 : Random.Range(0, count);
        }

        protected override void OnInit(RandomSelectorData data)
        {
        }
    }

}