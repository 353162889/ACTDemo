using System;
using System.Collections.Generic;
using System.Diagnostics;
using Game;
using UnityEngine.Assertions.Must;
using UnityEngine.Networking;

namespace Framework
{
    [Serializable]
    public class UtilityAIData : IUtilityAIData
    {
        public ISelectorData selectorData
        {
            get { return mSelectorData; }
        }
        public List<IUtilityGoalData> lstGoalDatas
        {
            get { return mLstGoalDatas; }
        }

        public ISelectorData mSelectorData;
        public List<IUtilityGoalData> mLstGoalDatas;
    }

    public struct UtilityDebugInfo
    {
        public IUtilityData utilityData;
        public UtilityNotifyType notifyType;
        public object msg;

        public UtilityDebugInfo(IUtilityData utilityData, UtilityNotifyType notifyType, object msg)
        {
            this.utilityData = utilityData;
            this.notifyType = notifyType;
            this.msg = msg;
        }
    }

    public class UtilityDebugEventDispatcher : EventDispatcher
    {
        private static UtilityDebugEventDispatcher m_cInstance;
        public static UtilityDebugEventDispatcher Instance
        {
            get
            {
                if (m_cInstance == null)
                {
                    m_cInstance = new UtilityDebugEventDispatcher();
                }
                return m_cInstance;
            }
        }
    }

    public static class UtilityDebugEvent
    {
        public static int SendDebugInfo = 1;
        public static int SendUtilityAIData = 2;
        public static int UpdateUtilityAIData = 3;
        public static int StartDebug = 4;
        public static int StopDebug = 5;
    }

    public class UtilityAI : IUtilityAI
    {
        private UtilityAIData utilityAIData;
        private List<IUtilityGoal> m_lstUtilityGoal;
        private IUtilitySelector m_cSelector;
        private IUtilityGoal m_cCurrentGoal;
        public IUtilityGoal currentGoal
        {
            get { return m_cCurrentGoal; }
        }
        private bool m_bDebug = false;
        public bool isDebug
        {
            get { return m_bDebug; }
        }
        private List<UtilityDebugInfo> m_lstDebugCacheInfo = new List<UtilityDebugInfo>() ;

        private static bool m_bInit = false;
        private static void StaticInitialize()
        {
            if (!m_bInit)
            {
                m_bInit = true;
                ResetObjectPool<List<UtilityValue>>.Instance.Init(10, lst => lst.Clear());
            }
        }

        public UtilityAI()
        {
            m_lstUtilityGoal = new List<IUtilityGoal>();
            StaticInitialize();
        }

        public void Init(UtilityAIData utilityData)
        {
            this.utilityAIData = utilityData;
            m_cSelector = (IUtilitySelector)this.CreateUtility(utilityData.selectorData);
            if (utilityData.lstGoalDatas != null)
            {
                for (int i = 0; i < utilityData.lstGoalDatas.Count; i++)
                {
                    var childData = utilityData.lstGoalDatas[i];
                    IUtilityGoal child = (IUtilityGoal) this.CreateUtility(childData);
                    AddUtilityGoal(child);
                }
            }
        }

        public bool AddUtilityGoal(IUtilityGoal utilityGoal)
        {
            if (utilityGoal == null) return false;
            if (m_lstUtilityGoal.Contains(utilityGoal)) return false;
            m_lstUtilityGoal.Add(utilityGoal);
            return true;
        }

        public void Select(IUtilityContext context)
        {
            var cacheUtilityValues = ResetObjectPool<List<UtilityValue>>.Instance.GetObject();
            for (int i = 0; i < m_lstUtilityGoal.Count; i++)
            {
                UtilityValue utilityValue = m_lstUtilityGoal[i].Decision(context);
                cacheUtilityValues.Add(utilityValue);
            }

            int index = m_cSelector.Select(cacheUtilityValues);
            ResetObjectPool<List<UtilityValue>>.Instance.SaveObject(cacheUtilityValues);
            IUtilityGoal utilityGoal = index >= 0 ? m_lstUtilityGoal[index] : null;
            var resultUtilityGoal = utilityGoal?.Selector(context);
            if (resultUtilityGoal != null)
            {
                Notify(resultUtilityGoal, UtilityNotifyType.BeSelected);
            }

            if (m_bDebug)
            {
                UtilityDebugEventDispatcher.Instance.Dispatch(UtilityDebugEvent.SendUtilityAIData, this.utilityAIData);
                UtilityDebugEventDispatcher.Instance.Dispatch(UtilityDebugEvent.SendDebugInfo, m_lstDebugCacheInfo);
                m_lstDebugCacheInfo = new List<UtilityDebugInfo>();
            }

            var oldGoal = m_cCurrentGoal;
            m_cCurrentGoal = resultUtilityGoal;
            if (oldGoal != m_cCurrentGoal)
            {
                var temp = oldGoal;
                while (temp != null)
                {
                    if (temp.selected != false) temp.startTime = context.time;
                    temp.selected = false;
                    temp = temp.parent;
                }
                temp = m_cCurrentGoal;
                while (temp != null)
                {
                    if (temp.selected != true) temp.startTime = context.time;
                    temp.selected = true;
                    temp = temp.parent;
                }
            }
        }

        public void SetDebug(bool debug)
        {
            m_bDebug = debug;
        }

        public void Notify<T>(IUtility utility, UtilityNotifyType notifyType, T msg)
        {
            if (!m_bDebug) return;
            m_lstDebugCacheInfo.Add( new UtilityDebugInfo(utility.utilityData, notifyType, msg));
        }

        public void Notify(IUtility utility, UtilityNotifyType notifyType)
        {
            if (!m_bDebug) return;
            m_lstDebugCacheInfo.Add(new UtilityDebugInfo(utility.utilityData, notifyType, null));
        }

        public IUtility CreateUtility(IUtilityData utilityData)
        {
            Type t;
            if (UtilityAIInitialize.GetUtilityType(utilityData.GetType(), out t))
            {
                var utility = (IUtility)Activator.CreateInstance(t);
                if (utility != null)
                {
                    utility.Init(this, utilityData);
                }
                return utility;
            }

            return null;
        }

        
    }
}