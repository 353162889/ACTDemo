using System;
using System.Collections.Generic;
using Framework;
using GameData;

namespace Game
{
    public class SkillManager : BaseManager<SkillManager>
    {
        private BTDataLoader<int> btDataLoader = new BTDataLoader<int>();
        private BTDataLoader<int> loadingBTDataLoader = new BTDataLoader<int>();

        public void LoadCfgs(Action callback)
        {
            loadingBTDataLoader.Clear();
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
            loadingBTDataLoader.LoadResCfgs(loadInfo, () =>
            {
                var temp = btDataLoader;
                btDataLoader = loadingBTDataLoader;
                loadingBTDataLoader = temp;
                loadingBTDataLoader.Clear();
                callback?.Invoke();
            });
        }

        public bool IsFinish()
        {
            return loadingBTDataLoader.IsFinish();
        }

        public BTTreeData GetSkillBTTreeData(int id)
        {
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