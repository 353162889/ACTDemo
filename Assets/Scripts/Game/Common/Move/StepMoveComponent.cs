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
        //单帧附加的移动速度（执行完当前帧后清零）
        public Vector3 singleFrameAppendVelocity;
        //记录上一帧最总应用到的单帧速度
        public Vector3 prevSingleFrameAppendVelocity;

        //单帧移动的位移(向量)
        public Vector3 curVelocity;
        //渴望速度，主要给表现使用
        public Vector3 desiredVelocity;

        //当前移动的最终速度(包括方向与大小)
        public Vector3 velocity = Vector3.zero;

        //移动时是否改变面向（根据移动方向）
        public bool changeFace = false;

        public bool isMoving = false;
    }
}