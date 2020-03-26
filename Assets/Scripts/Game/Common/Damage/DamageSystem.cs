using Unity.Entities;

namespace Game
{
    public class DamageSystem : ComponentSystem
    {
        public DamageInfo CalDamageBySkill(Entity caster, Entity target, SkillData skillData, EntityHitInfo hitInfo)
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.caster = caster;
            damageInfo.target = target;
            damageInfo.damage = 0;
            damageInfo.hitInfo = hitInfo;
            return damageInfo;
        }

        protected override void OnUpdate()
        {
        }
    }
}