using System;
using System.Collections.Generic;
using Cinemachine.Utility;
using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public abstract class BaseEntityHit : MonoBehaviour,IPoolable
    {
        private static Dictionary<Entity, Collider> dicCacheEntity = new Dictionary<Entity, Collider>();
        protected Dictionary<Entity, Collider> m_dicEntity = new Dictionary<Entity, Collider>();
        public Dictionary<Entity, Collider> dicEntity
        {
            get { return m_dicEntity; }
        }
        protected List<Entity> m_lstUpdateEntity = new List<Entity>();
        public List<Entity> lstUpdateEntity
        {
            get { return m_lstUpdateEntity; }
        }

        protected abstract World world { get; }

        void OnTriggerEnter(Collider collider)
        {
            var entityMono = collider.GetComponentInParent<EntityMonoBehaviour>();
            if (entityMono != null)
            {
                var entity = entityMono.entity;
                if (world.EntityManager.Exists(entity) && !m_dicEntity.ContainsKey(entity))
                {
                    m_dicEntity.Add(entity, collider);
                    if (!m_lstUpdateEntity.Contains(entity))
                    {
                        m_lstUpdateEntity.Add(entity);
                    }
                }
            }
        }

        void OnTriggerExit(Collider collider)
        {
            var entityMono = collider.GetComponentInParent<EntityMonoBehaviour>();
            if (entityMono != null)
            {
                var entity = entityMono.entity;
                m_dicEntity.Remove(entity);
                m_lstUpdateEntity.Remove(entity);
            }
        }

        void Update()
        {
            foreach (var pair in m_dicEntity)
            {
                if (!world.EntityManager.Exists(pair.Key) || pair.Value == null)
                {
                    m_dicEntity.Remove(pair.Key);
                    m_lstUpdateEntity.Remove(pair.Key);
                }
            }
        }

        public virtual bool TryGetNewHitInfo(ref List<EntityHitInfo> lst)
        {
            if (m_lstUpdateEntity.Count <= 0) return false;
            lst.Clear();
            for (int i = 0; i < m_lstUpdateEntity.Count; i++)
            {
                var entity = m_lstUpdateEntity[i];
                dicCacheEntity.Add(entity, m_dicEntity[entity]);
            }
            TryGetNewHitInfo(dicCacheEntity, ref lst);
            dicCacheEntity.Clear();
            m_lstUpdateEntity.Clear();
            return lst.Count > 0;
        }

        protected abstract Vector3 GetStartRayPosition();

        public void TryGetNewHitInfo(Dictionary<Entity, Collider> dict, ref List<EntityHitInfo> lstInfo)
        {
            var pos = GetStartRayPosition();
            foreach (var pair in dict)
            {
                var center = pair.Value.bounds.center;
                Vector3 direction = center - pos;
                if (direction == Vector3.zero)
                {
                    direction = transform.forward;
                }
                direction.Normalize();
                Vector3 startPosition = center - direction * 1000;
                //使用射线检测该碰撞
                RaycastHit hit;
                if (pair.Value.Raycast(new Ray(startPosition, direction), out hit, float.MaxValue))
                {
                    EntityHitInfo info = new EntityHitInfo();
                    info.entity = pair.Key;
                    info.point = hit.point;
                    info.normal = hit.normal;
                    info.collider = hit.collider;
                    info.direct = transform.forward;
                    info.rayDirect = direction;
                    lstInfo.Add(info);
                }
            }
        }
        
        public virtual void Reset()
        {
            m_dicEntity.Clear();
            m_lstUpdateEntity.Clear();
        }

        void OnDestroy()
        {
            Reset();
        }
    }
}