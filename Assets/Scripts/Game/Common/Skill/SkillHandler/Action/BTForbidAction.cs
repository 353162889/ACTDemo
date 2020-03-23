using Framework;
using NodeEditor;
using UnityEngine.UI;

namespace Game
{
    public class BTForbidActionData : IBTTimelineDurationData
    {
        [NEProperty("是否完成当前行为还原禁止")]
        public bool finishResume;
        public float forbidDuration;
        public ForbidType[] forbidArray;
        public float duration
        {
            get { return forbidDuration;}
            set { forbidDuration = value; }
        }
    }
    public class BTForbidAction : BTAction<SkillBTContext, BTForbidActionData>
    {
        internal protected struct InnerBTForbidActionData
        {
            public float time;
        }

        private static InnerBTForbidActionData DefaultActionData = new InnerBTForbidActionData() { time = 0 };

        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTForbidActionData data)
        {
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                if (data.forbidArray != null)
                {
                    for (int i = 0; i < data.forbidArray.Length; i++)
                    {
                        context.skillData.forbidance.Forbid(data.forbidArray[i]);
                    }
                }
            }

            BTStatus result;
            var cacheData =
                context.executeCache.GetCache<InnerBTForbidActionData>(btData.dataIndex, DefaultActionData);
            cacheData.time += context.deltaTime;
            context.executeCache.SetCache(btData.dataIndex, cacheData);

            if (cacheData.time >= data.duration)
            {
               
                result = BTStatus.Success;
            }
            else
            {
                result = BTStatus.Running;
            }

            return result;
        }

        protected override void Clear(SkillBTContext context, BTData btData, BTForbidActionData data)
        {
            if (!IsCleared(context, btData))
            {
                if (data.finishResume)
                {
                    if (data.forbidArray != null)
                    {
                        for (int i = 0; i < data.forbidArray.Length; i++)
                        {
                            context.skillData.forbidance.ResumeForbid(data.forbidArray[i]);
                        }
                    }
                }
            }

            base.Clear(context, btData, data);
        }
    }
}