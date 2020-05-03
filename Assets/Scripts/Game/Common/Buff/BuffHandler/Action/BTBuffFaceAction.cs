using Cinemachine.Utility;
using Framework;

namespace Game
{
    public class BTBuffFaceData
    {
        public BuffDirectionType directionType;
        public bool opposite;
        public bool immediately;
    }
    public class BTBuffFaceAction : BTAction<BuffBTContext, BTBuffFaceData>
    {
        protected override BTStatus Handler(BuffBTContext context, BTData btData, BTBuffFaceData data)
        {
            var entity = context.buffComponent.componentEntity;
            var faceSystem = context.world.GetExistingSystem<FaceSystem>();
            BTExecuteStatus exeStatus = context.executeCache.GetExecuteStatus(btData.dataIndex);
            if (exeStatus == BTExecuteStatus.Ready)
            {
                if (faceSystem != null)
                {
                    var direction = BuffHandlerUtility.GetBuffDirection(context, data.directionType);
                    if (data.opposite) direction = -direction;
                    faceSystem.FaceTo(entity, direction, data.immediately);
                }
            }
            if (!data.immediately && faceSystem != null && faceSystem.IsRotating(entity))
            {
                return BTStatus.Running;
            }
            return BTStatus.Success;
        }
    }
}