using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace Game
{
    public struct contactInfo
    {
        public Vector3 normal;
        public Vector3 point;
    }

    public class CollisionInfo : IPoolable
    {
        public Collider collider;
        public List<contactInfo> contacts = new List<contactInfo>();
        public void Reset()
        {
            collider = null;
            contacts.Clear();
        }
    }

    public class CollisionListener : MonoBehaviour
    {
        public List<CollisionInfo> lstCollisionInfos { get; } = new List<CollisionInfo>();

        void OnCollisionEnter(Collision collision)
        {
            if (!ObjectPool<CollisionInfo>.Instance.inited)
            {
                ObjectPool<CollisionInfo>.Instance.Init(30);
            }
            var collisionInfo = ObjectPool<CollisionInfo>.Instance.GetObject();
            collisionInfo.collider = collision.collider;
            for (int i = 0; i < collision.contactCount; i++)
            {
                var contact = collision.GetContact(i);
                var contactInfo = new contactInfo();
                contactInfo.point = contact.point;
                contactInfo.normal = contact.normal;
                collisionInfo.contacts.Add(contactInfo);
            }
            lstCollisionInfos.Add(collisionInfo);
        }

        void OnCollisionExit(Collision collision)
        {
            for (int i = lstCollisionInfos.Count - 1; i > -1; i--)
            {
                var collisionInfo = lstCollisionInfos[i];
                if (collisionInfo.collider == collision.collider)
                {
                    lstCollisionInfos.RemoveAt(i);
                    ObjectPool<CollisionInfo>.Instance.SaveObject(collisionInfo);
                    break;
                }
            }
        }
    }
}