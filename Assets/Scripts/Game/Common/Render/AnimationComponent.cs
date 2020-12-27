using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AnimationComponent : DataComponent
    {
        public Animator animator;

        public Dictionary<string, AnimatorControllerParameter> dictParameters = new Dictionary<string, AnimatorControllerParameter>();
        //记录上一次八向移动的位置，用于八向移动转向过度
        public Vector3 lastMoveDirectionPosition = Vector3.zero;
        public float moveDirectionSpeed = 5;

        public void SetAnimator(Animator animator)
        {
            this.animator = animator;
            var parameters = animator.parameters;
            dictParameters.Clear();
            foreach (var animatorControllerParameter in parameters)
            {
                dictParameters.Add(animatorControllerParameter.name, animatorControllerParameter);
            }
        }
    }
}