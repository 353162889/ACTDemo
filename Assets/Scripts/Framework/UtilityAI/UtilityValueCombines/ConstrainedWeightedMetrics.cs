using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Framework
{
    [Serializable]
    public class ConstrainedWeightedMetricsData : ICombineData
    {

    }
    public sealed class ConstrainedWeightedMetrics : UtilityBase<ConstrainedWeightedMetricsData>, IUtilityValueCombine
    {
        float _lowerBound;
        WeightedMetrics _measure;

        public float PNormMin
        {
            get { return _measure.PNormMin; }
        }

        public float PNormMax
        {
            get { return _measure.PNormMax; }
        }

        public float PNorm
        {
            get { return _measure.PNorm; }
            set { _measure.PNorm = value; }
        }

        /// <summary>
        ///   If the combined value of any utility is below this, the value of this measure will be 0.
        /// </summary>
        public float LowerBound
        {
            get { return _lowerBound; }
            set { _lowerBound = Mathf.Clamp01(value); }
        }

        public float Combine(ICollection<UtilityValue> elements)
        {
            if (elements.Any(el => el.Combined < LowerBound))
                return 0.0f;

            return _measure.Combine(elements);
        }

        public ConstrainedWeightedMetrics()
        {
            _measure = new WeightedMetrics();
        }

        public ConstrainedWeightedMetrics(float pNorm)
        {
            _measure = new WeightedMetrics(pNorm);
        }

        public ConstrainedWeightedMetrics(float pNorm, float lowerBound)
        {
            _measure = new WeightedMetrics(pNorm);
            LowerBound = lowerBound;
        }

        protected override void OnInit(ConstrainedWeightedMetricsData data)
        {
        }
    }
}