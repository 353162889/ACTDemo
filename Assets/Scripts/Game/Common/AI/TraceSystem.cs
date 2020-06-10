using Unity.Entities;

namespace Game
{
    public class TraceSystem : ComponentSystem
    {
        private PointsMoveSystem pointsMoveSystem;
        protected override void OnCreate()
        {
            pointsMoveSystem = World.GetOrCreateSystem<PointsMoveSystem>();
        }

        public void StartTrace(Entity entity, Entity target)
        {
            var traceComponent = World.GetComponent<TraceComponent>(entity);
            if (traceComponent == null) return;
            traceComponent.isTrace = true;
            traceComponent.target = target;
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
            Entities.ForEach((Entity entity, TraceComponent traceComponent, PointsMoveComponent pointsMoveComponent) =>
            {
                if (traceComponent.isTrace)
                {
                    if (!World.EntityManager.Exists(traceComponent.target))
                    {
                        StopTrace(entity);
                        return;
                    }

                    var transformComponent =  World.GetComponent<TransformComponent>(traceComponent.target);
                    pointsMoveSystem.Move(entity, transformComponent.position, true);
                }
            });
        }
    }
}