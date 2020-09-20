using Unity.Entities;

namespace Game
{
    public abstract class GlobalSensor : Sensor
    {
        public World world { get; private set; }
        public GlobalSensor(World world)
        {
            this.world = world;
            this.Init();
        }

        public abstract void UpdateEntity(Entity entity);
    }
}