using Framework;
using Unity.Entities;

namespace Game
{
    public class UtilityContext : IUtilityContext
    {
        public Entity entity
        {
            get { return utilityAIComponent.componentEntity; }
        }
        public UtilityAIComponent utilityAIComponent { get; private set; }

        /// <summary>
        /// 目标参数（0表示有目标，1表示无目标）
        /// </summary>
        public float targetFactor { get; set; }

        public float targetDistance { get; set; }

        public UtilityContext(UtilityAIComponent utilityAIComponent)
        {
            this.utilityAIComponent = utilityAIComponent;
        }
    }
}