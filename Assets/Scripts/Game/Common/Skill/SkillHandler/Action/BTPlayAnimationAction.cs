using System;
using Framework;

namespace Game
{
    public class BTPlayAnimationActionData : IBTTimelineDurationData
    {
        public float animDuration;
        public string animName;
        public float duration
        {
            get { return animDuration; }
            set { animDuration = value; } }
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
                animationSystem.ResetAnimatorParam(context.skillComponent.entity, "BreakSkill");
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
                return BTStatus.Success;
            }
        }

        protected override void Clear(SkillBTContext context, BTData btData, BTPlayAnimationActionData data)
        {
            if (!IsCleared(context, btData))
            {
                //重置动画名称
                var animationSystem = context.world.GetExistingSystem<AnimationSystem>();
                animationSystem.ResetAnimatorParam(context.skillComponent.entity, data.animName);
                if (context.skillData.isBreak)
                {
                    animationSystem.SetAnimatorParam(context.skillComponent.entity, "BreakSkill");
                }
            }
            base.Clear(context, btData, data);
        }
    }
}