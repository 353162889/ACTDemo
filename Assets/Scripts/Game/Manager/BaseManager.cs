using System.Collections.Generic;
using Framework;

namespace Game
{
    public abstract class BaseManager<T> : Singleton<T> where T : new()
    {
        public virtual void Init() { }
    }
}