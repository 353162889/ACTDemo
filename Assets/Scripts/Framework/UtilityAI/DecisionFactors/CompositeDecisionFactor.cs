
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class CompositeDecisionFactorData : ICompositeDecisionFactorData
    {
        public float weight { get; }
        public IEvaluatorData evaluatorData
        {
            get { return null; }
        }
        public ICombineData combineData
        {
            get { return mCombineData; }
        }
        public List<IDecisionFactorData> lstFactor
        {
            get { return mLstFactor; }
        }

        [Range(0, 1f)]
        public float mWeight = 1f;
        public ICombineData mCombineData;
        public List<IDecisionFactorData> mLstFactor;
    }

    public class CompositeDecisionFactor : UtilityBase<CompositeDecisionFactorData>, ICompositeDecisionFactor
    {
        private float m_fWeight;
        public float weight
        {
            get { return this.convertData.weight; }
        }

        private IUtilityValueCombine m_cCombine;
        private List<IDecisionFactor> m_lstDecisionFactor;

        public CompositeDecisionFactor()
        {
            m_lstDecisionFactor = new List<IDecisionFactor>();
        }

        public UtilityValue Decision(IUtilityContext context)
        {
            var cacheUtilityValues = ResetObjectPool<List<UtilityValue>>.Instance.GetObject();
            for (int i = 0; i < m_lstDecisionFactor.Count; i++)
            {
                var factor = m_lstDecisionFactor[i];
                UtilityValue utilityValue = factor.Decision(context);
                cacheUtilityValues.Add(utilityValue);
            }

            float value = m_cCombine.Combine(cacheUtilityValues);
            ResetObjectPool<List<UtilityValue>>.Instance.SaveObject(cacheUtilityValues);
            var result = new UtilityValue(value, weight);
            this.utilityAI.Notify(this, UtilityNotifyType.Decision, result);
            return result;
        }

        public bool AddDecisionFactor(IDecisionFactor decisionFactor)
        {
            if (decisionFactor == null) return false;
            if (m_lstDecisionFactor.Contains(decisionFactor)) return false;
            m_lstDecisionFactor.Add(decisionFactor);
            return true;
        }

        protected override void OnInit(CompositeDecisionFactorData data)
        {
            m_cCombine = (IUtilityValueCombine)this.utilityAI.CreateUtility(data.combineData);
        }
    }
}