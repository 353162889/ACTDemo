using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Framework
{
    [Serializable]
    public class WeightedMetricsData : ICombineData
    {
        [Range(1.0f, 10000.0f)]
        public float pow = 2f;
    }
    public sealed class WeightedMetrics : UtilityBase<WeightedMetricsData>, IUtilityValueCombine
    {
        public static float StaticCombine(ICollection<UtilityValue> elements, float pow)
        {
            var count = elements.Count;
            if (count == 0)
                return 0.0f;

            var wsum = 0.0f;
            foreach (var el in elements)
                wsum += el.Weight;

            if (Mathf.Approximately(wsum, 0))
                return 0.0f;

            var vlist = new List<float>(count);
            foreach (var el in elements)
            {
                var v = el.Weight / wsum * (float)Math.Pow(el.Value, pow);
                vlist.Add(v);
            }

            var sum = vlist.Sum();
            var res = (float)Math.Pow(sum, 1 / pow);

            return res;
        }

        public float Combine(ICollection<UtilityValue> elements)
        {
            return StaticCombine(elements, this.convertData.pow);
        }

        public WeightedMetrics()
        {
        }

        protected override void OnInit(WeightedMetricsData data)
        {
        }
    }

}