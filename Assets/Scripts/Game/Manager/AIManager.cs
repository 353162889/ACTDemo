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
        private UtilityAIDataLoader utilityAIDataLoader = new UtilityAIDataLoader();
        private UtilityAIDataLoader loadingUtilityAIDataLoader = new UtilityAIDataLoader();
        private Action finishCallback;

        public void LoadCfgs(Action callback)
        {
            finishCallback = callback;
            loadingBTDataLoader.Clear();
            Dictionary<string, string> loadInfo = new Dictionary<string, string>();
            var lstMonsterCfg = ResCfgSys.Instance.GetCfgLst<ResMonster>();
            for (int i = 0; i < lstMonsterCfg.Count; i++)
            {
                var monsterCfg = lstMonsterCfg[i];
                if (!string.IsNullOrEmpty(monsterCfg.aiScript) && !loadInfo.ContainsKey(monsterCfg.aiScript))
                {
                    loadInfo.Add(monsterCfg.aiScript, "Config/AIScript/" + monsterCfg.aiScript + ".bytes");
                }
            }
            loadInfo.Add("test_attack", "Config/AIScript/" + "test_attack" + ".bytes");
            loadInfo.Add("test_patrol", "Config/AIScript/" + "test_patrol" + ".bytes");
            loadingBTDataLoader.LoadResCfgs(loadInfo, () =>
            {
                var temp = btDataLoader;
                btDataLoader = loadingBTDataLoader;
                loadingBTDataLoader = temp;
                loadingBTDataLoader.Clear();
                CheckFinish();
            });

            loadingUtilityAIDataLoader.Clear();
            Dictionary<string, string> loadUtilityAIInfo = new Dictionary<string, string>();

            List<string> names = new List<string>();
            names.Add("test");
            for (int i = 0; i < names.Count; i++)
            {
                loadUtilityAIInfo.Add(names[i], "Config/UtilityAIScript/" + names[i] + ".bytes");
            }
            
            loadingUtilityAIDataLoader.LoadResCfgs(loadUtilityAIInfo, ()=>
            {
                var temp = utilityAIDataLoader;
                utilityAIDataLoader = loadingUtilityAIDataLoader;
                loadingUtilityAIDataLoader = temp;
                loadingUtilityAIDataLoader.Clear();
                CheckFinish();
            });
        }

        private void CheckFinish()
        {
            if (IsFinish())
            {
               finishCallback?.Invoke();
            }
        }

        public bool IsFinish()
        {
            return loadingBTDataLoader.IsFinish() && loadingUtilityAIDataLoader.IsFinish();
        }

        public BTTreeData GetAIBTTreeData(string aiFile)
        {
            return btDataLoader.GetData(aiFile);
        }

        public UtilityAIData GetUtilityAIData(string aiFile)
        {
            return utilityAIDataLoader.Get(aiFile);
        }
    }
}