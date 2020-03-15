using UnityEngine;

namespace Game
{
    public class GravityComponent : DataComponent
    {
        //重力大小
        public float gravity = -30f;
        //是否使用重力
        public bool useGravity = true;
    }
}