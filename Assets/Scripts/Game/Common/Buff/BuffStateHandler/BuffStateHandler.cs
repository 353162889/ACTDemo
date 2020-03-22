using Unity.Entities;

namespace Game
{
    public class BuffStateHandler : IBuffStateHandler
    {
        public virtual void OnStart(BuffStateContext context)
        {
            var forbids = context.buffStateConfig.forbids;
            for (int i = 0; i < forbids.Count; i++)
            {
                context.buffStateData.forbiddance.Forbid(forbids[i]);
            }
        }

        public virtual void OnUpdate(BuffStateContext context) { }

        public virtual void OnEnd(BuffStateContext context)
        {
            var forbids = context.buffStateConfig.forbids;
            for (int i = 0; i < forbids.Count; i++)
            {
                context.buffStateData.forbiddance.ResumeForbid(forbids[i]);
            }
        }
    }
}