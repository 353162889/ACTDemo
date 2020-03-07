using UnityEngine;

namespace Game
{
    public class AnimationPathMoveData
    {
        //移动路径信息[time,x,y,z,time,x,y,z,...]
        public float[] pointInfos;
    }

    public class StepMoveComponent : DataComponent
    {
        //渴望速度，主要给表现使用
        public Vector3 desiredVelocity = Vector3.zero;
        //当前移动的最终速度(包括方向与大小)
        public Vector3 velocity = Vector3.zero;

        //移动时是否改变面向（根据移动方向）
        public bool changeFace = false;

        public bool isMoving = false;

        //内部使用
        //单帧附加的移动速度（执行完当前帧后清零）
        public Vector3 innerFrameVelocity;
        //内部使用
        //当前可叠加的移动速度
        public Vector3 innerVelocity;
    }
}