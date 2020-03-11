using Unity.Entities;
using UnityEngine;

namespace Game
{
    public struct EntityHitInfo
    {
        public Entity entity;
        //碰撞点
        public Vector3 point;
        //碰撞区域的方向
        public Vector3 direct;
        //碰撞射线检测方向
        public Vector3 rayDirect;
        //碰撞点的法向量
        public Vector3 normal;
        //碰撞框
        public Collider collider;
    }
}