using System;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class EntityTriggerListener : MonoBehaviour
    {
        public Action<Entity, Collider> OnEntityEnter;
        public Action<Entity, Collider> OnEntityExit;

        void OnTriggerEnter(Collider collider)
        {
            var entityMono = collider.GetComponentInParent<EntityMonoBehaviour>();
            if (entityMono != null)
            {
                var entity = entityMono.entity;
                if (OnEntityEnter != null)
                {
                    OnEntityEnter.Invoke(entity, collider);
                }
            }
        }

        void OnTriggerExit(Collider collider)
        {
            var entityMono = collider.GetComponentInParent<EntityMonoBehaviour>();
            if (entityMono != null)
            {
                var entity = entityMono.entity;
                if (OnEntityExit != null)
                {
                    OnEntityExit.Invoke(entity, collider);
                }
            }
        }

        void OnDestroy()
        {
            OnEntityEnter = null;
            OnEntityExit = null;
        }
    }
}