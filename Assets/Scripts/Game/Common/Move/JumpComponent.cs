using UnityEngine;

namespace Game
{
    public class JumpComponent : DataComponent
    {
        //是否正在跳跃
        public bool isJump = false;
        //渴望高度
        public float desiredHeight = 0;
        //跳跃状态
        public JumpStateType jumpStateType;

        //开始起跳等待的时间（起跳需要一个蓄力过程，也需要一个蓄力动画）
        public float startJumpWaitTime = 3 / 30f;
        //开始离地的时间（配合动画使用）
        public float startJumpGroundTime;

        //跳跃落地高度
        public float endJumpGroundHeight = 0;

        public Vector3 jumpHorizonalVelocity;

        public Forbiddance forbidance;
    }
}