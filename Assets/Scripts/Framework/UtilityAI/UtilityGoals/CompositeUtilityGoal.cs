using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class CompositeUtilityGoalData : ICompositeUtilityGoalData
    {
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
        public IDecisionFactorData mDecisionFactorData;
        public ISelectorData mSelectorData;
        public List<IUtilityGoalData> mLstGoalDatas;
    }

    public class CompositeUtilityGoal : UtilityBase<CompositeUtilityGoalData>, ICompositeUtilityGoal
    {
        private List<IUtilityGoal> m_lstUtilityGoal;
        private IDecisionFactor m_cDecisionFactor;
        private IUtilitySelector m_cSelector;

        public float weight
        {
            get { return this.convertData.weight; }
        }

        public CompositeUtilityGoal()
        {
            m_lstUtilityGoal = new List<IUtilityGoal>();
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
            return true;
        }

        protected override void OnInit(CompositeUtilityGoalData data)
        {
            m_cDecisionFactor = (IDecisionFactor)this.utilityAI.CreateUtility(data.decisionFactorData);
            m_cSelector = (IUtilitySelector) this.utilityAI.CreateUtility(data.selectorData);
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