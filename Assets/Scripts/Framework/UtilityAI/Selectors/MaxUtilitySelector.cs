using System;
using System.Collections.Generic;


namespace Framework
{
    [Serializable]
    public class MaxUtilitySelectorData : ISelectorData
    {
    }

    public sealed class MaxUtilitySelector : UtilityBase<MaxUtilitySelectorData>, IUtilitySelector
    {
        public int Select(List<UtilityValue> elements)
        {
            var count = elements.Count;
            if (count == 0)
                return -1;
            if (count == 1)
                return 0;

            var maxUtil = 0.0f;
            var selIdx = -1;
            for (var i = 0; i < count; i++)
            {
                var el = elements[i];
                if (el.Combined > maxUtil)
                {
                    maxUtil = el.Combined;
                    selIdx = i;
                }
            }
            return selIdx;
        }

        protected override void OnInit(MaxUtilitySelectorData data)
        {
        }
    }

}