using System;
using System.Collections.Generic;


namespace Framework
{
    [Serializable]
    public class MultiplicativePseudoMeasureData : ICombineData
    {

    }
    public sealed class MultiplicativePseudoMeasure : UtilityBase<MultiplicativePseudoMeasureData>, IUtilityValueCombine
    {
        public float Combine(ICollection<UtilityValue> elements)
        {
            var count = elements.Count;
            if (count == 0)
                return 0.0f;

            var res = 1.0f;
            foreach (var el in elements)
                res *= el.Combined;

            return res;
        }

        protected override void OnInit(MultiplicativePseudoMeasureData data)
        {
        }
    }

}