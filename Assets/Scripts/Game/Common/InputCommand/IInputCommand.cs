using Framework;

namespace Game
{
    public interface IInputCommand : IPoolable
    {
        InputCommandType cmdType { get; }
    }
}