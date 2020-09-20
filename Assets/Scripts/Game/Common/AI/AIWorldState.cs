using Unity.Entities;

namespace Game
{
    public class AIWorldState
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

        /// <summary>
        /// 目标参数（0表示有目标，1表示无目标）
        /// </summary>
        public float targetFactor { get; set; }

        /// <summary>
        /// 与目标的距离
        /// </summary>
        public float targetDistance { get; set; }

        /// <summary>
        /// 攻击欲望
        /// </summary>
        public float attackDesired { get; set; }

        public void ResetTarget()
        {
            m_sFilterTarget = Entity.Null;
            m_sOverrideTarget = Entity.Null;
        }

        public void Clear()
        {
            ResetTarget();
            targetFactor = 0;
            targetDistance = 0;
        }
    }
}