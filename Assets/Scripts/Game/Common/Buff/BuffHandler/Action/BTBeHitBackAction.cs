using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using GameData;
using NodeEditor;
using UnityEngine;

namespace Game
{
    public enum BeHitBackType
    {
        ColliderDirect,
        ColliderCenterToTarget,
        HostToTarget,
    }
    public class BeHitBackActionData
    {
        public BeHitBackType backType;
        //格式[time,x,y,z,xr,yr,zr,time,x,y,z,xr,yr,zr,...]
        public float[] movePoints;
        [NEProperty("位移距离，<=0表示使用默认配置距离，主要用于动画编辑完成之后，又需要调整动画距离但不调整动画曲线，一般情况下填-1")]
        public float distance = -1;

        public bool chageFace = false;
        [NonSerialized]
        public float[] runtimeMovePoints = null;
    }
    public class BTBeHitBackAction : BTAction<BuffBTContext, BeHitBackActionData>
    {
        private static string defaultBehit = "Hit_Front";
        class BeHitBackAnimInfo
        {
            //与forward的夹角
            public float startAngle;
            public float endAngle;
        }
        private static Dictionary<string, BeHitBackAnimInfo> dicCfg = new Dictionary<string, BeHitBackAnimInfo>()
        {
            { "Hit_Back", new BeHitBackAnimInfo(){startAngle = 315,endAngle = 45}},
            { "Hit_Right", new BeHitBackAnimInfo(){startAngle = 45,endAngle = 135}},
            { "Hit_Front", new BeHitBackAnimInfo(){startAngle = 135,endAngle = 225}},
            { "Hit_Left", new BeHitBackAnimInfo(){startAngle = 225,endAngle = 315}},
        };

        internal protected struct InnerBeHitActionData
        {
            public float time;
            public Quaternion startRotation;
            public Vector3 direct;
            public string anim;
        }

        private static InnerBeHitActionData DefaultActionData = new InnerBeHitActionData() { time = 0, startRotation = Quaternion.identity };

