namespace Game
{
    public class InputStopMoveDirectionCmd : IInputCommand
    {
        public InputCommandType cmdType => InputCommandType.StopMoveDirection;
        public void Reset()
        {
        }
    }
}