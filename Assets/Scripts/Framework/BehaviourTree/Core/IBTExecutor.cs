namespace Framework
{
    //行为树执行器
    public interface IBTExecutor
    {
        BTStatus Execute(IBTContext context);
        void Clear(IBTContext context);
    }
}