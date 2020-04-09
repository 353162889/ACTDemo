using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Framework;
using Unity.Entities;
using UnityEngine;
using UnityEngineInternal;

namespace Game
{
    public class CameraSystem : ComponentSystem
    {
        public event Action<string> OnCameraChange; 
        private CameraComponent cameraComponent;
        private AvatarSystem avatarSystem;
        private InputSystem inputSystem;
        private Dictionary<string, Action> dicCameraUpdate;
        private Dictionary<string, Action<float,float>> dicCameraScreenAxisUpdate;
        private Dictionary<string, Action<float>> dicCameraScreenScroll;

        protected override void OnCreate()
        {
            cameraComponent = World.AddSingletonComponent<CameraComponent>();
            avatarSystem = World.GetOrCreateSystem<AvatarSystem>();
            inputSystem = World.GetOrCreateSystem<InputSystem>();
            dicCameraUpdate = new Dictionary<string, Action>();

            dicCameraScreenAxisUpdate = new Dictionary<string, Action<float, float>>();
            dicCameraScreenAxisUpdate.Add(CameraStrategy.NormalWalkCamera, OnUpdateNormalWalkCameraScreenAxis);

            dicCameraScreenScroll= new Dictionary<string, Action<float>>();
            dicCameraScreenScroll.Add(CameraStrategy.NormalWalkCamera, OnUpdateNormalWalkCameraScreenScroll);
        }

        public void ResetCameraStrategy(GameObject cameraRoot)
        {
            cameraComponent.dicCamera.Clear();
            var trans = cameraRoot.transform;
            for (int i = 0; i < trans.childCount; i++)
            {
                var child = trans.GetChild(i);
                var camera = child.GetComponent<CinemachineVirtualCameraBase>();
                if (camera != null)
                {
                    RegisterCinemachineCamera(camera.gameObject.name, camera);
                }
            }
        }

        public void RegisterCinemachineCamera(string strategy, CinemachineVirtualCameraBase cinemachineCamera)
        {
            if (!cameraComponent.dicCamera.ContainsKey(strategy))
            {
                cameraComponent.dicCamera.Add(strategy, cinemachineCamera);
            }
        }

        public void SetMainCamera(Camera camera)
        {
            cameraComponent.camera = camera;
        }

        public void SetFollow(Entity entity, string camera)
        {
            if (!cameraComponent.dicCamera.ContainsKey(camera))
            {
                CLog.LogError("找不到默认相机:"+camera);
                return;
            }
            cameraComponent.followEntity = entity;
            cameraComponent.defaultCamera = camera;
            UpdateCamera();
        }

        public void OnScreenAxisUpdate(float xAxis, float yAxis)
        {
            if (Cursor.visible) return;
            if (cameraComponent != null && dicCameraScreenAxisUpdate.ContainsKey(cameraComponent.curCamera))
            {
                dicCameraScreenAxisUpdate[cameraComponent.curCamera].Invoke(xAxis, yAxis);
            }
        }

        public void OnScreenScroll(float scroll)
        {
            if (Cursor.visible) return;
            if (cameraComponent != null && dicCameraScreenScroll.ContainsKey(cameraComponent.curCamera))
            {
                dicCameraScreenScroll[cameraComponent.curCamera].Invoke(scroll);
            }
        }

        public void PushCamera(string camera)
        {
            var stack = cameraComponent.cameraStack;
            if ((stack.Count > 0 && stack[stack.Count - 1] == camera) || !cameraComponent.dicCamera.ContainsKey(camera))
            {
                return;
            }
            stack.Add(camera);
            UpdateCamera();
        }

        public void PopCamera(string camera)
        {
            var stack = cameraComponent.cameraStack;
            for (int i = stack.Count - 1; i > -1 ; i--)
            {
                if (stack[i] == camera)
                {
                    stack.RemoveAt(i);
                    UpdateCamera();
                    break;
                }
            }
        }

        public string CurCamera()
        {
            return cameraComponent.curCamera;
        }

        public Camera GetMainCamera()
        {
            return cameraComponent.camera;
        }

