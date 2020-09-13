using Framework;
using Microsoft.CSharp.RuntimeBinder;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class AISystem : ComponentSystem, IBTExecutor
    {
        private TargetTriggerSystem targetTriggerSystem;

        protected override void OnCreate()
        {
            targetTriggerSystem = World.GetOrCreateSystem<TargetTriggerSystem>();
            UtilityDebugEventDispatcher.Instance.AddEvent(UtilityDebugEvent.UpdateUtilityAIData, OnReloadUtilityAI);
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            UtilityDebugEventDispatcher.Instance.RemoveEvent(UtilityDebugEvent.UpdateUtilityAIData, OnReloadUtilityAI);
        }

        public void RunBTScript(Entity entity, string aiFile)
        {
            var aiComponent = World.GetComponent<AIComponent>(entity);
            if (aiComponent == null) return;
            RunBTScript(aiComponent, aiFile);
        }

        public void RunBTScript(AIComponent aiComponent, string aiFile)
        {
            if (aiComponent.btContext.aiFile == aiFile) return;
            var btTreeData = AIManager.Instance.GetAIBTTreeData(aiFile);
            if (btTreeData == null)
            {
                CLog.LogError("can not find aifile " + aiComponent.btContext.aiFile);
                return;
            }
            aiComponent.btContext.Init(aiFile, World, aiComponent, btTreeData, this);
            CLog.LogArgs("RunScript", aiFile);
        }

        public void StartUtilityDebug(Entity debugEntity)
        {
            Entities.ForEach((Entity entity, AIComponent aiComponent) =>
            {
                aiComponent.utilityContext.utilityAI?.SetDebug(entity == debugEntity);
            });
        }

        public void StopUtilityDebug()
        {
            Entities.ForEach((Entity entity, AIComponent aiComponent) =>
            {
                aiComponent.utilityContext.utilityAI?.SetDebug(false);
            });
        }

        public void RunUtilityAI(AIComponent aiComponent, string aiFile)
        {
            if (aiComponent.utilityContext.aiFile == aiFile) return;
            var utilityAIData = AIManager.Instance.GetUtilityAIData(aiFile);
            if (utilityAIData == null) return;
            UtilityAI utilityAI = new UtilityAI();
            utilityAI.Init(utilityAIData);
            aiComponent.utilityContext.Init(aiFile, World, aiComponent, utilityAI);
        }

        public void ReloadUtilityAI(AIComponent aiComponent, UtilityAIData utilityAIData)
        {
            UtilityAI utilityAI = new UtilityAI();
            utilityAI.Init(utilityAIData);
            aiComponent.utilityContext.ReloadUtilityAI(utilityAI);
        }

        private void OnReloadUtilityAI(int key, object args)
        {
            var utilityAIData = (UtilityAIData)args;
            if (utilityAIData == null) return;
            Entities.ForEach((AIComponent aiComponent) =>
            {
                if (aiComponent.utilityContext.utilityAI != null && aiComponent.utilityContext.utilityAI.isDebug)
                {
                    ReloadUtilityAI(aiComponent, utilityAIData);
                }
            });
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, AIComponent aiComponent, TransformComponent transformComponent) =>
            {
                if (aiComponent.aiStateType == AIStateType.Running)
                {
                    var worldState = aiComponent.worldState;
                    var targetTriggerComponent = World.GetComponent<TargetTriggerComponent>(entity);
                    if (targetTriggerComponent != null)
                    {
                        var target = aiComponent.worldState.target;
                        if (target == Entity.Null || !targetTriggerSystem.ValidEntity(targetTriggerComponent, target))
                        {
                            aiComponent.worldState.ResetTarget();
                            if (targetTriggerComponent.lstEntity.Count > 0)
                            {
                                aiComponent.worldState.SetFilterTarget(targetTriggerComponent.lstEntity[0]);
                            }
                        }
                    }

                    worldState.targetFactor = aiComponent.worldState.target == Entity.Null ? 0 : 1;
                    worldState.targetDistance = 9999;
                    if (worldState.targetFactor > 0)
                    {
                        var targetTransformComponent = World.GetComponent<TransformComponent>(aiComponent.worldState.target);
                        worldState.targetDistance = (transformComponent.position - targetTransformComponent.position).magnitude;
                    }
                    
                    UtilityContext utilityContext = aiComponent.utilityContext;
                    if (!string.IsNullOrEmpty(utilityContext.aiFile))
                    {
                        BTUtilityAction action = (BTUtilityAction) utilityContext.utilityAI.Select(utilityContext);
                        if (action != null)
                        {
                            if (!string.IsNullOrEmpty(action.aiFile))
                            {
                                this.RunBTScript(aiComponent, action.aiFile);
                            }
                        }
                    }

                    var btContext = aiComponent.btContext;
                    if (!string.IsNullOrEmpty(btContext.aiFile))
                    {
                        btContext.SetDeltaTime(Time.deltaTime);
                        BTStatus btState = Execute(btContext);
                        if (btState != BTStatus.Running)
                        {
                            btContext.ClearCacheData();
                        }
                    }
                }
            });
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
            var aiContext = (AIBTContext) context;
            if (aiContext != null)
            {
                aiContext.ClearCacheData();
            }
        }
    }
}