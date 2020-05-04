using Framework;
using Unity.Entities;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Game
{
    public class InputSystem : ComponentSystem
    {
        private InputComponent inputComponent;
        private ComboSystem comboSystem;
        private CacheSkillSystem cacheSkillSystem;
        private SkillSystem skillSystem;
        private DirectionMoveSystem directionMoveSystem;
        private InAirSystem inAirSystem;
        private JumpSystem jumpSystem;
        private CameraSystem cameraSystem;
        private FaceSystem faceSystem;
        protected override void OnCreate()
        {
            inputComponent = World.AddSingletonComponent<InputComponent>();
            comboSystem = World.GetOrCreateSystem<ComboSystem>();
            cacheSkillSystem = World.GetOrCreateSystem<CacheSkillSystem>();
            skillSystem = World.GetOrCreateSystem<SkillSystem>();
            directionMoveSystem = World.GetOrCreateSystem<DirectionMoveSystem>();
            inAirSystem = World.GetOrCreateSystem<InAirSystem>();
            jumpSystem = World.GetOrCreateSystem<JumpSystem>();
            cameraSystem = World.GetOrCreateSystem<CameraSystem>();
            faceSystem = World.GetOrCreateSystem<FaceSystem>();
            ObjectPool<InputMoveDirectionCmd>.Instance.Init(5);
            ObjectPool<InputStopMoveDirectionCmd>.Instance.Init(5);
            ObjectPool<InputJumpCmd>.Instance.Init(2);
            ObjectPool<InputSkillCmd>.Instance.Init(10);
            ObjectPool<InputFaceCmd>.Instance.Init(5);
        }

        public void SetInputEntity(Entity entity)
        {
            inputComponent.inputEntity = entity;
        }

        public void InputMoveDirection(Vector2 moveDirection, bool changeFace)
        {
            var cmd = ObjectPool<InputMoveDirectionCmd>.Instance.GetObject();
            Vector3 direction = new Vector3(moveDirection.x, 0, moveDirection.y);
            direction.Normalize();
            cmd.moveDirection = direction;
            cmd.changeFace = changeFace;
            inputComponent.queueCmd.Enqueue(cmd);
        }

        public void OnScreenAxisUpdate(float xAxis, float yAxis)
        {
            cameraSystem.OnScreenAxisUpdate(xAxis, yAxis);
        }


        public void OnScreenSroll(float scroll)
        {
            cameraSystem.OnScreenScroll(scroll);
        }

        public void ChangeFace(Vector3 faceDirection, bool immediately)
        {
            var cmd = ObjectPool<InputFaceCmd>.Instance.GetObject();
            cmd.faceDirection = faceDirection;
            cmd.immediately = immediately;
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

        private Vector3 GetTransformInputDirection(Vector3 inputMoveDirection)
        {
            var worldDirection = cameraSystem.TransformDirection(inputMoveDirection);
            worldDirection.y = 0;
            worldDirection.Normalize();
            return worldDirection;
        }

        protected override void OnUpdate()
        {
            while (inputComponent.queueCmd.Count > 0)
            {
                var cmd = inputComponent.queueCmd.Dequeue();
                if (cmd.cmdType == InputCommandType.Face)
                {
                    var faceCmd = cmd as InputFaceCmd;
                    faceSystem.InputFace(inputComponent.inputEntity, faceCmd.faceDirection, faceCmd.immediately);
                    ObjectPool<InputFaceCmd>.Instance.SaveObject(faceCmd);
                }
                else if (cmd.cmdType == InputCommandType.MoveDirection)
                {
                    var moveDirectionCmd = cmd as InputMoveDirectionCmd;
                    inputComponent.inputMoveDirection = moveDirectionCmd.moveDirection;
                    inputComponent.inputMoveDirection.Normalize();
                    inputComponent.inputMoveChangeFace = moveDirectionCmd.changeFace;
                    ObjectPool<InputMoveDirectionCmd>.Instance.SaveObject(moveDirectionCmd);
                }
                else if (cmd.cmdType == InputCommandType.StopMoveDirection)
                {
                    var stopMoveDirectionCmd = cmd as InputStopMoveDirectionCmd;
                    inputComponent.inputMoveDirection = Vector3.zero;
                    inputComponent.inputMoveChangeFace = false;
                    directionMoveSystem.StopMove(inputComponent.inputEntity);
                    inAirSystem.StopInputMove(inputComponent.inputEntity);
                    ObjectPool<InputStopMoveDirectionCmd>.Instance.SaveObject(stopMoveDirectionCmd);
                }
                else if(cmd.cmdType == InputCommandType.Jump)
                {
                    var jumpCmd = cmd as InputJumpCmd;
                    Vector3 horizonalVelocity = Vector3.zero;
                    if (inputComponent.inputMoveDirection != Vector3.zero)
                    {
                        var directionMoveComponent = World.GetComponent<DirectionMoveComponent>(inputComponent.inputEntity);
                        if (directionMoveComponent != null)
                        {
                            horizonalVelocity = GetTransformInputDirection(inputComponent.inputMoveDirection) * directionMoveComponent.desiredSpeed;
                        }
                    }
                    jumpSystem.Jump(inputComponent.inputEntity, horizonalVelocity);
                    ObjectPool<InputJumpCmd>.Instance.SaveObject(jumpCmd);
                }
                else if(cmd.cmdType == InputCommandType.Skill)
                {
                    var skillCmd = cmd as InputSkillCmd;
                    var skillId = skillCmd.skillId;
                    skillId = skillSystem.GetReplaceSkill(inputComponent.inputEntity, skillId);
                    if (!comboSystem.CasterSkill(inputComponent.inputEntity, skillId))
                    {
                        if (!cacheSkillSystem.CastSkill(inputComponent.inputEntity, skillId))
                        {
                            skillSystem.CastSkill(inputComponent.inputEntity, skillId);
                        }
                    }

                    ObjectPool<InputSkillCmd>.Instance.SaveObject(skillCmd);
                }
            }

            if (inputComponent.inputMoveDirection != Vector3.zero)
            {
                var transformInputDirection = GetTransformInputDirection(inputComponent.inputMoveDirection);
                directionMoveSystem.Move(inputComponent.inputEntity, transformInputDirection, inputComponent.inputMoveChangeFace);
                inAirSystem.InputMove(inputComponent.inputEntity, transformInputDirection);
            }
        }
    }
}