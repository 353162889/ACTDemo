using System;
using Framework;
using UnityEngine.InputSystem.Utilities;

namespace Game
{
    [Serializable]
    public class BTUtilityActionData : IUtilityActionData
    {
        public string mName;
        public string mAIFile;
    }
    public class BTUtilityAction : UtilityBase<BTUtilityActionData>, IUtilityAction
    {
        public string name
        {
            get { return this.convertData.mName; }
        }
        public string aiFile
        {
            get { return this.convertData.mAIFile; }
        }

        protected override void OnInit(BTUtilityActionData data)
        {
        }
    }
}