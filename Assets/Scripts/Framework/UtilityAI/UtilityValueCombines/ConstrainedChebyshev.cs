using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Framework
{
    [Serializable]
    public class ConstrainedChebyshevData : ICombineData
    {
        [Range(0, 1f)]
        public float lowerBound;
    }
    public sealed class ConstrainedChebyshev : UtilityBase<ConstrainedChebyshevData>, IUtilityValueCombine
    {
        float _lowerBound;
        Chebyshev _measure;

        /// <summary>
        ///   If the combined value of any utility is below this, the value of this combine will be 0.
        /// </summary>

        public float Combine(ICollection<UtilityValue> elements)
        {
            if (elements.Any(el => el.Combined < this.convertData.lowerBound))
                return 0.0f;

            return _measure.Combine(elements);
        }

        public ConstrainedChebyshev()
        {
            _measure = new Chebyshev();
        }

        protected override void OnInit(ConstrainedChebyshevData data)
        {
        }
    }

}