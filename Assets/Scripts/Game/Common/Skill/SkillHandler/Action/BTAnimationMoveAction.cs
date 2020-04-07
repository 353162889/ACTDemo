using System;
using Framework;
using NodeEditor;
using UnityEngine;

namespace Game
{
    public class BTAnimationMoveActionData
    {
        //格式[time,x,y,z,xr,yr,zr,time,x,y,z,xr,yr,zr,...]
        public float[] movePoints;
        [NEProperty("位移时间，<=0表示使用默认配置时间")]
        public float duration = -1;
        [NEProperty("位移距离，<=0表示使用默认配置距离，主要用于动画编辑完成之后，又需要调整动画距离但不调整动画曲线，一般情况下填-1")]
        public float distance = -1;
        [NonSerialized]
        public float[] runtimeMovePoints = null;
    }
    public class BTAnimationMoveAction : BTAction<SkillBTContext, BTAnimationMoveActionData>
    {
        internal protected struct InnerBTAnimationMoveActionData
        {
            public float time;
            public Quaternion startRotation;
        }

        private static InnerBTAnimationMoveActionData DefaultActionData = new InnerBTAnimationMoveActionData() { time = 0, startRotation = Quaternion.identity};

        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTAnimationMoveActionData data)
        {
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                var curCacheData = context.executeCache.GetCache<InnerBTAnimationMoveActionData>(btData.dataIndex, DefaultActionData);
                var entityTransformComponent = context.world.GetComponent<TransformComponent>(context.skillComponent.componentEntity);
                curCacheData.startRotation = entityTransformComponent.rotation;
                context.executeCache.SetCache(btData.dataIndex, curCacheData);

                if (data.movePoints != null && data.runtimeMovePoints == null)
                {
                    data.runtimeMovePoints = AnimationMoveUtility.GetPoints(data.movePoints, data.duration, data.distance);
                }
            }

            var cacheData = context.executeCache.GetCache<InnerBTAnimationMoveActionData>(btData.dataIndex, DefaultActionData);
            var points = data.runtimeMovePoints;
            if (points == null || points.Length <= AnimationMoveUtility.DataSpace)
            {
                this.Clear(context, btData);
                return BTStatus.Success;
            }
            float startTime = cacheData.time;
            cacheData.time = cacheData.time + context.deltaTime;
            float endTime = cacheData.time;
            var offsetInfo = AnimationMoveUtility.GetOffset(points, cacheData.startRotation, startTime, endTime);
            Vector3 velocity = offsetInfo.offsetPos / Time.deltaTime;
            var stepMoveSystem = context.world.GetExistingSystem<StepMoveSystem>();
            stepMoveSystem.AppendSingleFrameVelocity(context.skillComponent.componentEntity, velocity, false);
            var faceSystem = context.world.GetExistingSystem<FaceSystem>();
            var transformComponent = context.world.GetComponent<TransformComponent>(context.skillComponent.componentEntity);
            faceSystem.FaceTo(context.skillComponent.componentEntity, offsetInfo.offsetRot * transformComponent.rotation, true);

            if (cacheData.time >= points[points.Length - AnimationMoveUtility.DataSpace])
            {
                return BTStatus.Success;
            }

            context.executeCache.SetCache(btData.dataIndex, cacheData);
            return BTStatus.Running;
        }
    }
}