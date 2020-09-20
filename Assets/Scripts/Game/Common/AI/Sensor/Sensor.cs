using Unity.Entities;

namespace Game
{
    public abstract class Sensor
    {

        public void Init()
        {
            OnInit();
            if (this is IEventSensor)
            {
                ((IEventSensor)this).RegisterEvent();
            }
        }

        public void Destroy()
        {
            if (this is IEventSensor)
            {
                ((IEventSensor)this).UnregisterEvent();
            }
            OnDestroy();
        }

        protected virtual void OnInit()
        {
            
        }

        protected virtual void OnDestroy()
        {
            
        }
    }
}