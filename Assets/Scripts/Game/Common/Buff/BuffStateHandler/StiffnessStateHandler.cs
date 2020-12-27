namespace Game
{
    public class StiffnessStateHandler : BuffStateHandler
    {
        public override void OnStart(BuffStateContext context)
        {
            base.OnStart(context);
            var skillSystem = context.world.GetExistingSystem<SkillSystem>();
            if (skillSystem != null)
            {
                skillSystem.CancelSkill(context.buffStateComponent.componentEntity, true);
            }
        }
    }
}