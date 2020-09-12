using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Framework
{
    [Serializable]
    public class ChebyshevData : ICombineData
    {

    }

    public sealed class Chebyshev : UtilityBase<ChebyshevData>, IUtilityValueCombine
    {
        public float Combine(ICollection<UtilityValue> elements)
        {
            var wsum = 0.0f;
            int count = elements.Count;

            if (count == 0)
                return 0.0f;

            foreach (var el in elements)
                wsum += el.Weight;

            if (Mathf.Approximately(wsum, 0))
                return 0.0f;

            var vlist = new List<float>(count);
            foreach (var el in elements)
                vlist.Add(el.Value * (el.Weight / wsum));

            var ret = vlist.Max<float>();
            return ret;
        }

        protected override void OnInit(ChebyshevData data)
        {
        }
    }

}