using Unity.Entities;

namespace Game
{
    public interface IBuffStateHandler
    {
        void OnStart(BuffStateContext context);
        void OnUpdate(BuffStateContext context);
        void OnEnd(BuffStateContext context);
    }
}