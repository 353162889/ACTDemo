using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BTExecuteCache
    {
        private List<ValueType> lstData = new List<ValueType>();
        private List<BTExecuteStatus> lstExecuteStatus = new List<BTExecuteStatus>();

        public void SetCache(int index, ValueType value)
        {
            if (index < 0)
            {
                Debug.LogError("index must great or equal 0");
                return;
            }

            if (index > lstData.Count)
            {
                for (int i = lstData.Count; i <= index; i++)
                {
                    lstData.Add(null);
                }
            }
            lstData[index] = value;
        }

        public T GetCache<T>(int index, T defaultValue = default(T)) where T : struct
        {
            if (index < 0 || index >= lstData.Count) return defaultValue;
            var value = lstData[index];
            if (value == null) return defaultValue;
            return (T) value;
        }

        public void SetExecuteStatus(int index, BTExecuteStatus value)
        {
            if (index < 0)
            {
                Debug.LogError("index must great or equal 0");
                return;
            }

            if (index > lstExecuteStatus.Count)
            {
                for (int i = lstExecuteStatus.Count; i <= index; i++)
                {
                    lstExecuteStatus.Add(BTExecuteStatus.Ready);
                }
            }
            lstExecuteStatus[index] = value;
        }

        public BTExecuteStatus GetExecuteStatus(int index)
        {
            if (index < 0 || index >= lstExecuteStatus.Count) return BTExecuteStatus.Ready;
            var value = lstExecuteStatus[index];
            if (value == null) return BTExecuteStatus.Ready;
            return value;
        }

        public void Clear()
        {
            lstData.Clear();
            lstExecuteStatus.Clear();
        }
    }
}