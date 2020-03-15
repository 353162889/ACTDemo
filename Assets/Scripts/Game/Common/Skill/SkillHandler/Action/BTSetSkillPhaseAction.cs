using Framework;

namespace Game
{
    public class BTSetSkillPhaseActionData
    {
        public SkillPhaseType phaseType = SkillPhaseType.Backswing;
    }
    public class BTSetSkillPhaseAction : BTAction<SkillBTContext, BTSetSkillPhaseActionData>
    {
        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTSetSkillPhaseActionData data)
        {
            if (data.phaseType > context.skillData.phase)
            {
                context.skillData.phase = data.phaseType;
            }

            return BTStatus.Success;
        }
    }
}