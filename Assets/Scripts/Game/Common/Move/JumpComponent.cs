using UnityEngine;

namespace Game
{
    public class JumpComponent : DataComponent
    {
        //是否正在跳跃
        public bool isJump = false;
        //跳跃渴望的速度
        public float desiredJumpSpeed = 0;
        //跳跃状态
        public JumpStateType jumpStateType;
        //起跳高度（播完起跳动画的高度）
        public float startJumpHeight = 0;
        //跳跃速度
        public Vector3 jumpVelocity;
    }
}