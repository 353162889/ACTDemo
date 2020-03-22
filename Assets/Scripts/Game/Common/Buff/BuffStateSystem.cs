using System;
using System.Collections.Generic;
using Framework;
using Unity.Entities;

namespace Game
{
    public class BuffStateSystem : ComponentSystem
    {
        public event Action<Entity, int> OnRemoveBuff;

        private ForbidSystem forbidSystem;
        private BuffStateContext m_cBuffStateContext;
        private List<BuffStateData> m_lstTemp;
        protected override void OnCreate()
        {
            m_cBuffStateContext = new BuffStateContext();
            m_lstTemp = new List<BuffStateData>();
            ObjectPool<BuffStateData>.Instance.Init(50);
            forbidSystem = World.GetOrCreateSystem<ForbidSystem>();
        }

        protected override void OnDestroy()
        {
            OnRemoveBuff = null;
        }

        public int AddState(Entity entity, BuffStateType stateType, int bindBuffId = -1)
        {
            var buffStateComponent = World.GetComponent<BuffStateComponent>(entity);
            if (buffStateComponent == null) return -1;
            var buffStateData = ObjectPool<BuffStateData>.Instance.GetObject();
            buffStateData.index = buffStateComponent.stateIndex++;
            buffStateData.stateType = stateType;
            buffStateData.bindBuffIndex = bindBuffId;
            buffStateData.status = BuffStateExeStatus.Init;
            buffStateData.forbiddance = forbidSystem.AddForbiddance(entity, "Buff_State_" + stateType.ToString());
            return buffStateData.index;
        }

        public bool RemoveState(Entity entity, int stateIndex)
        {
            if (stateIndex <= 0) return false;
            var buffStateComponent = World.GetComponent<BuffStateComponent>(entity);
            if (buffStateComponent == null) return false;
            BuffStateData buffStateData;
            if (buffStateComponent.dicStates.TryGetValue(stateIndex, out buffStateData))
            {
                buffStateData.status = BuffStateExeStatus.Destroy;
                return true;
            }
            return false;
        }

        private void RemoveState(BuffStateComponent buffStateComponent, BuffStateData buffStateData)
        {
            buffStateComponent.dicStates.Remove(buffStateData.index);
            //buff系统移除buff
            if (buffStateData.bindBuffIndex > 0 && OnRemoveBuff != null)
            {
                OnRemoveBuff.Invoke(buffStateComponent.entity, buffStateData.bindBuffIndex);
            }
            ObjectPool<BuffStateData>.Instance.SaveObject(buffStateData);
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, BuffStateComponent buffStateComponent) =>
            {
                foreach (var pair in buffStateComponent.dicStates)
                {
                    var buffStateData = pair.Value;
                    var cfg = BuffStateConfig.GetConfig(buffStateData.stateType);
                    m_cBuffStateContext.Init(World, buffStateComponent, buffStateData, cfg);
                    if (buffStateData.status == BuffStateExeStatus.Init)
                    {
                        cfg.handler.OnStart(m_cBuffStateContext);
                    }
                    else if (buffStateData.status == BuffStateExeStatus.Running)
                    {
                        cfg.handler.OnUpdate(m_cBuffStateContext);
                    }
                    else
                    {
                        cfg.handler.OnEnd(m_cBuffStateContext);
                        m_lstTemp.Add(buffStateData);
                    }
                    m_cBuffStateContext.Reset();
                }

                for (int i = 0; i < m_lstTemp.Count; i++)
                {
                    RemoveState(buffStateComponent, m_lstTemp[i]);
                }
                m_lstTemp.Clear();
            });
        }
    }
}