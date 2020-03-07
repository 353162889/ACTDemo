using System;
using Framework;

namespace Game
{
    public class BTPlayAnimationActionData
    {
        public float duration;
        public string animName;
    }
    public class BTPlayAnimationAction : BTAction<SkillBTContext, BTPlayAnimationActionData>
    {
        internal protected struct InnerPlayAnimationActionData
        {
            public float time;
        }

        private static InnerPlayAnimationActionData DefaultActionData = new InnerPlayAnimationActionData(){time=0};

        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTPlayAnimationActionData data)
        {
            var cacheData = context.executeCache.GetCache<InnerPlayAnimationActionData>(btData.dataIndex, DefaultActionData);
            if (context.executeCache.GetExecuteStatus(btData.dataIndex) == BTExecuteStatus.Ready)
            {
                var animationSystem = context.world.GetExistingSystem<AnimationSystem>();
                animationSystem.SetAnimatorParam(context.skillComponent.entity, data.animName);
            }

            cacheData.time = cacheData.time + context.deltaTime;
            context.executeCache.SetCache(btData.dataIndex, cacheData);
            if (cacheData.time < data.duration)
            {
                return BTStatus.Running;
            }
            else
            {
                this.Clear(context, btData);
                return BTStatus.Success;
            }
        }
    }
}