using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using GameData;
using NodeEditor;
using UnityEngine;

namespace Game
{
    public enum BuffMoveType
    {
        AnimationMove,
        Direction,
    }
    public class BTBuffMoveActionData
    {
        public BuffMoveType moveType;
        public Vector2 direction;
        [NEProperty("位移距离，<=0表示使用默认配置动画距离")]
        public float distance = -1;
        //格式[time,x,y,z,xr,yr,zr,time,x,y,z,xr,yr,zr,...]
        public float[] movePoints;
        //是否忽略旋转
        public bool ignoreRotation = false;
        //
        public bool useStartRotation = false;
        [NonSerialized]
        public float[] runtimeMovePoints = null;
    }
    public class BTBuffMoveAction : BTAction<BuffBTContext, BTBuffMoveActionData>
    {
        internal protected struct InnerBTBuffMoveActionData
        {
            public float time;
            public Quaternion startRotation;
        }

        private static InnerBTBuffMoveActionData DefaultActionData = new InnerBTBuffMoveActionData() { time = 0, startRotation = Quaternion.identity };

        protected override BTStatus Handler(BuffBTContext context, BTData btData, BTBuffMoveActionData data)
        {
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                var curCacheData = context.executeCache.GetCache<InnerBTBuffMoveActionData>(btData.dataIndex, DefaultActionData);
                var entityTransformComponent = context.world.GetComponent<TransformComponent>(context.buffComponent.componentEntity);
                curCacheData.startRotation = entityTransformComponent.rotation;
                context.executeCache.SetCache(btData.dataIndex, curCacheData);
                if (data.movePoints != null && data.runtimeMovePoints == null)
                {
                    var cfg = ResCfgSys.Instance.GetCfg<ResBuff>(context.buffData.buffId);
                    float duration = cfg.duration;
                    data.runtimeMovePoints = AnimationMoveUtility.GetPoints(data.movePoints, duration, data.distance);
                }
            }

            var cacheData = context.executeCache.GetCache<InnerBTBuffMoveActionData>(btData.dataIndex, DefaultActionData);
            var points = data.runtimeMovePoints;
            if (points == null || points.Length <= AnimationMoveUtility.DataSpace)
            {
                return BTStatus.Success;
            }

            float time = cacheData.time;
            cacheData.time = cacheData.time + context.deltaTime;
            var stepMoveSystem = context.world.GetExistingSystem<StepMoveSystem>();
            Quaternion startRotation;
            if (data.useStartRotation)
            {
                startRotation = cacheData.startRotation;
            }
            else
            {
                var transformComponent = context.world.GetComponent<TransformComponent>(context.buffComponent.componentEntity);
                startRotation = transformComponent.rotation;
            }
            stepMoveSystem.AnimationMove(context.buffComponent.componentEntity, points, time, context.deltaTime, startRotation, data.ignoreRotation);
            if (cacheData.time >= points[points.Length - AnimationMoveUtility.DataSpace])
            {
                return BTStatus.Success;
            }

            context.executeCache.SetCache(btData.dataIndex, cacheData);
            return BTStatus.Running;
        }
    }

}

