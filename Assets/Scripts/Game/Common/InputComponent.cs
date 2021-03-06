﻿using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class InputComponent : DataComponent
    {
        public Entity inputEntity;
        public Queue<IInputCommand> queueCmd = new Queue<IInputCommand>();
        public Vector3 inputMoveDirection = Vector3.zero;
        public bool inputMoveChangeFace = false;

        public float testRotateX = 300;
    }
}