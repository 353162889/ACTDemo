using Unity.Entities;

namespace Game
{
    public class MoveStateSystem : ComponentSystem
    {
        private SkillSystem skillSystem;
        protected override void OnCreate()
        {
            skillSystem = World.GetOrCreateSystem<SkillSystem>();
            skillSystem.OnSkillStart += OnSkillStart;
        }

        protected override void OnDestroy()
        {
            if (skillSystem != null)
            {
                skillSystem.OnSkillStart -= OnSkillStart;
            }
        }

        private void OnSkillStart(Entity entity, SkillData skillData)
        {
            var moveStateComponent = World.GetComponent<MoveStateComponent>(entity);
            if (moveStateComponent != null && moveStateComponent.moveStateType != MoveStateType.Walk)
            {
                moveStateComponent.moveStateType = MoveStateType.Walk;
            }
        }

        public void SetMoveState(Entity entity, MoveStateType moveStateType)
        {
            var moveStateComponent = World.GetComponent<MoveStateComponent>(entity);
            if (moveStateComponent != null)
            {
                moveStateComponent.moveStateType = moveStateType;
            }
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
            Entities.ForEach((MoveStateComponent moveStateComponent, StepMoveComponent stepMoveComponent, PropertyComponent propertyComponent) =>
            {
                if (!stepMoveComponent.isMoving && moveStateComponent.moveStateType != MoveStateType.Walk)
                {
                    moveStateComponent.moveStateType = MoveStateType.Walk;
                }

                propertyComponent.moveSpeed = GetMoveDesiredSpeed(moveStateComponent);
            });
        }
    }
}