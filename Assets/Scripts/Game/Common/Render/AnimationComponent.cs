using UnityEngine;

namespace Game
{
    public class AnimationComponent : DataComponent
    {
        public Animator animator;

        public AnimatorControllerParameter[] parameters;
        //记录上一次八向移动的位置，用于八向移动转向过度
        public Vector3 lastMoveDirectionPosition = Vector3.zero;
        public float moveDirectionSpeed = 5;
    }
}