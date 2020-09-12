using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Framework
{
    [Serializable]
    public class WeightedMetricsData : ICombineData
    {

    }
    public sealed class WeightedMetrics : UtilityBase<WeightedMetricsData>, IUtilityValueCombine
    {
        float _p;
        public readonly float PNormMin = 1.0f;
        public readonly float PNormMax = 10000.0f;

        public float PNorm
        {
            get { return _p; }
            set { _p = Mathf.Clamp(value, PNormMin, PNormMax); }
        }

        public float Combine(ICollection<UtilityValue> elements)
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
                var v = el.Weight / wsum * (float) Math.Pow(el.Value, _p);
                vlist.Add(v);
            }

            var sum = vlist.Sum();
            var res = (float) Math.Pow(sum, 1 / _p);

            return res;
        }

        public WeightedMetrics()
        {
            PNorm = 2.0f;
        }

        public WeightedMetrics(float pNorm)
        {
            PNorm = pNorm;
        }

        protected override void OnInit(WeightedMetricsData data)
        {
        }
    }

}