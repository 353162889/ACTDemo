using System;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class UtilityGoalData : IUtilityGoalData
    {

        public IEvaluatorData selectedInertiaEvaluatorData
        {
            get { return mSelectedInertiaEvaluatorData; }
        }

        public IEvaluatorData unselectedInertiaEvaluatorData
        {
            get { return mUnselectedInertiaEvaluatorData; }
        }
        public IDecisionFactorData decisionFactorData
        {
            get { return mDecisionFactorData; }
        }
        public IUtilityActionData actionData
        {
            get { return mActionData; }
        }
        public float weight
        {
            get { return mWeight; }
        }

        public IEvaluatorData mSelectedInertiaEvaluatorData;
        public IEvaluatorData mUnselectedInertiaEvaluatorData;
        public IDecisionFactorData mDecisionFactorData;
        public IUtilityActionData mActionData;

        public string desc;

        [Range(0, 1f)]
        public float mWeight = 1f;

       
    }

    public class UtilityGoal : UtilityBase<UtilityGoalData>, IUtilityGoal
    {
        private IUtilityAction m_cUtilityAction;
        private IDecisionFactor m_cDecisionFactor;
        private IDecisionFactorEvaluator m_cSelectedInertiaEvaluator;
        private IDecisionFactorEvaluator m_cUnselectedInertiaEvaluator;
        private float m_fInertiaWeight;
        public float weight
        {
            get { return this.convertData.weight; }
        }

        public IUtilityAction action
        {
            get { return m_cUtilityAction; }
        }

        public ICompositeUtilityGoal parent { get; set; }
        public float startTime { get; set; }
        public bool selected { get; set; }
       
        public UtilityValue Decision(IUtilityContext context)
        {
            if (startTime > 0)
            {
                float inputTime = context.time - startTime;
                if (selected && this.m_cSelectedInertiaEvaluator != null)
                {
                    m_fInertiaWeight = Mathf.Clamp01(this.m_cSelectedInertiaEvaluator.Evaluate(inputTime));
                }
                else if(!selected && this.m_cUnselectedInertiaEvaluator != null)
                {
                    m_fInertiaWeight = Mathf.Clamp01(this.m_cUnselectedInertiaEvaluator.Evaluate(inputTime));
                }
                else
                {
                    m_fInertiaWeight = 1f;
                }
            }
            else
            {
                m_fInertiaWeight = 1f;
                startTime = context.time;
            }
            UtilityValue utilityValue = m_cDecisionFactor.Decision(context);
            var result = new UtilityValue(utilityValue.Value, weight * m_fInertiaWeight);
            this.utilityAI.Notify(this, UtilityNotifyType.Decision, result);
            return result;
        }

        public IUtilityGoal Selector(IUtilityContext context)
        {
            this.utilityAI.Notify(this, UtilityNotifyType.Selector, this);
            return this;
        }

        protected override void OnInit(UtilityGoalData data)
        {
            m_cUtilityAction = (IUtilityAction) this.utilityAI.CreateUtility(data.actionData);
            m_cDecisionFactor = (IDecisionFactor) this.utilityAI.CreateUtility(data.decisionFactorData);
            if(data.mSelectedInertiaEvaluatorData != null)
                m_cSelectedInertiaEvaluator = (IDecisionFactorEvaluator)this.utilityAI.CreateUtility(data.mSelectedInertiaEvaluatorData);
            if(data.mUnselectedInertiaEvaluatorData != null)
                m_cUnselectedInertiaEvaluator = (IDecisionFactorEvaluator)this.utilityAI.CreateUtility(data.mUnselectedInertiaEvaluatorData);
            startTime = 0f;
            m_fInertiaWeight = 1f;
        }
    }
}