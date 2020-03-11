using System.Collections.Generic;
using Framework;
using Packages.Rider.Editor;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class EntityBoxHit : BaseEntityHit
    {
        private BoxCollider m_cBoxCollider;
        private World m_cWorld;
        public void Init(World world, Transform parent, Vector3 localPos, Quaternion localRot, Vector3 size)
        {
            m_cWorld = world;
            transform.parent = parent;
            transform.localPosition = localPos;
            transform.localRotation = localRot;
            m_cBoxCollider = gameObject.AddComponentOnce<BoxCollider>();
            m_cBoxCollider.gameObject.layer = LayerDefine.AttackColliderInt;
            m_cBoxCollider.size = size;
            m_cBoxCollider.isTrigger = true;
        }

        protected override World world
        {
            get { return m_cWorld; }
        }

        protected override Vector3 GetStartRayPosition()
        {
            return m_cBoxCollider.bounds.center - transform.forward * 1000;
        }

        public override void Reset()
        {
            base.Reset();
            m_cWorld = null;
        }
    }
}