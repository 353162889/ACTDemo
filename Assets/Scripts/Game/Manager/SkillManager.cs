using System;
using System.Collections.Generic;
using Framework;
using GameData;

namespace Game
{
    public class SkillManager : BaseManager<SkillManager>
    {
        private BTDataLoader<int> btDataLoader;

        public void LoadCfgs(Action callback)
        {
            if(btDataLoader == null)btDataLoader = new BTDataLoader<int>();
            btDataLoader.Clear();
            Dictionary<int,string> loadInfo = new Dictionary<int, string>();
            var lstSkill = ResCfgSys.Instance.GetCfgLst<ResSkill>();
            for (int i = 0; i < lstSkill.Count; i++)
            {
                var resSkill = lstSkill[i];
                if (!string.IsNullOrEmpty(resSkill.script))
                {
                    loadInfo.Add(resSkill.id, "Config/SkillScript/"+resSkill.script+".bytes");
                }
            }
            btDataLoader.LoadResCfgs(loadInfo, () =>
            {
                callback?.Invoke();
            });
        }

        public bool IsFinish()
        {
            return btDataLoader != null ? btDataLoader.IsFinish() : true;
        }

        public BTTreeData GetSkillBTTreeData(int id)
        {
            if (btDataLoader == null) return null;
            return btDataLoader.GetData(id);
        }

        public BTSkillRootData GetSkillRootData(BTTreeData treeData)
        {
            if (treeData != null)
                return treeData.rootData.data as BTSkillRootData;
            return null;
        }

        public BTSkillRootData GetSkillRootData(int id)
        {
            var treeData = GetSkillBTTreeData(id);
            return GetSkillRootData(treeData);
        }
    }
}