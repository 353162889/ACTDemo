using System;
using System.Collections.Generic;
using Framework;
using GameData;

namespace Game
{
    public class BuffManager : BaseManager<BuffManager>
    {
        private BTDataLoader<int> btDataLoader;
        private BTDataLoader<int> btPartDataLoader;
        private int finishCount;

        public void LoadCfgs(Action callback)
        {
            if (btDataLoader == null) btDataLoader = new BTDataLoader<int>();
            if(btPartDataLoader == null) btPartDataLoader = new BTDataLoader<int>();
            btDataLoader.Clear();
            btPartDataLoader.Clear();
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
                    if (resPart != null)
                    {
                        loadBuffPartInfo.Add(resPart.id, "Config/BuffScript/BuffPartScript/" + resPart.script + ".bytes");
                    }
                }
            }
            btDataLoader.LoadResCfgs(loadBuffInfo, () =>
            {
                finishCount++;
                if (finishCount == 2)
                {
                    callback?.Invoke();
                }
            });
            btPartDataLoader.LoadResCfgs(loadBuffPartInfo, () =>
            {
                finishCount++;
                if (finishCount == 2)
                {
                    callback?.Invoke();
                }
            });
        }

        public bool IsFinish()
        {
            if (btDataLoader != null && btPartDataLoader != null)
            {
                return btDataLoader.IsFinish() && btPartDataLoader.IsFinish();
            }

            return true;
        }

        public BTTreeData GetBuffBTTreeData(int id)
        {
            if (btDataLoader == null) return null;
            return btDataLoader.GetData(id);
        }

        public BTTreeData GetBuffPartBTTreeData(int id)
        {
            if (btPartDataLoader == null) return null;
            return btPartDataLoader.GetData(id);
        }
    }
}