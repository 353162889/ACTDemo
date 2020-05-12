using Framework;

namespace Game
{
    public class BTSetMoveStateActionData
    {
        public MoveStateType moveStateType;
    }
    public class BTSetMoveStateAction : BTAction<SkillBTContext, BTSetMoveStateActionData>
    {
        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTSetMoveStateActionData data)
        {
            var moveStateSystem = context.world.GetExistingSystem<MoveStateSystem>();
            if (moveStateSystem != null)
            {
                moveStateSystem.SetMoveState(context.skillComponent.componentEntity, data.moveStateType);
            }
            return BTStatus.Success;
        }
    }
}