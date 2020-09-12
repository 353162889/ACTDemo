using System;
using System.Collections.Generic;
using Framework;
using Unity.Entities;

namespace Game
{
    public class UtilityAISystem : ComponentSystem
    {
        private AISystem aiSystem;
        private TargetTriggerSystem targetTriggerSystem;
        protected override void OnCreate()
        {
            aiSystem = World.GetOrCreateSystem<AISystem>();
            targetTriggerSystem = World.GetOrCreateSystem<TargetTriggerSystem>();
            UtilityDebugEventDispatcher.Instance.AddEvent(UtilityDebugEvent.UpdateUtilityAIData, OnReloadUtilityAI);
        }

        protected override void OnDestroy()
        {
            UtilityDebugEventDispatcher.Instance.RemoveEvent(UtilityDebugEvent.UpdateUtilityAIData, OnReloadUtilityAI);
        }

        private void OnReloadUtilityAI(int key, object args)
        {
            var utilityAIData = (UtilityAIData) args;
            if (utilityAIData == null) return;
            Entities.ForEach((UtilityAIComponent utilityAIComponent) =>
            {
                if (utilityAIComponent.utilityAI != null && utilityAIComponent.utilityAI.isDebug)
                {
                    ReloadUtilityAI(utilityAIComponent, utilityAIData);
                }
            });
        }

        public void StartDebug(Entity debugEntity)
        {
            Entities.ForEach((Entity entity, UtilityAIComponent utilityAIComponent) =>
            {
                utilityAIComponent.utilityAI.SetDebug(entity == debugEntity);
            });
        }

        public void StopDebug()
        {
            Entities.ForEach((Entity entity, UtilityAIComponent utilityAIComponent) =>
            {
                utilityAIComponent.utilityAI.SetDebug(false);
            });
        }

        public void LoadAI(UtilityAIComponent utilityAIComponent, string aiFile)
        {
            if (utilityAIComponent.aiFile == aiFile) return;
            var utilityAIData = AIManager.Instance.GetUtilityAIData(aiFile);
            if (utilityAIData == null) return;
            utilityAIComponent.aiFile = aiFile;
            UtilityContext context = new UtilityContext(utilityAIComponent);
            UtilityAI utilityAI = new UtilityAI();
            utilityAI.Init(utilityAIData);
            utilityAIComponent.utilityContext = context;
            utilityAIComponent.utilityAI = utilityAI;
        }

        public void ReloadUtilityAI(UtilityAIComponent utilityAIComponent, UtilityAIData utilityAIData)
        {
            UtilityAI utilityAI = new UtilityAI();
            utilityAI.Init(utilityAIData);
            if (utilityAIComponent.utilityAI != null)
            {
                utilityAI.SetDebug(utilityAIComponent.utilityAI.isDebug);    
            }
            utilityAIComponent.utilityAI = utilityAI;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, UtilityAIComponent utilityAIComponent, AIComponent aiComponent, TransformComponent transformComponent) =>
            {
                if (utilityAIComponent.utilityAI != null)
                {
                    var targetTriggerComponent = World.GetComponent<TargetTriggerComponent>(entity);
                    if (targetTriggerComponent != null)
                    {
                        var target = aiComponent.blackBoard.target;
                        if (target == Entity.Null || !targetTriggerSystem.ValidEntity(targetTriggerComponent, target))
                        {
                            aiComponent.blackBoard.ResetTarget();
                            if (targetTriggerComponent.lstEntity.Count > 0)
                            {
                                aiComponent.blackBoard.SetFilterTarget(targetTriggerComponent.lstEntity[0]);
                            }
                        }
                    }

                    UtilityContext context = utilityAIComponent.utilityContext;
                    context.targetFactor = aiComponent.blackBoard.target == Entity.Null ? 0 : 1;
                    context.targetDistance = 9999;
                    if (context.targetFactor > 0)
                    {
                        var targetTransformComponent = World.GetComponent<TransformComponent>(aiComponent.blackBoard.target);
                        context.targetDistance = (transformComponent.position - targetTransformComponent.position).magnitude;
                    }
                    BTUtilityAction action = (BTUtilityAction)utilityAIComponent.utilityAI.Select(utilityAIComponent.utilityContext);
                    if (action != null)
                    {
                        CLog.LogArgs("ai:", action.name);
                        if (!string.IsNullOrEmpty(action.aiFile))
                        {
                            aiSystem.RunScript(aiComponent, action.aiFile);
                        }
                    }
                }
            });
        }
    }
}