using System;
using Framework;

namespace Game
{
    public enum BuffStateExeStatus
    {
        Init,
        Running,
        Destroy
    }
    [Serializable]
    public class BuffStateData : IPoolable, IEquatable<BuffStateData>
    {
        public int index;
        public BuffStateType stateType;
        public int bindBuffIndex;
        public Forbiddance forbiddance;
        public BuffStateExeStatus status;

        public void Reset()
        {
            bindBuffIndex = 0;
            forbiddance = null;
            status = BuffStateExeStatus.Init;
        }

        public static bool operator ==(BuffStateData lhs, BuffStateData rhs)
        {
            bool lRef = ReferenceEquals(null, lhs);
            bool rRef = ReferenceEquals(null, rhs);
            if (lRef && rRef) return true;
            if (lRef || rRef) return false;
            return rhs.index == rhs.index;
        }

        public static bool operator !=(BuffStateData lhs, BuffStateData rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(BuffStateData other)
        {
            return this == other;
        }

        public override bool Equals(object other)
        {
            return this == (BuffStateData)other;
        }

        public override int GetHashCode()
        {
            return index;
        }
    }
}