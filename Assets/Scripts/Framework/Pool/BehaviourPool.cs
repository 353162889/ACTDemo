using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class BehaviourPool<T> : Singleton<BehaviourPool<T>> where T : MonoBehaviour,IPoolable
    {
        private Queue<T> _pool;
        private int _capicity;
        private bool _inited;
        private Transform _parent;

        public void Init(int capicity, Transform parent)
        {
            if (!_inited)
            {
                this._capicity = capicity;
                _pool = new Queue<T>(_capicity);
                _inited = true;
                _parent = parent;
            }
        }

        public void SetCapicaty(int capicity)
        {
            this._capicity = capicity;
        }

        public T GetObject(Transform parent = null)
        {
            T obj = null;
            while (obj == null && _pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            if(obj == (UnityEngine.Object)null)
            {
                GameObject go = new GameObject();
                obj = go.AddComponentOnce<T>();
            }
            if(parent != null)
            {
                GameObjectUtil.AddChildToParent(parent.gameObject, obj.gameObject);
            }
            else
            {
                obj.transform.parent = null;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localEulerAngles = Vector3.zero;
                obj.transform.localScale = Vector3.one;
            }
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void SaveObject(T obj)
        {
            if (obj == (UnityEngine.Object) null) return;
            obj.Reset();
            obj.gameObject.SetActive(false);
            GameObjectUtil.AddChildToParent(_parent.gameObject, obj.gameObject);
            if (_pool.Count < _capicity)
            {
                _pool.Enqueue(obj);
            }
            else
            {
                CLog.Log("<color='yellow'>" + typeof(T) + " over capicity:" + _capicity + "</color>");
            }
        }
    }
}
