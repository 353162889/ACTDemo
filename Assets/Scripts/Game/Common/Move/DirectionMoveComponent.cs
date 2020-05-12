using UnityEngine;

namespace Game
{
    public class DirectionMoveComponent : DataComponent
    {
        //基础移动输入的方向
        public Vector3 inputDirection = Vector3.zero;
        public bool isMoving;
        public bool changeFace;
    }
}