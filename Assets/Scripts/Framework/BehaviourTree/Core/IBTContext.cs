using BTCore;

namespace Framework
{
    //存放BT树数据上下文
    public interface IBTContext
    {
        BTTreeData treeData { get; }
        IBTExecutor executor { get; }
        BTBlackBoard blackBoard { get; }
        BTExecuteCache executeCache { get; }
        IBTDataHandler GetHandler(int index);
        float deltaTime { get; }
    }
}