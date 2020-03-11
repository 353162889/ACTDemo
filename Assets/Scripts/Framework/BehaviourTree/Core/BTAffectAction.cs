using Framework;

namespace Game
{
    public abstract class BTAffectAction<T1, T2> : BTAction<T1, T2> where T1 : IBTContext
    {
        protected void TriggerChildren(T1 context, BTData btData)
        {
            if (btData.children != null)
            {
                for (int i = 0; i < btData.children.Count; i++)
                {
                    var childBTData = btData.children[i];
                    var childHandler = context.GetHandler(childBTData.keyIndex);
                    var result = childHandler.Handler(context, childBTData);
                    if (result == BTStatus.Fail)
                    {
                        CLog.LogColorArgs(CLogColor.Red, "BTAffectAction Execute child fail!child = ",
                            childBTData.data.GetType().Name);
                    }
                }
            }
        }
    }
}