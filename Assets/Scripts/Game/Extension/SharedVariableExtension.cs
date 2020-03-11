using System.Collections.Generic;
using BTCore;

namespace Game
{
    public class SharedListHitInfo : BTSharedVariable<List<EntityHitInfo>>
    {
        public static implicit operator SharedListHitInfo(List<EntityHitInfo> value) { return new SharedListHitInfo { mValue = value }; }
    }
}