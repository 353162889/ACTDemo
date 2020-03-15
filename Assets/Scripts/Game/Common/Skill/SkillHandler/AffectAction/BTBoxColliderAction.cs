using System.Collections.Generic;
using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class BTBoxColliderActionData : IBTTimelineDurationData
    {
        public Vector3 localPos;
        public Quaternion localRot;
        public Vector3 size;
        public float colliderDuration;
        public float duration
        {
            get { return colliderDuration;}
            set { colliderDuration = value; }
        }
    }


    public class BTBoxColliderAction : BTAffectAction<SkillBTContext, BTBoxColliderActionData>
    {

        internal protected struct InnerBTBoxColliderActionData
        {
            public float time;
            public EntityBoxHit boxHit;
            public List<Entity> lstHitEntity;
        }

        private static InnerBTBoxColliderActionData DefaultActionData = new InnerBTBoxColliderActionData() { time = 0, boxHit = null, lstHitEntity = null};

        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTBoxColliderActionData data)
        {
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            var cacheData = context.executeCache.GetCache<InnerBTBoxColliderActionData>(btData.dataIndex, DefaultActionData);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                var physicComponent = context.world.GetComponent<PhysicComponent>(context.skillComponent.entity);
                if (physicComponent == null) return BTStatus.Fail;
                var attackBoxParent = physicComponent.attackBoxParent;
                var entityBoxHit = BehaviourPool<EntityBoxHit>.Instance.GetObject(attackBoxParent);
                entityBoxHit.Init(context.world, attackBoxParent, data.localPos, data.localRot, data.size);
                cacheData.boxHit = entityBoxHit;
                cacheData.lstHitEntity = ResetObjectPool<List<Entity>>.Instance.GetObject();
            }

            cacheData.time += context.deltaTime;
            context.executeCache.SetCache(btData.dataIndex, cacheData);
            if (cacheData.boxHit != null)
            {
                var lst = ResetObjectPool<List<EntityHitInfo>>.Instance.GetObject();
                cacheData.boxHit.TryGetNewHitInfo(ref lst);
                for (int i = lst.Count - 1; i > -1; i--)
                {
                    var entity = lst[i].entity;
                    if (cacheData.lstHitEntity.Contains(entity))
                    {
                        lst.RemoveAt(i);
                    }
                    else
                    {
                        cacheData.lstHitEntity.Add(entity);
                    }
                }

                if (lst.Count > 0)
                {
                    //设置黑盒数据
                    context.blackBoard.SetData(SkillBlackBoardKeys.ListHitInfo, lst);
                    //触发子节点
                    this.TriggerChildren(context, btData);
                    CLog.LogArgs("攻击", lst.Count);
                    context.blackBoard.ClearData(SkillBlackBoardKeys.ListHitInfo);
                }
               
                ResetObjectPool<List<EntityHitInfo>>.Instance.SaveObject(lst);
            }

            if (cacheData.time > data.duration)
            {
                this.Clear(context, btData);
                return BTStatus.Success;
            }

            return BTStatus.Running;
        }

        protected override void Clear(SkillBTContext context, BTData btData, BTBoxColliderActionData data)
        {
            var cacheData = context.executeCache.GetCache<InnerBTBoxColliderActionData>(btData.dataIndex, DefaultActionData);
            if (cacheData.boxHit != null)
            {
                BehaviourPool<EntityBoxHit>.Instance.SaveObject(cacheData.boxHit);
                cacheData.boxHit = null;
            }

            if (cacheData.lstHitEntity != null)
            {
                ResetObjectPool<List<Entity>>.Instance.SaveObject(cacheData.lstHitEntity);
                cacheData.lstHitEntity = null;
            }
            base.Clear(context, btData, data);
        }
    }
}