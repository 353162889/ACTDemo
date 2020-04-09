using Framework;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class CameraLockView : BaseSubView
    {
        private RectTransform lockArea;
        private UIFollower follower;
        private CameraLockSystem cameraLockSystem;
        private CameraSystem cameraSystem;
        private AvatarSystem avatarSystem;
        public CameraLockView(GameObject go) : base(go)
        {
        }

        protected override void BuidUI()
        {
            lockArea = this.MainGO.FindChildComponentRecursive<RectTransform>("LockArea");
            var transFollower = this.MainGO.FindChildRecursive("LockFollow");
            follower = transFollower.AddComponentOnce<UIFollower>();
        }

        public override void OnEnterFinished()
        {
            base.OnEnterFinished();
            cameraLockSystem = World.Active.GetExistingSystem<CameraLockSystem>();
            cameraSystem = World.Active.GetExistingSystem<CameraSystem>();
            avatarSystem = World.Active.GetExistingSystem<AvatarSystem>();
        }

        public override void OnUpdate()
        {
            if (cameraLockSystem.IsEnable())
            {
                cameraLockSystem.SetViewport(ViewSys.Instance.canvas.worldCamera.WorldToViewportPoint(lockArea.transform.position));   
                if (cameraLockSystem.IsLocking())
                {
                    lockArea.gameObject.SetActive(false);
                    follower.gameObject.SetActive(true);
                    var hangpoint = avatarSystem.GetMountPoint(cameraLockSystem.LockEntity(), "Locked");
                    follower.SetTarget(cameraSystem.GetMainCamera(), hangpoint);

                }
                else
                {
                    lockArea.gameObject.SetActive(true);
                    follower.Dispose();
                    follower.gameObject.SetActive(false);
                }
            }
            else
            {
                lockArea.gameObject.SetActive(false);
                follower.Dispose();
                follower.gameObject.SetActive(false);
            }
        }
    }
}