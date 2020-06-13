using BTCore;
using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class SkillBTContext : IBTContext,IPoolable
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

        public BTSkillRootData skillRootData
        {
            get { return SkillManager.Instance.GetSkillRootData(m_btTreeData); }
        }


        private IBTExecutor m_executor;
        public IBTExecutor executor
        {
            get { return m_executor; }
        }

        private SkillComponent m_skillComponent;
        public SkillComponent skillComponent
        {
            get { return m_skillComponent; }
        }

        private SkillData m_skillData;
        public SkillData skillData
        {
            get { return m_skillData; }
        }

        public BTBlackBoard blackBoard
        {
            get { return skillComponent.skillData.blackBoard; }
        }

        public BTExecuteCache executeCache
        {
            get { return skillComponent.skillData.executeCache; }
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

        public void Init(World world, SkillComponent skillComponent,SkillData skillData, BTTreeData btTreeData, IBTExecutor executor, float deltaTime)
        {
            this.m_world = world;
            this.m_skillComponent = skillComponent;
            this.m_skillData = skillData;
            this.m_btTreeData = btTreeData;
            this.m_executor = executor;
            this.m_fDeltaTime = deltaTime;
        }

        public void Reset()
        {
            this.m_world = null;
            this.m_skillComponent = null;
            this.m_btTreeData = null;
            this.m_executor = null;
            this.m_skillData = null;
            this.m_fDeltaTime = 0;
        }
    }
}