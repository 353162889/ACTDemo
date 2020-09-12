using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Framework
{
    [Serializable]
    public class ConstrainedChebyshevData : ICombineData
    {

    }
    public sealed class ConstrainedChebyshev : UtilityBase<ConstrainedChebyshevData>, IUtilityValueCombine
    {
        float _lowerBound;
        Chebyshev _measure;

        /// <summary>
        ///   If the combined value of any utility is below this, the value of this combine will be 0.
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

        public ConstrainedChebyshev()
        {
            _measure = new Chebyshev();
        }

        public ConstrainedChebyshev(float lowerBound)
        {
            LowerBound = lowerBound;
            _measure = new Chebyshev();
        }

        protected override void OnInit(ConstrainedChebyshevData data)
        {
        }
    }

}