        protected override BTStatus Handler(BuffBTContext context, BTData btData, BeHitBackActionData data)
        {
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            var cacheData = context.executeCache.GetCache<InnerBeHitActionData>(btData.dataIndex, DefaultActionData);
            var cfg = ResCfgSys.Instance.GetCfg<ResBuff>(context.buffData.buffId);
            float duration = cfg.duration;
            if (exeStatus == BTExecuteStatus.Ready)
            {
                var entityTransformComponent = context.world.GetComponent<TransformComponent>(context.buffComponent.entity);
               
                context.executeCache.SetCache(btData.dataIndex, cacheData);

                if (data.movePoints != null && data.runtimeMovePoints == null)
                {
                    data.runtimeMovePoints = AnimationMoveUtility.GetPoints(data.movePoints, duration, data.distance);
                }

                var damageInfo = context.buffData.damageInfo;
                Quaternion startRotation = entityTransformComponent.rotation;
                if (damageInfo.hitInfo.IsEnabled())
                {
                    var direct = Vector3.zero;
                    if (data.backType == BeHitBackType.ColliderDirect)
                    {
                        direct = damageInfo.hitInfo.direct;
                        
                    }
                    else if (data.backType == BeHitBackType.ColliderCenterToTarget)
                    {
                        direct = damageInfo.hitInfo.rayDirect;
                    }
                    else if(data.backType == BeHitBackType.HostToTarget)
                    {
                        var targetTransform = context.world.GetComponent<TransformComponent>(damageInfo.target);
                        var casterTransform = context.world.GetComponent<TransformComponent>(damageInfo.caster);
                        direct = targetTransform.position - casterTransform.position;
                    }
                    direct.y = 0;
                    if (direct != Vector3.zero)
                        startRotation = Quaternion.LookRotation(direct);
                }
              
                var hostTransform = context.world.GetComponent<TransformComponent>(context.buffComponent.entity);
                float angle = Quaternion.Angle(hostTransform.rotation, startRotation);
                angle = Mathf.Repeat(angle, 360f);
                string behitAnim = defaultBehit;
                foreach (var pair in dicCfg)
                {
                    float start = 0;
                    float end = Mathf.Repeat(pair.Value.endAngle - pair.Value.startAngle, 360);
                    float v = Mathf.Repeat(angle - pair.Value.startAngle, 360);
                    if (start <= v && v <= end)
                    {
                        behitAnim = pair.Key;
                        break;
                    }
                }

                cacheData.startRotation = startRotation;
                cacheData.direct = startRotation * Vector3.forward;
                cacheData.direct.Normalize();
                cacheData.anim = behitAnim;

                //播放buff动画
                if (!string.IsNullOrEmpty(behitAnim))
                {
                    var animationSystem = context.world.GetExistingSystem<AnimationSystem>();
                    var paramType = animationSystem.GetAnimationParamType(context.buffComponent.entity, behitAnim);
                    if (paramType != null)
                    {
                        if (paramType.type == AnimatorControllerParameterType.Trigger)
                        {
                            animationSystem.ResetAnimatorParam(context.buffComponent.entity, behitAnim);
                            animationSystem.SetAnimatorParam(context.buffComponent.entity, behitAnim);
                            if (animationSystem.HasAnimatorParam(context.buffComponent.entity, behitAnim + "_Bool"))
                            {
                                animationSystem.SetAnimatorParam(context.buffComponent.entity, behitAnim + "_Bool", true);
                            }
                        }
                        else if (paramType.type == AnimatorControllerParameterType.Bool)
                        {
                            animationSystem.SetAnimatorParam(context.buffComponent.entity, behitAnim, true);
                        }
                    }
                }

            }
            float startTime = cacheData.time;
            cacheData.time = cacheData.time + context.deltaTime;
            float endTime = cacheData.time;
            var points = data.runtimeMovePoints;
            if (points == null || points.Length <= AnimationMoveUtility.DataSpace)
            {
                //没有配置距离，说明没有位移，直接等待buff结束
                if (data.distance > 0)
                {
                    var velocity = cacheData.direct * data.distance / duration;
                    var stepMoveSystem = context.world.GetExistingSystem<StepMoveSystem>();
                    stepMoveSystem.AppendSingleFrameVelocity(context.buffComponent.entity, velocity, false);
                }
            }
            else
            {
                var offsetInfo = AnimationMoveUtility.GetOffset(points, cacheData.startRotation, startTime, endTime);
                Vector3 velocity = offsetInfo.offsetPos / Time.deltaTime;
                var stepMoveSystem = context.world.GetExistingSystem<StepMoveSystem>();
                stepMoveSystem.AppendSingleFrameVelocity(context.buffComponent.entity, velocity, false);
                var faceSystem = context.world.GetExistingSystem<FaceSystem>();
                var transformComponent = context.world.GetComponent<TransformComponent>(context.buffComponent.entity);
                faceSystem.FaceTo(context.buffComponent.entity, offsetInfo.offsetRot * transformComponent.rotation, true);
            }
            context.executeCache.SetCache(btData.dataIndex, cacheData);
            if (cacheData.time >= duration)
            {
                return BTStatus.Success;
            }
            return BTStatus.Running;
        }

        protected override void Clear(BuffBTContext context, BTData btData, BeHitBackActionData data)
        {
            if (!IsCleared(context, btData))
            {
                var cacheData = context.executeCache.GetCache<InnerBeHitActionData>(btData.dataIndex, DefaultActionData);
                var anim = cacheData.anim;
                if (!string.IsNullOrEmpty(anim))
                {
                    var animationSystem = context.world.GetExistingSystem<AnimationSystem>();
                    var paramType = animationSystem.GetAnimationParamType(context.buffComponent.entity, anim);
                    if (paramType != null)
                    {
                        if (paramType.type == AnimatorControllerParameterType.Trigger)
                        {
                            animationSystem.ResetAnimatorParam(context.buffComponent.entity, anim);
                            if (animationSystem.HasAnimatorParam(context.buffComponent.entity, anim + "_Bool"))
                            {
                                animationSystem.SetAnimatorParam(context.buffComponent.entity, anim + "_Bool", false);
                            }

                        }
                        else if (paramType.type == AnimatorControllerParameterType.Bool)
                        {
                            animationSystem.SetAnimatorParam(context.buffComponent.entity, anim, false);
                        }
                    }
                }
            }
            base.Clear(context, btData, data);
        }
    }

}

