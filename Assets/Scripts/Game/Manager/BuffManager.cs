using System;
using System.Collections.Generic;
using Framework;
using GameData;

namespace Game
{
    public class BuffManager : BaseManager<BuffManager>
    {
        private BTDataLoader<int> btDataLoader = new BTDataLoader<int>();
        private BTDataLoader<int> loadingBTDataLoader = new BTDataLoader<int>();
        private BTDataLoader<int> btPartDataLoader = new BTDataLoader<int>();
        private BTDataLoader<int> loadingBTPartDataLoader = new BTDataLoader<int>();
        private int finishCount;

        public void LoadCfgs(Action callback)
        {
            loadingBTDataLoader.Clear();
            loadingBTPartDataLoader.Clear();
            finishCount = 0;
            Dictionary<int, string> loadBuffInfo = new Dictionary<int, string>();
            Dictionary<int, string> loadBuffPartInfo = new Dictionary<int, string>();
            var lstBuff = ResCfgSys.Instance.GetCfgLst<ResBuff>();
            for (int i = 0; i < lstBuff.Count; i++)
            {
                var resBuff = lstBuff[i];
                if (!string.IsNullOrEmpty(resBuff.script))
                {
                    loadBuffInfo.Add(resBuff.id, "Config/BuffScript/" + resBuff.script + ".bytes");
                }

                foreach (var partId in resBuff.parts)
                {
                    var resPart = ResCfgSys.Instance.GetCfg<ResBuffPartLogic>(partId);
                    if (resPart != null && !string.IsNullOrEmpty(resPart.script))
                    {
                        loadBuffPartInfo.Add(resPart.id, "Config/BuffScript/BuffPartScript/" + resPart.script + ".bytes");
                    }
                }
            }
            loadingBTDataLoader.LoadResCfgs(loadBuffInfo, () =>
            {
                var temp = btDataLoader;
                btDataLoader = loadingBTDataLoader;
                loadingBTDataLoader = temp;
                loadingBTDataLoader.Clear();
                finishCount++;
                if (finishCount == 2)
                {
                    callback?.Invoke();
                }
            });
            loadingBTPartDataLoader.LoadResCfgs(loadBuffPartInfo, () =>
            {
                var temp = btPartDataLoader;
                btPartDataLoader = loadingBTPartDataLoader;
                loadingBTPartDataLoader = temp;
                loadingBTPartDataLoader.Clear();
                finishCount++;
                if (finishCount == 2)
                {
                    callback?.Invoke();
                }
            });
        }

        public bool IsFinish()
        {
            return loadingBTDataLoader.IsFinish() && loadingBTPartDataLoader.IsFinish();
        }

        public BTTreeData GetBuffBTTreeData(int id)
        {
            return btDataLoader.GetData(id);
        }

        public BTTreeData GetBuffPartBTTreeData(int id)
        {
            return btPartDataLoader.GetData(id);
        }
    }
}