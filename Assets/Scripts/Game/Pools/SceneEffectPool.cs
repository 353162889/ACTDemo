using System;
using Framework;
using UnityEngine;

namespace Game
{
    public class SceneEffectPool : EffectPool<SceneEffectPool>
    {
        public void CacheObject(string name, int count, Action<string> callback)
        {
            string path = PathUtility.GetBattleEffectPath(name);
            base._CacheObject(path, true, count, callback);
        }

        public void RemoveCacheObject(string name, Action<string> callback)
        {
            string path = PathUtility.GetBattleEffectPath(name);
            base._RemoveCacheObject(path, callback);
        }

        public GameObject CreateEffect(string name, bool autoDestory, Transform parent = null)
        {
            string path = PathUtility.GetBattleEffectPath(name);
            return base._CreateEffect(path, autoDestory, parent);
        }
    }
}