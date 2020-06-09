using System.Collections.Generic;
using Unity.Entities;

namespace Game
{
    public class TargetTriggerComponent : DataComponent, IEntityRangeUpdate
    {
        //当前目标列表
        public List<Entity> lstEntity = new List<Entity>();
        //目标选择器
        public int filterId;
        //范围检测trigger
        public EntityRangeTrigger rangeTrigger;
        public bool isUpdate { get; set; }
    }
}