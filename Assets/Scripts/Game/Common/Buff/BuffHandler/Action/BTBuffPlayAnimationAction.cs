using System;
using Framework;
using GameData;
using UnityEngine;

namespace Game
{
    public class BTBuffPlayAnimationActionData
    {
        public string animName;
    }
    public class BTBuffPlayAnimationAction : BTAction<BuffBTContext, BTBuffPlayAnimationActionData>
    {
        internal protected struct InnerBuffPlayAnimationActionData
        {
            public float time;
        }

        private static InnerBuffPlayAnimationActionData DefaultActionData = new InnerBuffPlayAnimationActionData() { time = 0 };

        protected override BTStatus Handler(BuffBTContext context, BTData btData, BTBuffPlayAnimationActionData data)
        {
            var cacheData = context.executeCache.GetCache<InnerBuffPlayAnimationActionData>(btData.dataIndex, DefaultActionData);
            if (context.executeCache.GetExecuteStatus(btData.dataIndex) == BTExecuteStatus.Ready)
            {
                //播放buff动画
                if (!string.IsNullOrEmpty(data.animName))
                {
                    var animationSystem = context.world.GetExistingSystem<AnimationSystem>();
                    var paramType = animationSystem.GetAnimationParamType(context.buffComponent.componentEntity, data.animName);
                    if (paramType != null)
                    {
                        if (paramType.type == AnimatorControllerParameterType.Trigger)
                        {
                            animationSystem.ResetAnimatorParam(context.buffComponent.componentEntity, data.animName);
                            animationSystem.SetAnimatorParam(context.buffComponent.componentEntity, data.animName);
                            if (animationSystem.HasAnimatorParam(context.buffComponent.componentEntity, data.animName + "_Bool"))
                            {
                                animationSystem.SetAnimatorParam(context.buffComponent.componentEntity, data.animName + "_Bool", true);
                            }
                        }
                        else if (paramType.type == AnimatorControllerParameterType.Bool)
                        {
                            animationSystem.SetAnimatorParam(context.buffComponent.componentEntity, data.animName, true);
                        }
                    }
                }
            }
            var cfg = ResCfgSys.Instance.GetCfg<ResBuff>(context.buffData.buffId);
            float duration = cfg.duration;
            cacheData.time = cacheData.time + context.deltaTime;
            context.executeCache.SetCache(btData.dataIndex, cacheData);
            if (cacheData.time < duration)
            {
                return BTStatus.Running;
            }
            else
            {
                return BTStatus.Success;
            }
        }

        protected override void Clear(BuffBTContext context, BTData btData, BTBuffPlayAnimationActionData data)
        {
            if (!IsCleared(context, btData))
            {
                if (!string.IsNullOrEmpty(data.animName))
                {
                    var animationSystem = context.world.GetExistingSystem<AnimationSystem>();
                    var paramType = animationSystem.GetAnimationParamType(context.buffComponent.componentEntity, data.animName);
                    if (paramType != null)
                    {
                        if (paramType.type == AnimatorControllerParameterType.Trigger)
                        {
                            animationSystem.ResetAnimatorParam(context.buffComponent.componentEntity, data.animName);
                            if (animationSystem.HasAnimatorParam(context.buffComponent.componentEntity, data.animName + "_Bool"))
                            {
                                animationSystem.SetAnimatorParam(context.buffComponent.componentEntity, data.animName + "_Bool", false);
                            }

                        }
                        else if (paramType.type == AnimatorControllerParameterType.Bool)
                        {
                            animationSystem.SetAnimatorParam(context.buffComponent.componentEntity, data.animName, false);
                        }
                    }
                }
            }
            base.Clear(context, btData, data);
        }
    }
}