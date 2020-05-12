using Unity.Entities;

namespace Game
{
    public class MoveStateSystem : ComponentSystem
    {
        public void SetMoveState(Entity entity, MoveStateType moveStateType)
        {
            var moveStateComponent = World.GetComponent<MoveStateComponent>(entity);
            if (moveStateComponent != null)
            {
                moveStateComponent.moveStateType = moveStateType;
            }
        }

        public float GetMoveDesiredSpeed(Entity entity)
        {
            var moveStateComponent = World.GetComponent<MoveStateComponent>(entity);
            return GetMoveDesiredSpeed(moveStateComponent);
        }

        public float GetMoveDesiredSpeed(MoveStateComponent moveStateComponent)
        {
            if (moveStateComponent == null) return MoveStateComponent.defaultSpeed;
            else if (moveStateComponent.moveStateType == MoveStateType.Run)
            {
                return moveStateComponent.runSpeed;
            }
            else
            {
                return moveStateComponent.walkSpeed;
            }
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((MoveStateComponent moveStateComponent, StepMoveComponent stepMoveComponent) =>
            {
                if (!stepMoveComponent.isMoving && moveStateComponent.moveStateType != MoveStateType.Walk)
                {
                    moveStateComponent.moveStateType = MoveStateType.Walk;
                }
            });
        }
    }
}