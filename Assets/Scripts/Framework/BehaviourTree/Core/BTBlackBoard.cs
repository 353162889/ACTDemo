using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public T GetData<T>(string name)
        {
            object result;
            if (m_cacheData.TryGetValue(name, out result))
            {
                return (T) result;
            }
            return default(T);
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
