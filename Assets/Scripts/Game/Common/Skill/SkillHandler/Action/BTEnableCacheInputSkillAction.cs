using Framework;
using NodeEditor;

namespace Game
{
    public class BTEnableCacheInputSkillActionData : IBTTimelineDurationData
    {
        [NEProperty("是否完成当前行为还原禁止")]
        public bool finishResume;
        public float comboDuration;
        public float duration
        {
            get { return comboDuration; }
            set { comboDuration = value; }
        }
    }
    public class BTEnableCacheInputSkillAction : BTAction<SkillBTContext, BTEnableCacheInputSkillActionData>
    {

        internal protected struct InnerBTEnableCacheInputSkillActionData
        {
            public float time;
        }

        private static InnerBTEnableCacheInputSkillActionData DefaultActionData = new InnerBTEnableCacheInputSkillActionData() { time = 0 };
        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTEnableCacheInputSkillActionData data)
        {
            var cacheData =
                context.executeCache.GetCache<InnerBTEnableCacheInputSkillActionData>(btData.dataIndex, DefaultActionData);
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                context.skillData.enableInputSkill = true;
            }

            BTStatus result;

            cacheData.time += context.deltaTime;
            context.executeCache.SetCache(btData.dataIndex, cacheData);

            if (cacheData.time >= data.duration || !data.finishResume)
            {
                result = BTStatus.Success;
            }
            else
            {
                result = BTStatus.Running;
            }
            return result;
        }

        protected override void Clear(SkillBTContext context, BTData btData, BTEnableCacheInputSkillActionData data)
        {
            if (!IsCleared(context, btData))
            {
                if (data.finishResume)
                {
                    context.skillData.enableInputSkill = false;
                }
            }
            base.Clear(context, btData, data);
        }
    }
}