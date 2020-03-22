using Framework;

namespace Game
{
    public class BuffPartData : IPoolable
    {
        public int buffPartId;
        public float curDurationTime;
        public float curTriggerTime;
        public int curTriggerCount;
        public bool enabled;

        public BuffBTContext buffBTContext = new BuffBTContext();
        public void Reset()
        {
            buffPartId = 0;
            curDurationTime = 0;
            curTriggerTime = 0;
            curTriggerCount = 0;
            enabled = false;
            buffBTContext.Reset();
        }

        public void ResetEnabled(bool enabled)
        {
            this.enabled = enabled;
            curDurationTime = 0;
            curTriggerTime = 0;
            curTriggerCount = 0;
        }
    }
}