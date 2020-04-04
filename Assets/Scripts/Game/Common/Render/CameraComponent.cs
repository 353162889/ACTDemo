using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Entities;
using UnityEngine;

namespace Game
{

    public static class CameraStrategy
    {
        public static string NormalWalkCamera = "NormalWalkCamera";
    }

    public class CameraComponent : DataComponent
    {
        public Camera camera;
        public Entity followEntity;
        public List<string> cameraStack = new List<string>();
        public string defaultCamera;
        public string curCamera;
        public Dictionary<string, CinemachineVirtualCameraBase> dicCamera = new Dictionary<string, CinemachineVirtualCameraBase>();
    }
}