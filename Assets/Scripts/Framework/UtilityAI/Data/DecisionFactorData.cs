using System;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class DecisionFactorData : IDecisionFactorData
    {
        public float weight
        {
            get { return mWeight; }
        }
        public IEvaluatorData evaluatorData
        {
            get { return mEvaluatorData; }
        }

        [Range(0,1f)]
        public float mWeight = 1f;
        public IEvaluatorData mEvaluatorData;
    }
}