        public Entity FollowEntity()
        {
            return cameraComponent.followEntity;
        }

        private void UpdateCamera()
        {
            var camera = GetHighestPriorityCamera();
            foreach (var pair in cameraComponent.dicCamera)
            {
                if (pair.Value.gameObject.activeSelf)
                {
                    pair.Value.LookAt = null;
                    pair.Value.Follow = null;
                    pair.Value.gameObject.SetActive(false);
                }
            }

            var cinemachineCamera = cameraComponent.dicCamera[camera];
            var targetTrans = avatarSystem.GetMountPoint(cameraComponent.followEntity, "CameraMount");
            if (targetTrans == null)
            { 
                var prefabComponent = World.GetComponent<PrefabComponent>(cameraComponent.followEntity);
                if (prefabComponent != null)
                {
                    targetTrans = prefabComponent.transform;
                }
            }
            cinemachineCamera.LookAt = targetTrans;
            cinemachineCamera.Follow = targetTrans;

            cinemachineCamera.gameObject.SetActive(true);
            cameraComponent.curCamera = camera;
            if (OnCameraChange != null)
            {
                OnCameraChange.Invoke(cameraComponent.curCamera);
            }
        }

        private string GetHighestPriorityCamera()
        {
            var stack = cameraComponent.cameraStack;
            var dicCamera = cameraComponent.dicCamera;
            string camera = cameraComponent.defaultCamera;
            var priority = dicCamera[camera].Priority * 1000;
            for (int i = 0; i < stack.Count; i++)
            {
                var stackCamera = stack[i];
                var stackPriority = dicCamera[stackCamera].Priority * 1000 + i + 1;
                if (stackPriority >= priority)
                {
                    camera = stackCamera;
                    priority = stackPriority;
                }
            }

            return camera;
        }

        public Vector3 TransformDirection(Vector3 localDirection)
        {
            if (cameraComponent == null || cameraComponent.camera == null) return localDirection;
            return cameraComponent.camera.transform.TransformDirection(localDirection);
        }

        protected override void OnUpdate()
        {
            if (cameraComponent != null && dicCameraUpdate.ContainsKey(cameraComponent.curCamera))
            {
                dicCameraUpdate[cameraComponent.curCamera].Invoke();
            }
            
        }
        //镜头永远瞄准角色背后
        private void OnUpdateNormalWalkCameraScreenAxis(float xAxis, float yAxis)
        {
            var cinemachineCamera = cameraComponent.dicCamera[cameraComponent.curCamera];
            var freeLookCamera = cinemachineCamera as CinemachineFreeLook;
            if (freeLookCamera != null && cameraComponent.followEntity != Entity.Null)
            {
                var angle = xAxis * freeLookCamera.m_XAxis.m_MaxSpeed * Time.deltaTime;
                var transformComponent = World.GetComponent<TransformComponent>(cameraComponent.followEntity);
                var forward = Quaternion.AngleAxis(angle, Vector3.up) * (transformComponent.rotation * Vector3.forward);
                inputSystem.ChangeFace(forward, true);

                freeLookCamera.m_YAxis.Value =
                    Mathf.Clamp01(freeLookCamera.m_YAxis.Value +
                                  (-yAxis) * freeLookCamera.m_YAxis.m_MaxSpeed * Time.deltaTime);
            }
        }

        private void OnUpdateNormalWalkCameraScreenScroll(float scroll)
        {
            var cinemachineCamera = cameraComponent.dicCamera[cameraComponent.curCamera];
            var freeLookCamera = cinemachineCamera as CinemachineFreeLook;
            if (freeLookCamera != null && cameraComponent.followEntity != Entity.Null)
            {
                var freeLookExtension = freeLookCamera.GetComponent<FreeLookCameraScrollExtension>();
                if (freeLookExtension != null)
                {
                    freeLookExtension.scrollState.m_InputAxisValue = scroll;
                }
            }
        }

        protected override void OnDestroy()
        {
            CinemachineBrain.SoloCamera = null;
            OnCameraChange = null;
        }
    }
}
