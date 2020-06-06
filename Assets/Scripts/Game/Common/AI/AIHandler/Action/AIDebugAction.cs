using Framework;

namespace Game
{
    public class AIDebugActionData
    {
        public string msg;
    }
    public class AIDebugAction : BTAction<AIBTContext, AIDebugActionData>
    {
        protected override BTStatus Handler(AIBTContext context, BTData btData, AIDebugActionData data)
        {
            CLog.Log(data.msg);
            return BTStatus.Success;
        }
    }
}