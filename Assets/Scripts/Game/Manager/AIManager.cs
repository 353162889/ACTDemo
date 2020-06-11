using System;
using System.Collections.Generic;
using Framework;
using GameData;

namespace Game
{
    public class AIManager : BaseManager<AIManager>
    {
        private BTDataLoader<string> btDataLoader;

        public void LoadCfgs(Action callback)
        {
            if (btDataLoader == null) btDataLoader = new BTDataLoader<string>();
            btDataLoader.Clear();
            Dictionary<string, string> loadInfo = new Dictionary<string, string>();
            var lstMonsterCfg = ResCfgSys.Instance.GetCfgLst<ResMonster>();
            for (int i = 0; i < lstMonsterCfg.Count; i++)
            {
                var monsterCfg = lstMonsterCfg[i];
                if (!string.IsNullOrEmpty(monsterCfg.aiScript))
                {
                    loadInfo.Add(monsterCfg.aiScript, "Config/AIScript/" + monsterCfg.aiScript + ".bytes");
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

        public BTTreeData GetAIBTTreeData(string aiFile)
        {
            if (btDataLoader == null) return null;
            return btDataLoader.GetData(aiFile);
        }
    }
}