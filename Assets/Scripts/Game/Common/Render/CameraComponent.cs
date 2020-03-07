using Cinemachine;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class CameraComponent : DataComponent
    {
        public Camera camera;
        public Entity followEntity;
        public CinemachineVirtualCamera followVirtualCamera;
    }
}