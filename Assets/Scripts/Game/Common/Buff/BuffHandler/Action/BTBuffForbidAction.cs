using Framework;
using NodeEditor;
using UnityEngine.UI;

namespace Game
{
    public class BTBuffForbidActionData : IBTTimelineDurationData
    {
        [NEProperty("是否完成当前行为还原禁止")]
        public bool finishResume;
        public float forbidDuration;
        public ForbidType[] forbidArray;
        public float duration
        {
            get { return forbidDuration; }
            set { forbidDuration = value; }
        }
    }
    public class BTBuffForbidAction : BTAction<BuffBTContext, BTBuffForbidActionData>
    {
        internal protected struct InnerBTBuffForbidActionData
        {
            public float time;
        }

        private static InnerBTBuffForbidActionData DefaultActionData = new InnerBTBuffForbidActionData() { time = 0 };

        protected override BTStatus Handler(BuffBTContext context, BTData btData, BTBuffForbidActionData data)
        {
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                if (data.forbidArray != null)
                {
                    for (int i = 0; i < data.forbidArray.Length; i++)
                    {
                        if (context.IsPartScript())
                        {
                            context.GetPartData().forbidance.Forbid(data.forbidArray[i]);
                        }
                        else
                        {
                            context.buffData.forbidance.Forbid(data.forbidArray[i]);
                        }
                        
                    }
                }
            }

            BTStatus result;
            var cacheData =
                context.executeCache.GetCache<InnerBTBuffForbidActionData>(btData.dataIndex, DefaultActionData);
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

        protected override void Clear(BuffBTContext context, BTData btData, BTBuffForbidActionData data)
        {
            if (!IsCleared(context, btData))
            {
                if (data.finishResume)
                {
                    if (data.forbidArray != null)
                    {
                        for (int i = 0; i < data.forbidArray.Length; i++)
                        {
                            if (context.IsPartScript())
                            {
                                context.GetPartData().forbidance.ResumeForbid(data.forbidArray[i]);
                            }
                            else
                            {
                                context.buffData.forbidance.ResumeForbid(data.forbidArray[i]);
                            }
                        }
                    }
                }
            }

            base.Clear(context, btData, data);
        }
    }
}