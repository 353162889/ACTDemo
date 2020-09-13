using BTCore;
using Framework;

namespace Game
{

    public static class AIBlackBoardKeys
    {
        public static string TargetPosition = "TargetPosition";
        public static string SkillIDList = "SkillIDList";
    }

    public class AIComponent : DataComponent
    {
        public AIStateType aiStateType = AIStateType.Running;
        public AIWorldState worldState = new AIWorldState();
        public AIBTContext btContext = new AIBTContext();
        public UtilityContext utilityContext = new UtilityContext();
    }
}