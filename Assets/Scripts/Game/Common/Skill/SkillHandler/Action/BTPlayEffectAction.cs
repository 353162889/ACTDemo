﻿using Framework;
using NodeEditor;
using UnityEngine;

namespace Game
{
    public class BTPlayEffectActionData
    {
        //特效名称
        public string effectName;
        //挂点
        public string mountPoint;
        //局部相对位置
        public Vector3 localPos;
        //局部旋转
        public Quaternion localRot;
        //是否自动销毁
        public bool autoDestroy;
        //播放特效时间,autoDestroy为false时生效
        [NEProperty("播放特效时间,autoDestroy为false时生效")]
        public float duration;
    }
    public class BTPlayEffectAction : BTAction<SkillBTContext, BTPlayEffectActionData>
    {
        internal protected struct InnerBTPlayEffectActionData
        {
            public float time;
            public GameObject effectGO;
        }

        private static InnerBTPlayEffectActionData DefaultActionData = new InnerBTPlayEffectActionData() { time = 0, effectGO = null};

        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTPlayEffectActionData data)
        {
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            var cacheData = context.executeCache.GetCache<InnerBTPlayEffectActionData>(btData.dataIndex, DefaultActionData);
            if (exeStatus == BTExecuteStatus.Ready && !string.IsNullOrEmpty(data.effectName))
            {
                var avatarSystem = context.world.GetOrCreateSystem<AvatarSystem>();
                Transform parent = null;
                if (!string.IsNullOrEmpty(data.mountPoint))
                {
                    parent = avatarSystem.GetMountPoint(context.skillComponent.entity, data.mountPoint);
                }

                var go = SceneEffectPool.Instance.CreateEffect(data.effectName, data.autoDestroy, parent);
                if (go != null)
                {
                    cacheData.effectGO = go;
                    if (parent == null)
                    {
                        var transformComponent = context.world.GetComponent<TransformComponent>(context.skillComponent.entity);
                        go.transform.position =
                            transformComponent.position + transformComponent.rotation * data.localPos;
                        go.transform.rotation = data.localRot;
                    }
                    else
                    {
                        go.transform.localPosition = data.localPos;
                        go.transform.localRotation = data.localRot;
                    }
                }
            }


            BTStatus result = BTStatus.Success;
            cacheData.time += context.deltaTime;
            context.executeCache.SetCache(btData.dataIndex, cacheData);
            if (!data.autoDestroy)
            {
                if (cacheData.time >= data.duration)
                {
                    if (cacheData.effectGO != null)
                    {
                        SceneEffectPool.Instance.DestroyEffectGO(cacheData.effectGO);
                        cacheData.effectGO = null;
                    }
                    this.Clear(context, btData);
                    result = BTStatus.Success;
                }
                else
                {
                    result = BTStatus.Running;
                }
            }
            return result;
        }
    }
}