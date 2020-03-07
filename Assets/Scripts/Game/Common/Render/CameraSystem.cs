using Cinemachine;
using Unity.Entities;

namespace Game
{
    public class CameraSystem : ComponentSystem
    {
        private CameraComponent cameraComponent;

        public void SetFollow(Entity entity, CinemachineVirtualCamera virtualCamera)
        {
            cameraComponent.followEntity = entity;
            cameraComponent.followVirtualCamera = virtualCamera;
            var prefabComponent = World.GetComponent<PrefabComponent>(entity);
            CinemachineBrain.SoloCamera = virtualCamera;
            if (prefabComponent != null)
            {
                CinemachineBrain.SoloCamera.LookAt = prefabComponent.transform;
                CinemachineBrain.SoloCamera.Follow = prefabComponent.transform;
            }
        }

        protected override void OnCreate()
        {
            cameraComponent = World.AddSingletonComponent<CameraComponent>();
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDestroy()
        {
            CinemachineBrain.SoloCamera = null;
        }
    }
}