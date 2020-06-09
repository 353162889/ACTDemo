using BTCore;
using Framework;

namespace Game
{

    public static class AIBlackBoardKeys
    {
        public static string TargetPosition = "TargetPosition";
        public static string TargetEntity = "TargetEntity";
        public static string SkillIDList = "SkillIDList";
    }

    public class AIComponent : DataComponent
    {
        public string aiFile;
        public AIStateType aiStateType = AIStateType.Running;
        public BTBlackBoard blackBoard = new BTBlackBoard();
        public BTExecuteCache executeCache = new BTExecuteCache();
    }
}