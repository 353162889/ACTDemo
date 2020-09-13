using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Framework
{
    [Serializable]
    public class ConstrainedWeightedMetricsData : ICombineData
    {
        [Range(1.0f, 10000.0f)]
        public float pow = 2f;
        [Range(0,1f)]
        public float lowerBound;
    }
    public sealed class ConstrainedWeightedMetrics : UtilityBase<ConstrainedWeightedMetricsData>, IUtilityValueCombine
    {
        float _lowerBound;

        /// <summary>
        ///   If the combined value of any utility is below this, the value of this measure will be 0.
        /// </summary>

        public float Combine(ICollection<UtilityValue> elements)
        {
            if (elements.Any(el => el.Combined < this.convertData.lowerBound))
                return 0.0f;

            return WeightedMetrics.StaticCombine(elements, this.convertData.pow);
        }

        public ConstrainedWeightedMetrics()
        {
        }

        protected override void OnInit(ConstrainedWeightedMetricsData data)
        {
        }
    }
}