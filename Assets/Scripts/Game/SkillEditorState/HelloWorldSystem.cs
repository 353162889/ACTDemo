using System.Diagnostics;
using Framework;
using Unity.Entities;

namespace Game
{
    public class HelloWorldSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            CLog.Log("Hello World");
        }

        protected override void OnUpdate()
        {
        }
    }
}