using UnityEngine;

namespace Game
{
    public class BuffFloatComponent : DataComponent
    {
        //是否正在浮空
        public bool isFloat = false;
        //浮空速度
        public Vector3 floatVerticalVelocity;
        public Vector3 floatHorizonalVelocity;
    }
}