
using UnityEngine;

namespace Game
{
    public class InputFaceCmd : IInputCommand
    {
        public Vector3 faceDirection;
        public bool immediately;
        public void Reset()
        {
            faceDirection = Vector3.zero;
            immediately = false;
        }

        public InputCommandType cmdType => InputCommandType.Face;
    }
}