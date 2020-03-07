﻿using UnityEngine;

namespace Game
{
    public class GravityComponent : DataComponent
    {
        //重力大小
        public float gravity = -15.8f;
        //是否使用重力
        public bool useGravity = true;

        //重力附加的当前速度
        public Vector3 velocity = Vector3.zero;
    }
}