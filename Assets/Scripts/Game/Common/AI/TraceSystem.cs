using Framework;
using Unity.Entities;

namespace Game
{
    public class TraceSystem : ComponentSystem
    {
        private PointsMoveSystem pointsMoveSystem;
        private FaceSystem faceSystem;
        protected override void OnCreate()
        {
            pointsMoveSystem = World.GetOrCreateSystem<PointsMoveSystem>();
            faceSystem = World.GetOrCreateSystem<FaceSystem>();
        }

        public void StartTrace(Entity entity, Entity target)
        {
            var traceComponent = World.GetComponent<TraceComponent>(entity);
            if (traceComponent == null) return;
            traceComponent.isTrace = true;
            traceComponent.target = target;
            traceComponent.sqrStopMoveDistance = traceComponent.stopMoveDistance * traceComponent.stopMoveDistance;
        }

        public void StopTrace(Entity entity)
        {
            var traceComponent = World.GetComponent<TraceComponent>(entity);
            if (traceComponent == null) return;
            traceComponent.isTrace = false;
            traceComponent.target = Entity.Null;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, TraceComponent traceComponent, TransformComponent transformComponent, PointsMoveComponent pointsMoveComponent) =>
            {
                if (traceComponent.isTrace)
                {
                    if (!World.EntityManager.Exists(traceComponent.target))
                    {
                        StopTrace(entity);
                        return;
                    }

                    var targetTransformComponent =  World.GetComponent<TransformComponent>(traceComponent.target);
                    var offset = targetTransformComponent.position - transformComponent.position;
                    if (offset.sqrMagnitude <
                        traceComponent.sqrStopMoveDistance)
                    {
                        pointsMoveSystem.StopMove(entity);
                        faceSystem.FaceTo(entity, offset);
                    }
                    else
                    {
                        pointsMoveSystem.Move(entity, targetTransformComponent.position, true);
                    }
                    
                }
            });
        }
    }
}