using UnityEngine;

namespace Game
{
    public class InAirComponent : DataComponent
    {
        //是否正在空中
        public bool isInAir = false;

        public InAirStateType inAirStateType;

        public float endGroundHeight = 0;
        //空中速度
        public Vector3 inAirVerticalVelocity;
        //跳跃是开始的水平速度方向，（后续跳跃中的移动以这个速度为基准，附加速度后，最终速度不会超过该值）
        public Vector3 startHorizonalDirection;
        //开始跳跃的水平速度大小
        public float startHorizonalSpeed;
        //当前空中的移动速度
        public Vector3 velocity;

        //基础移动输入的方向
        public Vector3 inputDirection = Vector3.zero;
        //输入速度大小系数（与startHorizonalVelocity大小相关）
        public float inputSpeedFactor = 4;
        //输入速度大小
        public float inputSpeed = 0;
    }
}