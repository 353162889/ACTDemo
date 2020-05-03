using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using GameData;
using NodeEditor;
using UnityEngine;

namespace Game
{
    public class BTBuffMoveActionData
    {
        public BuffDirectionType directionType = BuffDirectionType.HostDirect;
        [NEProperty("位移距离，<=0表示使用默认配置动画距离")]
        public float distance = -1;
        //格式[time,x,y,z,xr,yr,zr,time,x,y,z,xr,yr,zr,...]
        public float[] movePoints;
        //是否忽略旋转
        public bool ignoreRotation = false;
        //是否使用初始方向（即在移动过程中如果面向改变了，移动路径是否已当前面向作为参考）
        public bool useStartRotation = true;
        [NonSerialized]
        public float[] runtimeMovePoints = null;
    }
    public class BTBuffMoveAction : BTAction<BuffBTContext, BTBuffMoveActionData>
    {
        internal protected struct InnerBTBuffMoveActionData
        {
            public float time;
            public Quaternion startRotation;
            public Vector3 startDirect;
        }

        private static InnerBTBuffMoveActionData DefaultActionData = new InnerBTBuffMoveActionData() { time = 0, startRotation = Quaternion.identity };

        protected override BTStatus Handler(BuffBTContext context, BTData btData, BTBuffMoveActionData data)
        {
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            var cfg = ResCfgSys.Instance.GetCfg<ResBuff>(context.buffData.buffId);
            float duration = cfg.duration;
            float distance = data.distance;
            float overrideDistance = context.blackBoard.GetData<float>(BuffBlackBoardKeys.OverrideDistance);
            if (overrideDistance > 0) distance = overrideDistance;
            if (exeStatus == BTExecuteStatus.Ready)
            {
                var curCacheData = context.executeCache.GetCache<InnerBTBuffMoveActionData>(btData.dataIndex, DefaultActionData);
                var direction = BuffHandlerUtility.GetBuffDirection(context, data.directionType);
                curCacheData.startDirect = direction;
                curCacheData.startRotation = Quaternion.LookRotation(curCacheData.startDirect);
                context.executeCache.SetCache(btData.dataIndex, curCacheData);
                if (data.movePoints != null && data.runtimeMovePoints == null)
                {
                    data.runtimeMovePoints = AnimationMoveUtility.GetPoints(data.movePoints, duration, distance);
                }
            }

            var cacheData = context.executeCache.GetCache<InnerBTBuffMoveActionData>(btData.dataIndex, DefaultActionData);
            float startTime = cacheData.time;
            cacheData.time = cacheData.time + context.deltaTime;
            float endTime = cacheData.time;
            var points = data.runtimeMovePoints;
            if (points == null || points.Length <= AnimationMoveUtility.DataSpace)
            {
                //没有配置距离，说明没有位移，直接等待buff结束
                if (distance > 0)
                {
                    var velocity = cacheData.startDirect * distance / duration;
                    var stepMoveSystem = context.world.GetExistingSystem<StepMoveSystem>();
                    stepMoveSystem.AppendSingleFrameVelocity(context.buffComponent.componentEntity, velocity, false);
                }
                else
                {
                    return BTStatus.Success;
                }
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
                        context.world.GetComponent<TransformComponent>(context.buffComponent.componentEntity);
                    startRotation = transformComponent.rotation;
                }

                stepMoveSystem.AnimationMove(context.buffComponent.componentEntity, points, startTime,
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

