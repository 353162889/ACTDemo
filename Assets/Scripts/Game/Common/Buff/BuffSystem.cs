using System.Collections.Generic;
using Framework;
using GameData;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Game
{
    public class BuffSystem : ComponentSystem,IBTExecutor
    {
        private BuffStateSystem buffStateSystem;
        protected override void OnCreate()
        {
            ObjectPool<BuffData>.Instance.Init(50);
            ObjectPool<BuffPartData>.Instance.Init(100);
            ResetObjectPool<List<BuffData>>.Instance.Init(3, (List<BuffData> lst) => { lst.Clear(); });
            buffStateSystem = World.GetOrCreateSystem<BuffStateSystem>();
            buffStateSystem.OnRemoveBuff += BuffStateSystemOnOnRemoveBuff;
        }

        protected override void OnDestroy()
        {
            buffStateSystem.OnRemoveBuff -= BuffStateSystemOnOnRemoveBuff;
        }

        private void BuffStateSystemOnOnRemoveBuff(Entity entity, int buffIndex)
        {
            RemoveBuffByIndex(entity, buffIndex);
        }

        public void TriggerBuffEvent(BuffComponent buffComponent, BuffData buffData, BuffEventType eventType)
        {
            foreach (var buffPartData in buffData.lstPart)
            {
                var buffPartCfg = ResCfgSys.Instance.GetCfg<ResBuffPartLogic>(buffPartData.buffPartId);
                if (BuffConfig.GetEventTypeByString(buffPartCfg.eventTriggerType) == eventType)
                {
                    EnableBuffPart(buffComponent, buffData, buffPartData);
                }
            }
        }

        public int AddBuff(Entity entity, int buffId)
        {
            var buffComponent = World.GetComponent<BuffComponent>(entity);
            if (buffComponent == null) return -1;
            var buffCfg = ResCfgSys.Instance.GetCfg<ResBuff>(buffId);
            if (buffCfg == null) return -1;
            var multiCount = buffCfg.multiCount;
            if (multiCount <= 0) return -1;
            CLog.LogArgs("添加buff", buffId);
            List<BuffData> lst;
            if (buffComponent.dicIdToBuffLst.TryGetValue(buffId, out lst))
            {
                int curCount = 0;
                bool exist = false;
                for (int i = lst.Count - 1; i > -1; i--)
                {
                    if (lst[i].status != BuffExeStatus.Destroy) curCount++;
                    if (curCount >= multiCount)
                    {
                        if (!RemoveBuffByIndex(entity, lst[i].index))
                        {
                            return -1;
                        }

                        exist = true;
                        break;
                    }
                }

                if (exist)
                {
                    for (int i = buffComponent.lstAdd.Count - 1; i > -1; i--)
                    {
                        if (lst[i].status != BuffExeStatus.Destroy) curCount++;
                        if (curCount >= multiCount)
                        {
                            if (!RemoveBuffByIndex(entity, lst[i].index))
                            {
                                return -1;
                            }
                            break;
                        }
                    }
                }
            }

            var buffData = ObjectPool<BuffData>.Instance.GetObject();
            buffData.index = buffComponent.startIndex++;
            buffData.buffId = buffId;
            buffData.status = BuffExeStatus.Init;
            for (int i = 0; i < buffCfg.parts.Count; i++)
            {
                var partId = buffCfg.parts[i];
                var buffPartData = ObjectPool<BuffPartData>.Instance.GetObject();
                buffPartData.buffPartId = partId;
                buffPartData.enabled = false;
                buffData.lstPart.Add(buffPartData);
            }

            buffComponent.lstAdd.Add(buffData);
            return buffData.index;
        }

        public bool RemoveBuffByIndex(Entity entity, int index)
        {
            var buffComponent = World.GetComponent<BuffComponent>(entity);
            if (buffComponent == null) return false;
            BuffData buffData;
            if (buffComponent.dicIndexToBuffData.TryGetValue(index, out buffData))
            {
                CLog.LogArgs("移除buff", buffData.buffId);
                buffData.Detach();
                return true;
            }

            for (int i = 0; i < buffComponent.lstAdd.Count; i++)
            {
                if (buffComponent.lstAdd[i].index == index)
                {
                    CLog.LogArgs("移除buff", buffComponent.lstAdd[i].buffId);
                    buffComponent.lstAdd[i].Detach();
                    return true;
                }
            }
            return false;
        }

        public bool RemoveBuffById(Entity entity, int buffId)
        {
            var buffComponent = World.GetComponent<BuffComponent>(entity);
            if (buffComponent == null) return false;
            bool result = false;
            List<BuffData> lst;
            if (buffComponent.dicIdToBuffLst.TryGetValue(buffId, out lst))
            {
                for (int i = 0; i < lst.Count; i++)
                {
                    lst[i].Detach();
                    result = true;
                }
            }

            for (int i = 0; i < buffComponent.lstAdd.Count; i++)
            {
                if (buffComponent.lstAdd[i].buffId == buffId)
                {
                    buffComponent.lstAdd[i].Detach();
                    result = true;
                }
            }
            return result;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, BuffComponent buffComponent) =>
            {
                //检测添加buff
                CheckAddBuff(buffComponent);
                foreach (var pair in buffComponent.dicIndexToBuffData)
                {
                    if (pair.Value.status != BuffExeStatus.Destroy)
                    {
                        //更新buff
                        UpdateBuff(buffComponent, pair.Value, false);
                    }
                    if(pair.Value.status == BuffExeStatus.Destroy)
                    {
                        TriggerBuffEvent(buffComponent, pair.Value, BuffEventType.HOST_BUFF_DETACH);
                        UpdateBuff(buffComponent, pair.Value, true);
                    }
                }
                //检测移除buff
                CheckRemoveBuff(buffComponent);
            });
        }

        private void CheckAddBuff(BuffComponent buffComponent)
        {
            for (int i = 0; i < buffComponent.lstAdd.Count; i++)
            {
                var buffData = buffComponent.lstAdd[i];
                if (buffData.status == BuffExeStatus.Init)
                {
                    List<BuffData> lst;
                    if (!buffComponent.dicIdToBuffLst.TryGetValue(buffData.buffId, out lst))
                    {
                        lst = new List<BuffData>();
                        buffComponent.dicIdToBuffLst.Add(buffData.buffId, lst);
                    }
                    lst.Add(buffData);
                    buffComponent.dicIndexToBuffData.Add(buffData.index, buffData);
                    TriggerBuffEvent(buffComponent, buffData, BuffEventType.HOST_BUFF_ATTACH);
                    var buffCfg = ResCfgSys.Instance.GetCfg<ResBuff>(buffData.buffId);
                    for (int j = 0; j < buffCfg.states.Count; i++)
                    {
                        var stateType = BuffStateConfig.GetStateTypeByString(buffCfg.states[i]);
                        if (stateType != BuffStateType.NONE)
                        {
                            int buffStateIndex = buffStateSystem.AddState(buffComponent.entity, stateType);
                            buffData.lstBuffStateIndex.Add(buffStateIndex);
                        }
                    }
                }
            }
            buffComponent.lstAdd.Clear();
        }

        private void CheckRemoveBuff(BuffComponent buffComponent)
        {
            var temp = ResetObjectPool<List<BuffData>>.Instance.GetObject();
            foreach (var pair in buffComponent.dicIndexToBuffData)
            {
                if (pair.Value.status == BuffExeStatus.Destroy)
                {
                    temp.Add(pair.Value);
                }
            }

            for (int i = 0; i < buffComponent.lstAdd.Count; i++)
            {
                if (buffComponent.lstAdd[i].status == BuffExeStatus.Destroy)
                {
                    temp.Add(buffComponent.lstAdd[i]);
                }
            }

            for (int i = 0; i < temp.Count; i++)
            {
                var buffData = temp[i];
                foreach (var buffPartData in buffData.lstPart)
                {
                    DisableBuffPart(buffComponent, buffData, buffPartData);
                }

                foreach (var index in buffData.lstBuffStateIndex)
                {
                    buffStateSystem.RemoveState(buffComponent.entity, index);
                }

                Clear(buffData.buffBTContext);
                buffData.lstBuffStateIndex.Clear();
                if (buffComponent.dicIndexToBuffData.Remove(buffData.index))
                {
                    List<BuffData> lst;
                    if (buffComponent.dicIdToBuffLst.TryGetValue(buffData.buffId, out lst))
                    {
                        lst.Remove(buffData);
                    }
                }
                else
                {
                    buffComponent.lstAdd.Remove(buffData);
                }

                foreach (var buffPartData in buffData.lstPart)
                {
                    ObjectPool<BuffPartData>.Instance.SaveObject(buffPartData);
                }
                buffData.lstPart.Clear();

                //调用detach方法
                ObjectPool<BuffData>.Instance.SaveObject(buffData);
            }
            ResetObjectPool<List<BuffData>>.Instance.SaveObject(temp);

        }

        private void UpdateBuff(BuffComponent buffComponent, BuffData buffData, bool destroy)
        {
            var buffCfg = ResCfgSys.Instance.GetCfg<ResBuff>(buffData.buffId);

            for (int i = 0; i < buffData.lstPart.Count; i++)
            {
                var partData = buffData.lstPart[i];
                if (partData.enabled)
                {
                    UpdateBuffPart(buffComponent, buffData, partData);
                }
            }

            if (!destroy)
            {
                if (!string.IsNullOrEmpty(buffCfg.script))
                {
                    //执行脚本
                    var btTreeData = BuffManager.Instance.GetBuffBTTreeData(buffData.buffId);
                    if (btTreeData == null)
                    {
                        buffData.Detach();
                        //如果没有逻辑，直接结束运行
                        return;
                    }

                    float deltaTime = Time.deltaTime;
                    buffData.buffBTContext.Reset();
                    buffData.buffBTContext.Init(World, buffComponent, buffData, btTreeData, this, deltaTime);
                    BTStatus btState = Execute(buffData.buffBTContext);
                    if (btState != BTStatus.Running)
                    {
                        buffData.Detach();
                    }
                }

                buffData.buffTime += Time.deltaTime;
                if (buffData.buffTime >= buffCfg.duration)
                {
                    buffData.Detach();
                }
            }
        }

        private void EnableBuffPart(BuffComponent buffComponent, BuffData buffData, BuffPartData buffPartData)
        {
            buffPartData.ResetEnabled(true);
            CLog.LogArgs("触发buff部位,buffId=", buffData.buffId, "partId=", buffPartData.buffPartId);
        }

        private void DisableBuffPart(BuffComponent buffComponent, BuffData buffData, BuffPartData buffPartData)
        {
            var buffPartCfg = ResCfgSys.Instance.GetCfg<ResBuffPartLogic>(buffPartData.buffPartId);
            RemoveProperty(buffComponent, buffData, buffPartData, buffPartCfg);
            buffPartData.ResetEnabled(false);
        }

        private void UpdateBuffPart(BuffComponent buffComponent, BuffData buffData, BuffPartData buffPartData)
        {
            if (!buffPartData.enabled) return;
            var buffPartCfg = ResCfgSys.Instance.GetCfg<ResBuffPartLogic>(buffPartData.buffPartId);
            if (buffPartCfg == null) return;
            float deltaTime = Time.deltaTime;
            buffPartData.curTriggerTime += deltaTime;
            if (buffPartData.curTriggerTime >= buffPartCfg.interval)
            {
                ExecuteBuffPart(buffComponent, buffData, buffPartData, buffPartCfg);
                buffPartData.curTriggerTime -= buffPartCfg.interval;
            }

            buffPartData.curDurationTime += deltaTime;
            if (buffPartData.curDurationTime >= buffPartCfg.duration)
            {
                DisableBuffPart(buffComponent, buffData, buffPartData);
            }
        }

        private void ExecuteBuffPart(BuffComponent buffComponent, BuffData buffData, BuffPartData buffPartData,
            ResBuffPartLogic buffPartCfg)
        {
            if (!buffPartData.enabled) return;
            if (buffPartData.curTriggerCount < buffPartCfg.eventTriggerCount)
            {
                buffPartData.curTriggerCount++;
                //更新属性
                AddProperty(buffComponent, buffData, buffPartData, buffPartCfg);
                //添加buff
                for (int i = 0; i < buffPartCfg.buffIds.Count; i++)
                {
                    AddBuff(buffComponent.entity, buffPartCfg.buffIds[i]);
                }
                
                //执行逻辑
                if (!string.IsNullOrEmpty(buffPartCfg.script))
                {
                    //执行脚本
                    var btTreeData = BuffManager.Instance.GetBuffPartBTTreeData(buffPartData.buffPartId);
                    if (btTreeData != null)
                    {
                        buffPartData.buffBTContext.Reset();
                        buffPartData.buffBTContext.Init(World, buffComponent, buffData, btTreeData, this, 0);
                        buffPartData.buffBTContext.SetPartData(buffPartData);
                        Execute(buffPartData.buffBTContext);
                        Clear(buffPartData.buffBTContext);
                    }

                }
            }
        }

        private void AddProperty(BuffComponent buffComponent, BuffData buffData, BuffPartData buffPartData, ResBuffPartLogic buffPartCfg)
        {

        }

        private void RemoveProperty(BuffComponent buffComponent, BuffData buffData, BuffPartData buffPartData, ResBuffPartLogic buffPartCfg)
        {

        }

        public BTStatus Execute(IBTContext context)
        {
            var treeData = context.treeData;
            if (treeData.rootData.children.Count > 0)
            {
                var childBtData = treeData.rootData.children[0];
                return BTDataHandlerInitialize.GetHandler(childBtData.keyIndex).Handler(context, childBtData);
            }
            return BTStatus.Success;
        }

        public void Clear(IBTContext context)
        {
            var treeData = context.treeData;
            if (treeData != null && treeData.rootData != null && treeData.rootData.children.Count > 0)
            {
                var childBtData = treeData.rootData.children[0];
                BTDataHandlerInitialize.GetHandler(childBtData.keyIndex).Clear(context, childBtData);
            }
        }
    }
}