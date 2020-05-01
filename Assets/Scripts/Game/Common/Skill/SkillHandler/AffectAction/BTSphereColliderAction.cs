using System.Collections.Generic;
using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class BTSphereColliderActionData : IBTTimelineDurationData
    {
        public int filterId;
        public Vector3 localPos;
        public Quaternion localRot;
        public float radius;
        public float colliderDuration;
        public float duration
        {
            get { return colliderDuration; }
            set { colliderDuration = value; }
        }
    }


    public class BTSphereColliderAction : BTAffectAction<SkillBTContext, BTSphereColliderActionData>
    {

        internal protected struct InnerBTSphereColliderActionData
        {
            public float time;
            public EntitySphereHit sphereHit;
            public List<Entity> lstHitEntity;
        }

        private static InnerBTSphereColliderActionData DefaultActionData = new InnerBTSphereColliderActionData() { time = 0, sphereHit = null, lstHitEntity = null };

        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTSphereColliderActionData data)
        {
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            var cacheData = context.executeCache.GetCache<InnerBTSphereColliderActionData>(btData.dataIndex, DefaultActionData);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                var physicComponent = context.world.GetComponent<PhysicComponent>(context.skillComponent.componentEntity);
                if (physicComponent == null) return BTStatus.Fail;
                var attackColliderParent = physicComponent.attackColliderParent;
                var entitySphereHit = BehaviourPool<EntitySphereHit>.Instance.GetObject(attackColliderParent);
                entitySphereHit.Init(context.world, attackColliderParent, data.localPos, data.localRot, data.radius);
                cacheData.sphereHit = entitySphereHit;
                cacheData.lstHitEntity = ResetObjectPool<List<Entity>>.Instance.GetObject();
            }

            cacheData.time += context.deltaTime;
            context.executeCache.SetCache(btData.dataIndex, cacheData);
            if (cacheData.sphereHit != null)
            {
                var lst = ResetObjectPool<List<EntityHitInfo>>.Instance.GetObject();
                cacheData.sphereHit.TryGetNewHitInfo(ref lst);
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

                var tempTargets = ResetObjectPool<List<Entity>>.Instance.GetObject();
                var tempResults = ResetObjectPool<List<Entity>>.Instance.GetObject();
                foreach (var entityHitInfo in lst)
                {
                    tempTargets.Add(entityHitInfo.entity);
                }
                TargetFilter.Filter(data.filterId, context.world, context.skillComponent.componentEntity, tempTargets, ref tempResults);
                for (int i = lst.Count - 1; i > -1; i--)
                {
                    if (!tempResults.Contains(lst[i].entity))
                    {
                        lst.RemoveAt(i);
                    }
                }
                ResetObjectPool<List<Entity>>.Instance.SaveObject(tempTargets);
                ResetObjectPool<List<Entity>>.Instance.SaveObject(tempResults);

                if (lst.Count > 0)
                {
                    var damageSystem = context.world.GetExistingSystem<DamageSystem>();
                    var lstDamage = ResetObjectPool<List<DamageInfo>>.Instance.GetObject();
                    for (int i = 0; i < lst.Count; i++)
                    {
                        var hitInfo = lst[i];
                        var damageInfo = damageSystem.CalDamageBySkill(context.skillComponent.componentEntity, hitInfo.entity, context.skillData,
                            hitInfo);
                        lstDamage.Add(damageInfo);
                    }
                    //设置黑盒数据
                    context.blackBoard.SetData(SkillBlackBoardKeys.ListDamageInfo, lstDamage);
                    //触发子节点
                    this.TriggerChildren(context, btData);
                    context.blackBoard.ClearData(SkillBlackBoardKeys.ListDamageInfo);
                    ResetObjectPool<List<DamageInfo>>.Instance.SaveObject(lstDamage);
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

        protected override void Clear(SkillBTContext context, BTData btData, BTSphereColliderActionData data)
        {
            var cacheData = context.executeCache.GetCache<InnerBTSphereColliderActionData>(btData.dataIndex, DefaultActionData);
            if (cacheData.sphereHit != null)
            {
                BehaviourPool<EntitySphereHit>.Instance.SaveObject(cacheData.sphereHit);
                cacheData.sphereHit = null;
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