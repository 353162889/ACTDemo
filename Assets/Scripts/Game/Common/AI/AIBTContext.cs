using BTCore;
using Framework;
using Unity.Entities;

namespace Game
{
    public class AIBTContext : IBTContext,IPoolable
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

        private AIComponent m_aiComponent;
        public AIComponent aiComponent
        {
            get { return m_aiComponent; }
        }

        public BTBlackBoard blackBoard
        {
            get { return aiComponent.blackBoard; }
        }

        public AIBlackBoard aiBlackBoard
        {
            get { return aiComponent.blackBoard as AIBlackBoard; }
        }

        public BTExecuteCache executeCache
        {
            get { return aiComponent.executeCache; }
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

        public void Init(World world, AIComponent aiComponent, BTTreeData btTreeData, IBTExecutor executor, float deltaTime)
        {
            this.m_world = world;
            this.m_aiComponent = aiComponent;
            this.m_btTreeData = btTreeData;
            this.m_executor = executor;
            this.m_fDeltaTime = deltaTime;
        }

        public void Reset()
        {
            this.m_world = null;
            this.m_aiComponent = null;
            this.m_btTreeData = null;
            this.m_executor = null;
            this.m_fDeltaTime = 0;
        }
    }
}