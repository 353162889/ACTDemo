using Framework;
using Unity.Entities;

namespace Game
{
    public class UtilityContext : IUtilityContext
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


        private AIComponent m_aiComponent;
        public AIComponent aiComponent
        {
            get { return m_aiComponent; }
        }

        private UtilityAI m_utilityAI;
        public UtilityAI utilityAI
        {
            get { return m_utilityAI; }
        }

        public AIWorldState worldState
        {
            get { return m_aiComponent.worldState; }
        }

        public void Init(string aiFile, World world, AIComponent aiComponent, UtilityAI utilityAI)
        {
            this.Reset();
            this.m_aiFile = aiFile;
            this.m_world = world;
            this.m_aiComponent = aiComponent;
            this.m_utilityAI = utilityAI;

        }

        public void ReloadUtilityAI(UtilityAI reloadUtilityAI)
        {
            if (this.m_utilityAI != null)
            {
                reloadUtilityAI.SetDebug(this.m_utilityAI.isDebug);
            }
            this.m_utilityAI = reloadUtilityAI;
        }

        public void Reset()
        {
            this.m_aiFile = null;
            this.m_world = null;
            this.m_aiComponent = null;
            this.m_utilityAI = null;
        }
    }
}