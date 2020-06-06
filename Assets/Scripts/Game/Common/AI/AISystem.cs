﻿using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class AISystem : ComponentSystem, IBTExecutor
    {
        private AIBTContext aiBTContext;

        protected override void OnCreate()
        {
            aiBTContext = new AIBTContext();
            base.OnCreate();
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