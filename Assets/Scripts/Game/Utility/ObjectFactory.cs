using Framework;

namespace Game
{
    public class ObjectFactory<T> : Singleton<ObjectFactory<T>> where T : new()
    {
        public T Generate()
        {
            return new T();
        }
    }
}