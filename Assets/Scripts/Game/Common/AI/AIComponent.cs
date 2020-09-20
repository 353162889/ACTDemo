using System.Collections.Generic;
using BTCore;
using Framework;

namespace Game
{

    public static class AIBlackBoardKeys
    {
        public static string TargetPosition = "TargetPosition";
        public static string SkillIDList = "SkillIDList";
    }

    public class AIComponent : DataComponent
    {
        public AIStateType aiStateType = AIStateType.Running;
        public AIWorldState worldState = new AIWorldState();
        public AIBTContext btContext = new AIBTContext();
        public UtilityContext utilityContext = new UtilityContext();
        public List<EntitySensor> lstSensor = new List<EntitySensor>();
        public List<IUpdateSensor> lstUpdateSensor = new List<IUpdateSensor>();

        public override void OnDestroy()
        {
            for (int i = 0; i < lstSensor.Count; i++)
            {
                lstSensor[i].Destroy();
            }
        }
    }
}