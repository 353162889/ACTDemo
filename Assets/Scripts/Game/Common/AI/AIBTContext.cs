using BTCore;
using Framework;
using Unity.Entities;

namespace Game
{
    public class AIBTContext : IBTContext,IPoolable
    {
        private string m_aiFile;
        public string aiFile
        {
            get { return m_aiFile; }
        }

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
            get { return m_aiBlackBoard; }
        }

        private AIBlackBoard m_aiBlackBoard = new AIBlackBoard();
        public AIBlackBoard aiBlackBoard
        {
            get { return m_aiBlackBoard; }
        }

        private BTExecuteCache m_executeCache = new BTExecuteCache();
        public BTExecuteCache executeCache
        {
            get { return m_executeCache; }
        }

        public IBTDataHandler GetHandler(int index)
        {
            return BTDataHandlerInitialize.GetHandler(index);
        }

        public AIWorldState worldState
        {
            get { return m_aiComponent.worldState; }
        }

        private float m_fDeltaTime = 0f;
        public float deltaTime
        {
            get { return m_fDeltaTime; }
        }

        public void Init(string aiFile, World world, AIComponent aiComponent, BTTreeData btTreeData, IBTExecutor executor)
        {
            Reset();
            this.m_aiFile = aiFile;
            this.m_world = world;
            this.m_aiComponent = aiComponent;
            this.m_btTreeData = btTreeData;
            this.m_executor = executor;
        }

        public void SetDeltaTime(float deltaTime)
        {
            this.m_fDeltaTime = deltaTime;
        }

        public void Reset()
        {
            ClearCacheData();
            this.m_aiFile = "";
            this.m_world = null;
            this.m_aiComponent = null;
            this.m_btTreeData = null;
            this.m_executor = null;
            this.m_fDeltaTime = 0;
            m_executeCache.Clear();
            m_aiBlackBoard.Clear();
        }

        public void ClearCacheData()
        {
            if (treeData != null && treeData.rootData != null && treeData.rootData.children.Count > 0)
            {
                var childBtData = treeData.rootData.children[0];
                BTDataHandlerInitialize.GetHandler(childBtData.keyIndex).Clear(this, childBtData);
            }
        }
    }
}