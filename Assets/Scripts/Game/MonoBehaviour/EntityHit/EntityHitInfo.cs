using Unity.Entities;
using UnityEngine;

namespace Game
{
    public struct EntityHitInfo
    {
        public static EntityHitInfo Null = new EntityHitInfo(){entity = Entity.Null, point = Vector3.zero, direct = Vector3.forward, rayDirect = Vector3.forward, normal = Vector3.up, collider = null};

        public Entity entity;
        /// <summary>
        /// 碰撞点
        /// </summary>
        public Vector3 point;
        /// <summary>
        /// 碰撞区域的方向
        /// </summary>
        public Vector3 direct;
        /// <summary>
        /// 碰撞射线检测方向
        /// </summary>
        public Vector3 rayDirect;
        /// <summary>
        /// 碰撞点的法向量
        /// </summary>
        public Vector3 normal;
        /// <summary>
        /// 碰撞框中心位置
        /// </summary>
        public Vector3 colliderCenter;
        /// <summary>
        /// 碰撞框
        /// </summary>
        public Collider collider;

        public bool IsEnabled()
        {
            return entity != Entity.Null;
        }
    }
}