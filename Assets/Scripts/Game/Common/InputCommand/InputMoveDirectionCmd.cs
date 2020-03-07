using UnityEngine;

namespace Game
{
    public class InputMoveDirectionCmd : IInputCommand
    {
        public Vector3 moveDirection { get; set; }

        public InputCommandType cmdType => InputCommandType.MoveDirection;

        public virtual void Reset()
        {
            moveDirection = Vector3.zero;
        }
    }
}