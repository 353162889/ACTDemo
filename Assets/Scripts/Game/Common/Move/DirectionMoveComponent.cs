using UnityEngine;

namespace Game
{
    public class DirectionMoveComponent : DataComponent
    {
        //基础移动输入的方向
        public Vector3 inputDirection = Vector3.zero;
        //基础移动的渴望速度大小
        public float desiredSpeed = 0;
        public bool changeFace;
    }
}