using Unity.Entities;

namespace Game
{
    public class TraceComponent : DataComponent
    {
        public bool isTrace;
        public Entity target;
        public float stopMoveDistance;
        public float sqrStopMoveDistance;
    }
}