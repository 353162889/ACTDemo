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
        public string aiFile;
        public AIStateType aiStateType = AIStateType.Running;
        public AIBlackBoard blackBoard = new AIBlackBoard();
        public BTExecuteCache executeCache = new BTExecuteCache();
    }
}