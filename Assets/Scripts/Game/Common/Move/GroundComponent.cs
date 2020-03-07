using UnityEngine;

namespace Game
{
    public class GroundComponent : DataComponent
    {
        public bool isGround = false;

        public GroundPointInfo groundPointInfo;

        public byte moveFlag = 0;
    }
}