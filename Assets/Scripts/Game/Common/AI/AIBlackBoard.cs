
using BTCore;
using Unity.Entities;

namespace Game
{
    public class AIBlackBoard : BTBlackBoard
    {
        //通用目标,AI目标通过filter策略选出
        private Entity m_sFilterTarget = Entity.Null;
        //AI中逻辑需要特殊设置的目标
        private Entity m_sOverrideTarget = Entity.Null;

        public Entity target
        {
            get
            {
                if (m_sOverrideTarget != Entity.Null) return m_sOverrideTarget;
                return m_sFilterTarget;
            }
        }

        public void SetFilterTarget(Entity target)
        {
            m_sFilterTarget = target;
        }

        public void SetOverrideTarget(Entity target)
        {
            m_sOverrideTarget = target;
        }

        public void ResetTarget()
        {
            m_sFilterTarget = Entity.Null;
            m_sOverrideTarget = Entity.Null;
        }

        public override void Clear()
        {
            m_sFilterTarget = Entity.Null;
            m_sOverrideTarget = Entity.Null;
            base.Clear();
        }
    }
}