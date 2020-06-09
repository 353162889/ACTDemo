using System.Collections.Generic;
using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public interface IEntityRangeUpdate
    {
        bool isUpdate { get; set; }
    }

    public class EntityRangeTrigger : MonoBehaviour
    {
        private Dictionary<Entity, Collider> dicEntityInfo = new Dictionary<Entity, Collider>();
#if UNITY_EDITOR
        [SerializeField]
#endif
        private List<Entity> m_lstEntity = new List<Entity>();
        public List<Entity> lstEntity
        {
            get { return m_lstEntity; }
        }
        private World m_cWorld;
        private EntityTriggerListener m_sEnterListener;
        private EntityTriggerListener m_sExitListener;
        private IEntityRangeUpdate m_cEntityRangeUpdate;

        public void Init(World world, Collider enterTrigger, Collider exitTrigger, IEntityRangeUpdate entityRangeUpdate = null)
        {
            m_cWorld = world;
            dicEntityInfo.Clear();
            m_lstEntity.Clear();
            m_sEnterListener = enterTrigger.gameObject.AddComponentOnce<EntityTriggerListener>();
            m_sEnterListener.OnEntityEnter = OnEntityEnter;
            m_sExitListener = exitTrigger.gameObject.AddComponentOnce<EntityTriggerListener>();
            m_sExitListener.OnEntityExit = OnEntityExit;
            m_cEntityRangeUpdate = entityRangeUpdate;
        }

        private void SetUpdate()
        {
            if (m_cEntityRangeUpdate != null)
            {
                m_cEntityRangeUpdate.isUpdate = true;
            }
        }

        void OnEntityEnter(Entity entity, Collider collider)
        {
            if (!dicEntityInfo.ContainsKey(entity))
            {
                dicEntityInfo.Add(entity, collider);
                m_lstEntity.Add(entity);
                SetUpdate();
            }
        }

        void OnEntityExit(Entity entity, Collider collider)
        {
            if (dicEntityInfo.Remove(entity))
            {
                m_lstEntity.Remove(entity);
                SetUpdate();
            }
        }

        void Update()
        {
            var temp = ResetObjectPool<List<Entity>>.Instance.GetObject();
            foreach (var pair in dicEntityInfo)
            {
                if (!m_cWorld.EntityManager.Exists(pair.Key) || pair.Value == null)
                {
                    temp.Add(pair.Key);
                }
            }

            foreach (var entity in temp)
            {
                dicEntityInfo.Remove(entity);
                m_lstEntity.Remove(entity);
                SetUpdate();
            }
            ResetObjectPool<List<Entity>>.Instance.SaveObject(temp);
        }

        void OnDestroy()
        {
            m_cWorld = null;
            dicEntityInfo.Clear();
            m_lstEntity.Clear();
            m_sEnterListener = null;
            m_sExitListener = null;
            m_cEntityRangeUpdate = null;
        }
    }
}