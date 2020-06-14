using System;
using System.Collections.Generic;
using Framework;
using GameData;

namespace Game
{
    public class AIManager : BaseManager<AIManager>
    {
        private BTDataLoader<string> btDataLoader = new BTDataLoader<string>();
        private BTDataLoader<string> loadingBTDataLoader = new BTDataLoader<string>();

        public void LoadCfgs(Action callback)
        {
            loadingBTDataLoader.Clear();
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

        public BTTreeData GetAIBTTreeData(string aiFile)
        {
            return btDataLoader.GetData(aiFile);
        }
    }
}