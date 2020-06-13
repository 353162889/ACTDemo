using Framework;
using NodeEditor;

namespace Game
{
    [NENodeBtn("设置有效范围", "NETreeWindowBtnExtension", "SetSkillEffectiveRange")]
    public class BTSkillRootData
    {
        //技能的有效范围
        public float effectiveRange;
    }

    public class BTSkillRoot : BTDecorator<SkillBTContext, BTSkillRootData>
    {
    }
}