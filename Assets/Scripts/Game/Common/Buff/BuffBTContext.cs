using BTCore;
using Framework;
using Unity.Entities;

namespace Game
{
    public class BuffBTContext : IBTContext, IPoolable
    {
        private World m_world;

        public World world
        {
            get { return m_world; }
        }

        private BTTreeData m_btTreeData;

        public BTTreeData treeData
        {
            get { return m_btTreeData; }
        }

        private IBTExecutor m_executor;

        public IBTExecutor executor
        {
            get { return m_executor; }
        }

        private BuffComponent m_buffComponent;

        public BuffComponent buffComponent
        {
            get { return m_buffComponent; }
        }

        private BuffData m_buffData;

        public BuffData buffData
        {
            get { return m_buffData; }
        }

        public BTBlackBoard blackBoard
        {
            get { return buffData.blackBoard; }
        }

        public BTExecuteCache executeCache
        {
            get { return buffData.executeCache; }
        }

        public IBTDataHandler GetHandler(int index)
        {
            return BTDataHandlerInitialize.GetHandler(index);
        }

        private float m_fDeltaTime;

        public float deltaTime
        {
            get { return m_fDeltaTime; }
        }


        public void Init(World world, BuffComponent buffComponent, BuffData buffData, BTTreeData btTreeData,
            IBTExecutor executor, float deltaTime)
        {
            this.m_world = world;
            this.m_buffComponent = buffComponent;
            this.m_buffData = buffData;
            this.m_btTreeData = btTreeData;
            this.m_executor = executor;
            this.m_fDeltaTime = deltaTime;
        }

        private BuffPartData m_cBuffPartData;
        public void SetPartData(BuffPartData partData)
        {
            this.m_cBuffPartData = partData;
        }

        public BuffPartData GetPartData()
        {
            return m_cBuffPartData;
        }

        public void Reset()
        {
            this.m_world = null;
            this.m_buffComponent = null;
            this.m_btTreeData = null;
            this.m_executor = null;
            this.m_buffData = null;
            this.m_cBuffPartData = null;
            this.m_fDeltaTime = 0;
        }
    }
}