namespace Game
{
    public class InputJumpCmd : IInputCommand
    {
        public void Reset()
        {
        }

        public InputCommandType cmdType => InputCommandType.Jump;
    }
}