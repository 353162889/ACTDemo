namespace Framework
{
    public abstract class UtilityBase<T> : IUtility where T : IUtilityData
    {
        private IUtilityAI m_cUtilityAI;
        public IUtilityAI utilityAI
        {
            get { return m_cUtilityAI; }
        }

        private IUtilityData m_cUtilityData;
        public IUtilityData utilityData
        {
            get { return m_cUtilityData; }
        }

        public T convertData
        {
            get { return (T)m_cUtilityData; }
        }

        public void Init(IUtilityAI utilityAI, IUtilityData data)
        {
            m_cUtilityAI = utilityAI;
            if (data == null)
            {
                CLog.LogError(this.GetType() + "data is null!");
                return;
            }
            m_cUtilityData = data;
            OnInit(convertData);
        }

        protected abstract void OnInit(T data);
    }
}