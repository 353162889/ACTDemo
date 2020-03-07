namespace Game
{
    public class InputSkillCmd : IInputCommand
    {
        public int skillId;
        public void Reset()
        {
        }

        public InputCommandType cmdType
        {
            get { return InputCommandType.Skill; }
        }
    }
}