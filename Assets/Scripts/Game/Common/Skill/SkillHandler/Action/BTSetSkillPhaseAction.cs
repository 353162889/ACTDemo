using Framework;
using NodeEditor;

namespace Game
{
    public class BTSetSkillPhaseActionData : IBTTimelineDurationData
    {
        [NEProperty("是否完成当前行为还原禁止")]
        public bool finishResume;
        public float phaseDuration;
        public SkillPhaseType phaseType = SkillPhaseType.Backswing;
        public float duration
        {
            get { return phaseDuration;}
            set { phaseDuration = value; }
        }
    }
    public class BTSetSkillPhaseAction : BTAction<SkillBTContext, BTSetSkillPhaseActionData>
    {
        internal protected struct InnerBTSetSkillPhaseActionData
        {
            public float time;
            public SkillPhaseType oldPhaseType;
        }

        private static InnerBTSetSkillPhaseActionData DefaultActionData = new InnerBTSetSkillPhaseActionData() { time = 0, oldPhaseType = SkillPhaseType.Normal };

        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTSetSkillPhaseActionData data)
        {
            var cacheData =
                context.executeCache.GetCache<InnerBTSetSkillPhaseActionData>(btData.dataIndex, DefaultActionData);
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                cacheData.oldPhaseType = context.skillData.phase;
                if (data.phaseType > context.skillData.phase)
                {
                    context.skillData.phase = data.phaseType;
                }
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

        protected override void Clear(SkillBTContext context, BTData btData, BTSetSkillPhaseActionData data)
        {
            if (!IsCleared(context, btData))
            {
                if (data.finishResume)
                {
                    var cacheData =
                        context.executeCache.GetCache<InnerBTSetSkillPhaseActionData>(btData.dataIndex, DefaultActionData);
                    context.skillData.phase = cacheData.oldPhaseType;
                }
            }
            base.Clear(context, btData, data);
        }
    }
}