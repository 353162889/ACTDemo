using Framework;
using NodeEditor;

namespace Game
{
    public class TestSkillData
    {
        public int index;
    }

    public class TestSkillHandler : BTDataHandler<SkillBTContext, TestSkillData>
    {
        protected override BTStatus Handler(SkillBTContext context, BTData btData, TestSkillData data)
        {
            CLog.LogArgs("TestSkillHandler", data.index);
            return BTStatus.Success;
        }
    }
}