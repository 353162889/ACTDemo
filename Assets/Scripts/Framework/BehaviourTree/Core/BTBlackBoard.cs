using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;

namespace BTCore
{
    public class BTBlackBoard
    {
        public float deltaTime { get; set; }
        protected Dictionary<string, object> m_cacheData;
        public BTBlackBoard()
        {
            m_cacheData = new Dictionary<string, object>();
        }

        public void SetData(string name, object data)
        {
            m_cacheData[name] = data;
        }

        public T GetData<T>(string name, T defaultValue = default(T))
        {
            object result;
            if (m_cacheData.TryGetValue(name, out result))
            {
                return (T) result;
            }
            return defaultValue;
        }

        public T GetAndSetData<T>(string name, ObjectFactory<T> defaultFactory) where T :new()
        {
            object result;
            if (m_cacheData.TryGetValue(name, out result))
            {
                return (T)result;
            }

            var value = defaultFactory.Generate();
            m_cacheData.Add(name, value);
            return value;
        }

        public bool GetData<T>(string name, out T result)
        {
            object value = null;
            if (m_cacheData.TryGetValue(name, out value) && value != null)
            {
                result = (T) value;
                return true;
            }

            result = default(T);
            return false;
        }

        public void ClearData(string name)
        {
            m_cacheData.Remove(name);
        }

        public virtual void Clear()
        {
            deltaTime = 0;
            m_cacheData.Clear();
        }
    }
}
