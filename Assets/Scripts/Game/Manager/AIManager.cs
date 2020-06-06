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
            string aiName = "test_ai";
            loadInfo.Add(aiName, "Config/AIScript/" + aiName + ".bytes");
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