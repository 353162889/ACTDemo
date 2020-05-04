using System;
using Framework;

namespace Game
{
    public enum ForbidType
    {
        Ability = 1 << 1,
        Jump = 1 << 2,
        InputMove = 1 << 3,
        InputFace = 1 << 4,
        Gravity = 1 << 5,
    }

    public class Forbiddance : IEquatable<Forbiddance>, IPoolable
    {
        public int Index = -1;
        public int flags;
        public string desc;

        public bool IsForbid(ForbidType forbidType)
        {
            return (this.flags & (int) forbidType) != 0;
        }

        public void Forbid(ForbidType forbidType)
        {
            Forbid((int)forbidType);
        }

        public void ResumeForbid(ForbidType resumeType)
        {
            ResumeForbid((int)resumeType);
        }

        public void Forbid(int flag)
        {
            this.flags |= flag;
        }

        public void ResumeForbid(int flag)
        {
            this.flags &= (~flag);
        }

        public static bool operator ==(Forbiddance lhs, Forbiddance rhs)
        {
            bool lRef = ReferenceEquals(null, lhs);
            bool rRef = ReferenceEquals(null, rhs);
            if (lRef && rRef) return true;
            if (lRef || rRef) return false;
            return lhs.Index == rhs.Index;
        }

        public static bool operator !=(Forbiddance lhs, Forbiddance rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(Forbiddance other)
        {
            return this == other;
        }

        public override bool Equals(object other)
        {
            return this == (Forbiddance) other;
        }

        public override int GetHashCode()
        {
            return Index;
        }

        public static Forbiddance Null => new Forbiddance();

        public override string ToString()
        {
            return Equals(Forbiddance.Null) ? "Entity.Null" : $"Entity({Index},desc:{desc},flags:{flags})";
        }

        public void Reset()
        {
            flags = 0;
            desc = "";
        }
    }
}