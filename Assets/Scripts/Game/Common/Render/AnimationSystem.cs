using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class AnimationSystem : ComponentSystem
    {
 #region Animator Way
        private bool ValidAnimator(AnimationComponent animationComponent)
        {
            if (null == animationComponent.animator) return false;
            return true;
        }

        public bool HasAnimatorParam(Entity entity, string paramName)
        {
            var animationComponent = World.GetComponent<AnimationComponent>(entity);
            if (null == animationComponent) return false;
            return HasAnimatorParam(animationComponent, paramName);
        }

        public bool HasAnimatorParam(AnimationComponent animationComponent, string paramName)
        {
            if (!ValidAnimator(animationComponent)) return false;
            if (animationComponent.parameters == null) return false;
            var parameters = animationComponent.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].name == paramName) return true;
            }
            return false;
        }

        public void SetAnimatorParam(Entity entity, string paramName, float paramValue)
        {
            var animationComponent = World.GetComponent<AnimationComponent>(entity);
            if (null == animationComponent) return;
            SetAnimatorParam(animationComponent, paramName, paramValue);
        }

        public void SetAnimatorParam(AnimationComponent animationComponent, string paramName, float paramValue)
        {
            if (!HasAnimatorParam(animationComponent, paramName)) return;
            var animator = animationComponent.animator;
            animator.SetFloat(paramName, paramValue);
        }

        public void SetAnimatorParam(Entity entity, string paramName, int paramValue)
        {
            var animationComponent = World.GetComponent<AnimationComponent>(entity);
            if (null == animationComponent) return;
            SetAnimatorParam(animationComponent, paramName, paramValue);
        }

        public void SetAnimatorParam(AnimationComponent animationComponent, string paramName, int paramValue)
        {
            if (!HasAnimatorParam(animationComponent, paramName)) return;
            var animator = animationComponent.animator;
            animator.SetInteger(paramName, paramValue);
        }

        public void SetAnimatorParam(Entity entity, string paramName, bool paramValue)
        {
            var animationComponent = World.GetComponent<AnimationComponent>(entity);
            if (null == animationComponent) return;
            SetAnimatorParam(animationComponent, paramName, paramValue);
        }

        public void SetAnimatorParam(AnimationComponent animationComponent, string paramName, bool paramValue)
        {
            if (!HasAnimatorParam(animationComponent, paramName)) return;
            var animator = animationComponent.animator;
            animator.SetBool(paramName, paramValue);
        }

        public void SetAnimatorParam(Entity entity, string paramName)
        {
            var animationComponent = World.GetComponent<AnimationComponent>(entity);
            if (null == animationComponent) return;
            SetAnimatorParam(animationComponent, paramName);
        }

        public void SetAnimatorParam(AnimationComponent animationComponent, string paramName)
        {
            if (!HasAnimatorParam(animationComponent, paramName)) return;
            var animator = animationComponent.animator;
            animator.SetTrigger(paramName);
        }

        public void ResetAnimatorParam(Entity entity, string paramName)
        {
            var animationComponent = World.GetComponent<AnimationComponent>(entity);
            if (null == animationComponent) return;
            ResetAnimatorParam(animationComponent, paramName);
        }

        public void ResetAnimatorParam(AnimationComponent animationComponent, string paramName)
        {
            if (!HasAnimatorParam(animationComponent, paramName)) return;
            animationComponent.animator.ResetTrigger(paramName);
        }

        public void ResetAnimatorAllParam(Entity entity)
        {
            var animationComponent = World.GetComponent<AnimationComponent>(entity);
            if (null == animationComponent) return;
            ResetAnimatorAllParam(animationComponent);
        }

        public void ResetAnimatorAllParam(AnimationComponent animationComponent)
        {
            if (!ValidAnimator(animationComponent)) return;
            var animator = animationComponent.animator;
            int count = animator.parameterCount;
            for (int i = 0; i < count; i++)
            {
                animator.ResetTrigger(animator.GetParameter(i).name);
            }
        }

        public AnimatorControllerParameter GetAnimationParamType(Entity entity, string animationParam)
        {
            var animationComponent = World.GetComponent<AnimationComponent>(entity);
            if (null == animationComponent) return null;
            return GetAnimationParamType(animationComponent, animationParam);
        }

        public AnimatorControllerParameter GetAnimationParamType(AnimationComponent animationComponent, string animationParam)
        {
            if (!ValidAnimator(animationComponent)) return null;
            var count = animationComponent.animator.parameterCount;
            for (int i = 0; i < count; i++)
            {
                var param = animationComponent.animator.GetParameter(i);
                if (param.name == animationParam) return param;
            }

            return null;
        }

        public float GetAnimationTime(Entity entity, string animationClipName)
        {
            var animationComponent = World.GetComponent<AnimationComponent>(entity);
            if (null == animationComponent) return 0;
            return GetAnimationTime(animationComponent, animationClipName);
        }

        public float GetAnimationTime(AnimationComponent animationComponent, string animationClipName)
        {
            if (!ValidAnimator(animationComponent)) return 0;
            var animator = animationComponent.animator;
            var clips = animator.runtimeAnimatorController.animationClips;
            foreach (var animationClip in clips)
            {
                if (animationClip.name == animationClipName) return animationClip.length;
            }

            return 0;
        }
        #endregion

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, AnimationComponent animationComponent, StepMoveComponent stepMoveComponent, TransformComponent transformComponent) =>
            {
                var directionMoveComponent = World.GetComponent<DirectionMoveComponent>(entity);
                if (directionMoveComponent != null && directionMoveComponent.isMoving)
                {
                    var velocity = stepMoveComponent.desiredVelocity;
                    velocity.y = 0;
                    this.SetAnimatorParam(animationComponent, AnimatorParamDefine.IsMoving, true);
                    this.SetAnimatorParam(animationComponent, AnimatorParamDefine.MoveSpeed, velocity.magnitude);
                    bool isRun = false;
                    var moveStateComponent = World.GetComponent<MoveStateComponent>(entity);
                    if (moveStateComponent != null)
                    {
                        isRun = moveStateComponent.moveStateType == MoveStateType.Run;
                    }
                    this.SetAnimatorParam(animationComponent, AnimatorParamDefine.IsRun, isRun);
                    //设置移动方向
                    var direction = Quaternion.Inverse(transformComponent.rotation) * velocity;
                    direction.Normalize();
                    animationComponent.lastMoveDirectionPosition = Vector3.MoveTowards(animationComponent.lastMoveDirectionPosition, direction,
                        animationComponent.moveDirectionSpeed * Time.deltaTime);

                    this.SetAnimatorParam(animationComponent, AnimatorParamDefine.MoveDirectionX, animationComponent.lastMoveDirectionPosition.x);
                    this.SetAnimatorParam(animationComponent, AnimatorParamDefine.MoveDirectionZ, animationComponent.lastMoveDirectionPosition.z);
                }
                else
                {
                    this.SetAnimatorParam(animationComponent, AnimatorParamDefine.IsMoving, false);
                    this.SetAnimatorParam(animationComponent, AnimatorParamDefine.MoveSpeed, 0.0f);
                    animationComponent.lastMoveDirectionPosition = Vector3.zero;
                }

                var jumpComponent = World.GetComponent<JumpComponent>(entity);
                JumpStateType jumpState = JumpStateType.None;
                if (jumpComponent != null)
                {
                    jumpState = jumpComponent.jumpStateType;
                }
                this.SetAnimatorParam(animationComponent, AnimatorParamDefine.JumpBeforeAir,
                    jumpState == JumpStateType.JumpBeforeAir);

                var inAirComponent = World.GetComponent<InAirComponent>(entity);
                bool isInAir = false;
                InAirStateType inAirState = InAirStateType.None;
                if (inAirComponent != null)
                {
                    isInAir = inAirComponent.isInAir;
                    inAirState = inAirComponent.inAirStateType;
                }
                this.SetAnimatorParam(animationComponent, AnimatorParamDefine.InAir, isInAir);
                if (isInAir)
                {
                    this.SetAnimatorParam(animationComponent, AnimatorParamDefine.InAirBeforeGround,
                        inAirState == InAirStateType.InAirBeforeGround);
                }
            });
        }
    }
}