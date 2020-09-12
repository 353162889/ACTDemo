using System;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class UtilityGoalData : IUtilityGoalData
    {
        public IUtilityActionData actionData
        {
            get { return mActionData; }
        }
        public IDecisionFactorData decisionFactorData
        {
            get { return mDecisionFactorData; }
        }
        public float weight
        {
            get { return mWeight; }
        }

        public IUtilityActionData mActionData;
        public IDecisionFactorData mDecisionFactorData;

        public string desc;

        [Range(0, 1f)]
        public float mWeight = 1f;
    }

    public class UtilityGoal : UtilityBase<UtilityGoalData>, IUtilityGoal
    {
        private IUtilityAction m_cUtilityAction;
        private IDecisionFactor m_cDecisionFactor;
        public float weight
        {
            get { return this.convertData.weight; }
        }

        public UtilityValue Decision(IUtilityContext context)
        {
            UtilityValue utilityValue = m_cDecisionFactor.Decision(context);
            var result = new UtilityValue(utilityValue.Value, weight);
            this.utilityAI.Notify(this, UtilityNotifyType.Decision, result);
            return result;
        }

        public IUtilityAction Selector(IUtilityContext context)
        {
            this.utilityAI.Notify(this, UtilityNotifyType.Selector, m_cUtilityAction);
            return m_cUtilityAction;
        }

        protected override void OnInit(UtilityGoalData data)
        {
            m_cUtilityAction = (IUtilityAction) this.utilityAI.CreateUtility(data.actionData);
            m_cDecisionFactor = (IDecisionFactor) this.utilityAI.CreateUtility(data.decisionFactorData);
        }
    }
}