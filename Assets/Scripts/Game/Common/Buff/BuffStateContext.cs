using Framework;
using Unity.Entities;

namespace Game
{
    public class BuffStateContext : IPoolable
    {
        private World m_world;
        public World world
        {
            get { return m_world; }
        }

        private BuffStateComponent m_cBuffStateComponent;
        public BuffStateComponent buffStateComponent
        {
            get { return m_cBuffStateComponent; }
        }

        private BuffStateData m_buffStateData;
        public BuffStateData buffStateData
        {
            get { return m_buffStateData; }
        }

        private BuffStateConfig m_buffStateConfig;
        public BuffStateConfig buffStateConfig
        {
            get { return m_buffStateConfig; }
        }

        public void Init(World world, BuffStateComponent buffStateComponent, BuffStateData buffStateData, BuffStateConfig cfg)
        {
            this.m_world = world;
            this.m_cBuffStateComponent = buffStateComponent;
            this.m_buffStateData = buffStateData;
            this.m_buffStateConfig = cfg;
        }

        public void Reset()
        {
            m_world = null;
            this.m_cBuffStateComponent = null;
            this.m_buffStateData = null;
            this.m_buffStateConfig = null;
        }
    }
}