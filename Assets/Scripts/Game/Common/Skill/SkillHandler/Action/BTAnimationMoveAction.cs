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
        //是否忽略旋转
        public bool ignoreRotation = false;
        //是否使用初始方向（即在移动过程中如果面向改变了，移动路径是否已当前面向作为参考）
        public bool useStartRotation = true;
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
            float startTime = cacheData.time;
            cacheData.time = cacheData.time + context.deltaTime;
            float endTime = cacheData.time;
           
            var points = data.runtimeMovePoints;
            if (points == null || points.Length <= AnimationMoveUtility.DataSpace)
            {
                return BTStatus.Success;
            }
            else
            {
                var stepMoveSystem = context.world.GetExistingSystem<StepMoveSystem>();
                Quaternion startRotation;
                if (data.useStartRotation)
                {
                    startRotation = cacheData.startRotation;
                }
                else
                {
                    var transformComponent =
                        context.world.GetComponent<TransformComponent>(context.skillComponent.componentEntity);
                    startRotation = transformComponent.rotation;
                }

                stepMoveSystem.AnimationMove(context.skillComponent.componentEntity, points, startTime,
                    context.deltaTime, startRotation, data.ignoreRotation);
                if (endTime >= points[points.Length - AnimationMoveUtility.DataSpace])
                {
                    return BTStatus.Success;
                }
            }

            context.executeCache.SetCache(btData.dataIndex, cacheData);
            return BTStatus.Running;
        }
    }
}