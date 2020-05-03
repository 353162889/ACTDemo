using UnityEngine;

namespace Game
{
    public class InAirComponent : DataComponent
    {
        //是否正在空中
        public bool isInAir = false;
        //空中速度
        public Vector3 inAirVerticalVelocity;
        public Vector3 inAirHorizonalVelocity;
    }
}