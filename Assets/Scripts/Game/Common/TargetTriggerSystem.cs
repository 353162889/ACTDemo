using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class TargetTriggerSystem : ComponentSystem
    {
        public TargetTriggerComponent AddSphereTargetTriggerComponent(Entity entity, float enterRadius, float exitRadius, int filterId)
        {
            var physicComponent = World.GetComponent<PhysicComponent>(entity);
            if (physicComponent == null) return null;
            var targetTriggerComponent = World.AddComponentOnce<TargetTriggerComponent>(entity);
            targetTriggerComponent.filterId = filterId;
            targetTriggerComponent.isUpdate = false;
            GameObject goSphereRangeTrigger = new GameObject();
            physicComponent.targetTriggerParent.gameObject.AddChildToParent(goSphereRangeTrigger, "SphereRangeTrigger");
            targetTriggerComponent.rangeTrigger = goSphereRangeTrigger.AddComponentOnce<EntityRangeTrigger>();

            GameObject goEnterCollider = new GameObject();
            goEnterCollider.layer = LayerDefine.TargetTriggerColliderInt;
            goSphereRangeTrigger.AddChildToParent(goEnterCollider, "EnterCollider");
            var enterCollider = goEnterCollider.AddComponentOnce<SphereCollider>();
            enterCollider.isTrigger = true;
            enterCollider.radius = enterRadius;
            SphereCollider exitCollider = enterCollider;
            if (enterRadius != exitRadius)
            {
                GameObject goExitCollider = new GameObject();
                goEnterCollider.layer = LayerDefine.TargetTriggerColliderInt;
                goSphereRangeTrigger.AddChildToParent(goExitCollider, "ExitCollider");
                exitCollider = goExitCollider.AddComponentOnce<SphereCollider>();
                exitCollider.isTrigger = true;
                exitCollider.radius = exitRadius;
            }
            targetTriggerComponent.rangeTrigger.Init(World, enterCollider, exitCollider, targetTriggerComponent);

            return targetTriggerComponent;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, TargetTriggerComponent triggerComponent) =>
            {
                if (triggerComponent.isUpdate)
                {
                    var lstEntity = triggerComponent.rangeTrigger.lstEntity;
                    triggerComponent.lstEntity.Clear();
                    TargetFilter.Filter(triggerComponent.filterId, World, entity, lstEntity, ref triggerComponent.lstEntity);
                    triggerComponent.isUpdate = false;
                }
            });
        }
    }
}