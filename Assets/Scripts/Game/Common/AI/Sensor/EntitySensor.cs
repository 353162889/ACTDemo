using Unity.Entities;

namespace Game
{
    public abstract class EntitySensor : Sensor
    {
        public World world { get; private set; }
        public Entity entity { get; private set; }
        public AIWorldState worldState { get; private set; }

        public EntitySensor(World world, Entity entity, AIWorldState worldState)
        {
            this.world = world;
            this.entity = entity;
            this.worldState = worldState;
            this.Init();
        }
    }
}