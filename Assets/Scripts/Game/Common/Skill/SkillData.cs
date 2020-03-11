using BTCore;
using Framework;

namespace Game
{
    public static class SkillBlackBoardKeys
    {
        public static string ListHitInfo = "ListHitInfo";
    }

    public class SkillData : IPoolable
    {
        public int skillId;
        public float skillTime;
        public float skillTimeScale;
        public BTBlackBoard blackBoard = new BTBlackBoard();
        public BTExecuteCache executeCache = new BTExecuteCache();
        public Forbiddance forbidance;

        public void Reset()
        {
            blackBoard.Clear();
            executeCache.Clear();
        }
    }
}