using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class PCInputSystem : ComponentSystem
    {
        private PCInputComponent pcInputComponent;
        private InputSystem inputSystem;
        protected override void OnCreate()
        {
            pcInputComponent = World.AddSingletonComponent<PCInputComponent>();
            inputSystem = World.GetOrCreateSystem<InputSystem>();
        }

        private static Dictionary<KeyCode, Vector2> directionKeyCodeMap = new Dictionary<KeyCode, Vector2>()
        {
            {KeyCode.A, Vector2.left},
            {KeyCode.D, Vector2.right},
            {KeyCode.W, Vector2.up},
            {KeyCode.S, Vector2.down},

            {KeyCode.LeftArrow, Vector2.left},
            {KeyCode.RightArrow, Vector2.right},
            {KeyCode.UpArrow, Vector2.up},
            {KeyCode.DownArrow, Vector2.down},
        };

        private static KeyCode JumpKeyCode = KeyCode.Space;

        private static Dictionary<KeyCode, int> dicSkillCode = new Dictionary<KeyCode, int>()
        {
            {KeyCode.J, 1},
            {KeyCode.K, 1001},
        };

        protected override void OnUpdate()
        {
            Vector2 keyDirection = Vector2.zero;
            foreach (var pairValue in directionKeyCodeMap)
            {
                if (Input.GetKey(pairValue.Key))
                {
                    keyDirection += pairValue.Value;
                }
            }

            if (pcInputComponent.lastInputDirection != keyDirection)
            {
                pcInputComponent.lastInputDirection = keyDirection;
                if (keyDirection == Vector2.zero)
                {
                    inputSystem.InputStopMoveDirection();
                }
                else
                {
                    inputSystem.InputMoveDirection(keyDirection);
                }
            }

            if (Input.GetKeyDown(JumpKeyCode))
            {
                inputSystem.InputJump();
            }

            foreach (var pairValue in dicSkillCode)
            {
                if (Input.GetKeyDown(pairValue.Key))
                {
                    inputSystem.InputSkill(pairValue.Value);
                    break;
                }
            }
        }
    }
}