using System;
using System.Collections.Generic;
using BTCore;
using Framework;

namespace Game
{
    public enum BuffExeStatus
    {
        Init,
        Running,
        Destroy
    }
    [Serializable]
    public class BuffData : IPoolable
    {
        public int index;
        public int buffId;
        public float buffTime;
        public BuffExeStatus status;
        public List<BuffPartData> lstPart = new List<BuffPartData>();
        public List<int> lstBuffStateIndex = new List<int>();
        public BTBlackBoard blackBoard = new BTBlackBoard();
        public BTExecuteCache executeCache = new BTExecuteCache();
        public BuffBTContext buffBTContext = new BuffBTContext();

        public void Reset()
        {
            index = 0;
            buffId = 0;
            status = BuffExeStatus.Init;
            lstPart.Clear();
            lstBuffStateIndex.Clear();
            buffTime = 0;
            blackBoard.Clear();
            executeCache.Clear();
            buffBTContext.Reset();
        }

        public void Detach()
        {
            this.status = BuffExeStatus.Destroy;
        }
    }
}