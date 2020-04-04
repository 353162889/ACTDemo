using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class InputSystem : ComponentSystem
    {
        private InputComponent inputComponent;
        private ComboSystem comboSystem;
        private CacheSkillSystem cacheSkillSystem;
        private SkillSystem skillSystem;
        private DirectionMoveSystem directionMoveSystem;
        private JumpSystem jumpSystem;
        private CameraSystem cameraSystem;
        protected override void OnCreate()
        {
            inputComponent = World.AddSingletonComponent<InputComponent>();
            comboSystem = World.GetOrCreateSystem<ComboSystem>();
            cacheSkillSystem = World.GetOrCreateSystem<CacheSkillSystem>();
            skillSystem = World.GetOrCreateSystem<SkillSystem>();
            directionMoveSystem = World.GetOrCreateSystem<DirectionMoveSystem>();
            jumpSystem = World.GetOrCreateSystem<JumpSystem>();
            cameraSystem = World.GetOrCreateSystem<CameraSystem>();
            ObjectPool<InputMoveDirectionCmd>.Instance.Init(5);
            ObjectPool<InputStopMoveDirectionCmd>.Instance.Init(5);
            ObjectPool<InputJumpCmd>.Instance.Init(2);
            ObjectPool<InputSkillCmd>.Instance.Init(10);
        }

        public void InputMoveDirection(Vector2 moveDirection)
        {
            var cmd = ObjectPool<InputMoveDirectionCmd>.Instance.GetObject();
            Vector3 direction = cameraSystem.TransformDirection(new Vector3(moveDirection.x, 0, moveDirection.y));
            direction.Normalize();
            cmd.moveDirection = direction;
            inputComponent.queueCmd.Enqueue(cmd);
        }

        public void InputStopMoveDirection()
        {
            var cmd = ObjectPool<InputStopMoveDirectionCmd>.Instance.GetObject();
            inputComponent.queueCmd.Enqueue(cmd);
        }

        public void InputJump()
        {
            var cmd = ObjectPool<InputJumpCmd>.Instance.GetObject();
            inputComponent.queueCmd.Enqueue(cmd);
        }

        public void InputSkill(int skillId)
        {
            var cmd = ObjectPool<InputSkillCmd>.Instance.GetObject();
            cmd.skillId = skillId;
            inputComponent.queueCmd.Enqueue(cmd);
        }

        protected override void OnUpdate()
        {
            while (inputComponent.queueCmd.Count > 0)
            {
                var cmd = inputComponent.queueCmd.Dequeue();
                if (cmd.cmdType == InputCommandType.MoveDirection)
                {
                    var moveDirectionCmd = cmd as InputMoveDirectionCmd;
                    inputComponent.inputMoveDirection = moveDirectionCmd.moveDirection;
                    inputComponent.inputMoveDirection.Normalize();
                    ObjectPool<InputMoveDirectionCmd>.Instance.SaveObject(moveDirectionCmd);
                }
                else if (cmd.cmdType == InputCommandType.StopMoveDirection)
                {
                    var stopMoveDirectionCmd = cmd as InputStopMoveDirectionCmd;
                    inputComponent.inputMoveDirection = Vector3.zero;
                    directionMoveSystem.StopMove(inputComponent.entity);
                    ObjectPool<InputStopMoveDirectionCmd>.Instance.SaveObject(stopMoveDirectionCmd);
                }
                else if(cmd.cmdType == InputCommandType.Jump)
                {
                    var jumpCmd = cmd as InputJumpCmd;
                    Vector3 horizonalVelocity = Vector3.zero;
                    if (inputComponent.inputMoveDirection != Vector3.zero)
                    {
                        var directionMoveComponent = World.GetComponent<DirectionMoveComponent>(inputComponent.entity);
                        if (directionMoveComponent != null)
                        {
                            horizonalVelocity = inputComponent.inputMoveDirection * directionMoveComponent.desiredSpeed;
                        }
                    }
                    jumpSystem.Jump(inputComponent.entity, horizonalVelocity);
                    ObjectPool<InputJumpCmd>.Instance.SaveObject(jumpCmd);
                }
                else if(cmd.cmdType == InputCommandType.Skill)
                {
                    var skillCmd = cmd as InputSkillCmd;
                    if (!comboSystem.CasterSkill(inputComponent.entity, skillCmd.skillId))
                    {
                        if (!cacheSkillSystem.CastSkill(inputComponent.entity, skillCmd.skillId))
                        {
                            skillSystem.CastSkill(inputComponent.entity, skillCmd.skillId);
                        }
                    }

                    ObjectPool<InputSkillCmd>.Instance.SaveObject(skillCmd);
                }
            }

            if (inputComponent.inputMoveDirection != Vector3.zero)
            {
                directionMoveSystem.Move(inputComponent.entity, inputComponent.inputMoveDirection);
            }
        }
    }
}