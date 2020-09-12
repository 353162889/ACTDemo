using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Framework
{
    [Serializable]
    public class WeightedRandomSelectorData : ISelectorData
    {

    }
    public sealed class WeightedRandomSelector : UtilityBase<WeightedRandomSelectorData>, IUtilitySelector
    {
        float _proportion = 0.2f;

        public float Proportion
        {
            get { return _proportion; }
            set { _proportion = Mathf.Clamp01(value); }
        }

        public int Select(List<UtilityValue> elements)
        {
            var count = elements.Count;
            if (count == 0)
                return -1;

            var maxElemIdx = Mathf.Clamp(Mathf.CeilToInt(_proportion * count), 0, count - 1);

            var sorted = elements.Select((x, i) => new KeyValuePair<UtilityValue, int>(x, i))
                .OrderByDescending(x => x.Key.Combined);

            List<UtilityValue> sortedUtils = sorted.Select(x => x.Key).ToList();
            List<int> sortedUtilIndices = sorted.Select(x => x.Value).ToList();

            if (maxElemIdx == 0)
                return sortedUtilIndices[0];

            var cumSum = new float[maxElemIdx];
            cumSum[0] = sortedUtils[0].Weight;
            for (int i = 1; i < maxElemIdx; i++)
                cumSum[i] = cumSum[i - 1] + sortedUtils[i].Weight;
            for (int i = 0; i < maxElemIdx; i++)
                cumSum[i] /= cumSum[maxElemIdx - 1];

            
            float rval = Random.Range(0, 1f);
            int index = Array.BinarySearch(cumSum, rval);

            // From MSDN: If the index is negative, it represents the bitwise
            // complement of the next larger element in the array.
            if (index < 0)
                index = ~index;
            return sortedUtilIndices[index];
        }

        public WeightedRandomSelector()
        {
        }

        public WeightedRandomSelector(float proportion)
        {
            Proportion = proportion;
        }

        protected override void OnInit(WeightedRandomSelectorData data)
        {
        }
    }

}