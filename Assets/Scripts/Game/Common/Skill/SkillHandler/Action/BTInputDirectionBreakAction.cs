using Cinemachine.Utility;
using Framework;

namespace Game
{
    public class BTInputDirectionBreakActionData : IBTTimelineDurationData
    {
        public float breakDuration;
        public float duration
        {
            get { return breakDuration;}
            set { breakDuration = value; }
        }
    }
    public class BTInputDirectionBreakAction : BTAction<SkillBTContext, BTInputDirectionBreakActionData>
    {
        internal protected struct InnerBTInputDirectionBreakActionData
        {
            public float time;
        }

        private static InnerBTInputDirectionBreakActionData DefaultActionData = new InnerBTInputDirectionBreakActionData() { time = 0 };
        protected override BTStatus Handler(SkillBTContext context, BTData btData, BTInputDirectionBreakActionData data)
        {
            var directionMoveComponent = context.world.GetComponent<DirectionMoveComponent>(context.skillComponent.entity);
            if (directionMoveComponent != null && !directionMoveComponent.inputDirection.AlmostZero())
            {
                context.skillComponent.skillData.isBreak = true;
            }

            BTStatus result;
            var cacheData =
                context.executeCache.GetCache<InnerBTInputDirectionBreakActionData>(btData.dataIndex, DefaultActionData);
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
            if (result != BTStatus.Running)
            {
                this.Clear(context, btData);
            }
            return result;
        }
    }
}