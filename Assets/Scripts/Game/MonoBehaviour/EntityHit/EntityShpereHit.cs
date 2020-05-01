using System.Collections.Generic;
using Framework;
using Packages.Rider.Editor;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class EntitySphereHit : BaseEntityHit
    {
        private SphereCollider m_cSphereCollider;
        private World m_cWorld;
        public void Init(World world, Transform parent, Vector3 localPos, Quaternion localRot, float radius)
        {
            m_cWorld = world;
            transform.parent = parent;
            transform.localPosition = localPos;
            transform.localRotation = localRot;
            m_cSphereCollider = gameObject.AddComponentOnce<SphereCollider>();
            m_cSphereCollider.gameObject.layer = LayerDefine.AttackColliderInt;
            m_cSphereCollider.radius = radius;
            m_cSphereCollider.isTrigger = true;
        }

        protected override World world
        {
            get { return m_cWorld; }
        }

        protected override Vector3 GetStartRayPosition()
        {
            return m_cSphereCollider.transform.position;
        }

        public override void Reset()
        {
            base.Reset();
            m_cWorld = null;
        }
    }
}