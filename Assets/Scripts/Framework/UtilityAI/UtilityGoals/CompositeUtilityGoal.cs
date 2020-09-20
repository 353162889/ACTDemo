using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class CompositeUtilityGoalData : ICompositeUtilityGoalData
    {

        public IEvaluatorData selectedInertiaEvaluatorData
        {
            get { return mSelectedInertiaEvaluatorData; }
        }

        public IEvaluatorData unselectedInertiaEvaluatorData
        {
            get { return mUnselectedInertiaEvaluatorData; }
        }

        public IUtilityActionData actionData
        {
            get { return null; }
        }
        public IDecisionFactorData decisionFactorData
        {
            get { return mDecisionFactorData; }
        }
        public float weight
        {
            get { return mWeight; }
        }
        public ISelectorData selectorData
        {
            get { return mSelectorData; }
        }

        public List<IUtilityGoalData> lstGoalDatas
        {
            get { return mLstGoalDatas; }
        }

        public string desc;
        [Range(0, 1f)]
        public float mWeight = 1f;
        public IEvaluatorData mSelectedInertiaEvaluatorData;
        public IEvaluatorData mUnselectedInertiaEvaluatorData;
        public IDecisionFactorData mDecisionFactorData;
        public ISelectorData mSelectorData;
        public List<IUtilityGoalData> mLstGoalDatas;
    }

    public class CompositeUtilityGoal : UtilityBase<CompositeUtilityGoalData>, ICompositeUtilityGoal
    {
        private List<IUtilityGoal> m_lstUtilityGoal;
        private IDecisionFactor m_cDecisionFactor;
        private IUtilitySelector m_cSelector;
        private IDecisionFactorEvaluator m_cSelectedInertiaEvaluator;
        private IDecisionFactorEvaluator m_cUnselectedInertiaEvaluator;
        private float m_fInertiaWeight;

        public float weight
        {
            get { return this.convertData.weight; }
        }

        public IUtilityAction action
        {
            get { return null; }
        }

        public ICompositeUtilityGoal parent { get; set; }
        public float startTime { get; set; }
        public bool selected { get; set; }

        public CompositeUtilityGoal()
        {
            m_lstUtilityGoal = new List<IUtilityGoal>();
        }

        public UtilityValue Decision(IUtilityContext context)
        {
            if (startTime > 0)
            {
                float inputTime = context.time - startTime;
                if (selected && this.m_cSelectedInertiaEvaluator != null)
                {
                    m_fInertiaWeight = this.m_cSelectedInertiaEvaluator.Evaluate(inputTime);
                }
                else if (!selected && this.m_cUnselectedInertiaEvaluator != null)
                {
                    m_fInertiaWeight = this.m_cUnselectedInertiaEvaluator.Evaluate(inputTime);
                }
                else
                {
                    m_fInertiaWeight = 1f;
                }
            }
            else
            {
                m_fInertiaWeight = 1f;
            }
            UtilityValue utilityValue = m_cDecisionFactor.Decision(context);
            var result = new UtilityValue(utilityValue.Value, weight * m_fInertiaWeight);
            this.utilityAI.Notify(this, UtilityNotifyType.Decision, result);
            return result;
        }

        public IUtilityGoal Selector(IUtilityContext context)
        {
            var cacheUtilityValues = ResetObjectPool<List<UtilityValue>>.Instance.GetObject();
            for (int i = 0; i < m_lstUtilityGoal.Count; i++)
            {
                var subGoal = m_lstUtilityGoal[i];
                UtilityValue utilityValue = subGoal.Decision(context);
                cacheUtilityValues.Add(utilityValue);
            }

            int index = m_cSelector.Select(cacheUtilityValues);
            ResetObjectPool<List<UtilityValue>>.Instance.SaveObject(cacheUtilityValues);
            IUtilityGoal utilityGoal = index >= 0 ? m_lstUtilityGoal[index] : null;
            var result = utilityGoal?.Selector(context);
            this.utilityAI.Notify(this, UtilityNotifyType.Selector, result);
            return result;
        }

        public bool AddUtilityGoal(IUtilityGoal utilityGoal)
        {
            if (utilityGoal == null) return false;
            if (m_lstUtilityGoal.Contains(utilityGoal)) return false;
            m_lstUtilityGoal.Add(utilityGoal);
            utilityGoal.parent = this;
            return true;
        }

        protected override void OnInit(CompositeUtilityGoalData data)
        {
            startTime = 0;
            m_cDecisionFactor = (IDecisionFactor)this.utilityAI.CreateUtility(data.decisionFactorData);
            m_cSelector = (IUtilitySelector) this.utilityAI.CreateUtility(data.selectorData);
            if(data.mSelectedInertiaEvaluatorData != null)
                m_cSelectedInertiaEvaluator = (IDecisionFactorEvaluator)this.utilityAI.CreateUtility(data.mSelectedInertiaEvaluatorData);
            if(data.mUnselectedInertiaEvaluatorData != null)
                m_cUnselectedInertiaEvaluator = (IDecisionFactorEvaluator)this.utilityAI.CreateUtility(data.mUnselectedInertiaEvaluatorData);
            if (data.lstGoalDatas != null)
            {
                for (int i = 0; i < data.lstGoalDatas.Count; i++)
                {
                    var childData = data.lstGoalDatas[i];
                    var childUtilityGoal = (IUtilityGoal)this.utilityAI.CreateUtility(childData);
                    if (childUtilityGoal != null)
                    {
                        AddUtilityGoal(childUtilityGoal);
                    }
                }
            }
        }
    }
}