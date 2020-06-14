using Framework;
using Microsoft.CSharp.RuntimeBinder;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class AISystem : ComponentSystem, IBTExecutor
    {
        private AIBTContext aiBTContext;
        private TargetTriggerSystem targetTriggerSystem;

        protected override void OnCreate()
        {
            aiBTContext = new AIBTContext();
            targetTriggerSystem = World.GetOrCreateSystem<TargetTriggerSystem>();
            base.OnCreate();
        }

        public void RunScript(Entity entity, string aiFile)
        {
            var aiComponent = World.GetComponent<AIComponent>(entity);
            if (aiComponent.aiFile == aiFile) return;
            var oldAIFile = aiComponent.aiFile;
            var btTreeData = AIManager.Instance.GetAIBTTreeData(oldAIFile);
            if (btTreeData != null)
            {
                aiBTContext.Reset();
                aiBTContext.Init(World, aiComponent, btTreeData, this, Time.deltaTime);
                Clear(aiBTContext);
            }
            aiComponent.aiFile = aiFile;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, AIComponent aiComponent) =>
            {
                if (aiComponent.aiStateType == AIStateType.Running)
                {
                    if (string.IsNullOrEmpty(aiComponent.aiFile)) return;
                    var btTreeData = AIManager.Instance.GetAIBTTreeData(aiComponent.aiFile);
                    if (btTreeData == null)
                    {
                        CLog.LogError("can not find aifile "+ aiComponent.aiFile);
                        return;
                    }

                    var targetTriggerComponent = World.GetComponent<TargetTriggerComponent>(entity);
                    if (targetTriggerComponent != null)
                    {
                        var target = aiComponent.blackBoard.target;
                        if (target == Entity.Null || !targetTriggerSystem.ValidEntity(targetTriggerComponent,target))
                        {
                            aiComponent.blackBoard.ResetTarget();
                            if (targetTriggerComponent.lstEntity.Count > 0)
                            {
                                aiComponent.blackBoard.SetFilterTarget(targetTriggerComponent.lstEntity[0]);
                            }
                        }
                    }

                    aiBTContext.Reset();
                    aiBTContext.Init(World, aiComponent, btTreeData, this, Time.deltaTime);
                    BTStatus btState = Execute(aiBTContext);
                    if (btState != BTStatus.Running)
                    {
                        Clear(aiBTContext);
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
            var treeData = context.treeData;
            if (treeData != null && treeData.rootData != null && treeData.rootData.children.Count > 0)
            {
                var childBtData = treeData.rootData.children[0];
                BTDataHandlerInitialize.GetHandler(childBtData.keyIndex).Clear(context, childBtData);
            }
        }
    }
}