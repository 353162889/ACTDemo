using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Game
{
    public class SkillComponent : DataComponent
    {
        public SkillData skillData;
        public Dictionary<int, float> dicCdInfo = new Dictionary<int, float>();
    }
}
