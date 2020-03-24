﻿using System.Collections.Generic;
using Framework;
using NodeEditor;
using Unity.Entities;

namespace Game
{
    public class BTAddBuffActionData : IBTTimelineDurationData
    {
        public int filterId;
        [NEProperty("是否完成当前行为还原该行为（移除添加的buff）")]
        public bool finishResume;
        public float addBuffDuration;
        public int[] lstBuffId;
        public float duration
        {
            get { return addBuffDuration;}
            set { addBuffDuration = value; }
        }
    }
    public class BTAddBuffAction : BTAction<SkillBTContext, BTAddBuffActionData>
    {
        internal protected struct InnerBTAddBuffActionData
        {
            public float time;
            public List<int> lstBuffIndex;
        }
        private static InnerBTAddBuffActionData DefaultActionData = new InnerBTAddBuffActionData() { time = 0 };
        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTAddBuffActionData data)
        {
            var cacheData =
                context.executeCache.GetCache<InnerBTAddBuffActionData>(btData.dataIndex, DefaultActionData);
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                var buffSystem = context.world.GetExistingSystem<BuffSystem>();
                if (data.lstBuffId != null)
                {
                    if (cacheData.lstBuffIndex == null)
                    {
                        cacheData.lstBuffIndex = ResetObjectPool<List<int>>.Instance.GetObject();
                    }

                    var lst = context.blackBoard.GetData<List<EntityHitInfo>>(SkillBlackBoardKeys.ListHitInfo);
                    if (lst != null)
                    {
                        var tempTargets = ResetObjectPool<List<Entity>>.Instance.GetObject();
                        var tempResults = ResetObjectPool<List<Entity>>.Instance.GetObject();
                        foreach (var entityHitInfo in lst)
                        {
                            tempTargets.Add(entityHitInfo.entity);
                        }
                        TargetFilter.Filter(data.filterId, context.world, context.skillComponent.entity, tempTargets, ref tempResults);
                        foreach (var entity in tempResults)
                        {
                            foreach (var entityHitInfo in lst)
                            {
                                if (entityHitInfo.entity == entity)
                                {
                                    for (int i = 0; i < data.lstBuffId.Length; i++)
                                    {
                                        int index = buffSystem.AddBuff(entityHitInfo.entity, data.lstBuffId[i]);
                                        if (index > 0)
                                            cacheData.lstBuffIndex.Add(index);
                                    }
                                }
                            }
                        }

                        ResetObjectPool<List<Entity>>.Instance.SaveObject(tempTargets);
                        ResetObjectPool<List<Entity>>.Instance.SaveObject(tempResults);

                    }
                }
            }

            BTStatus result;

            cacheData.time += context.deltaTime;
            context.executeCache.SetCache(btData.dataIndex, cacheData);

            if (cacheData.time >= data.duration || !data.finishResume)
            {
                result = BTStatus.Success;
            }
            else
            {
                result = BTStatus.Running;
            }
            return result;
        }

        protected override void Clear(SkillBTContext context, BTData btData, BTAddBuffActionData data)
        {
            if (!IsCleared(context, btData))
            {
                if (data.finishResume)
                {
                    var cacheData =
                        context.executeCache.GetCache<InnerBTAddBuffActionData>(btData.dataIndex, DefaultActionData);
                    if (cacheData.lstBuffIndex != null)
                    {
                        var buffSystem = context.world.GetExistingSystem<BuffSystem>();
                        for (int i = 0; i < cacheData.lstBuffIndex.Count; i++)
                        {
                            buffSystem.RemoveBuffByIndex(context.skillComponent.entity, cacheData.lstBuffIndex[i]);
                        }
                        ResetObjectPool<List<int>>.Instance.SaveObject(cacheData.lstBuffIndex);
                        cacheData.lstBuffIndex = null;
                    }
                }
            }
            base.Clear(context, btData, data);
        }
    }
}