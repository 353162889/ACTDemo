using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class ForbidSystem : ComponentSystem
    {
#if UNITY_EDITOR
        public static bool DebugForbid = false;
#endif

        protected override void OnCreate()
        {
            ObjectPool<Forbiddance>.Instance.Init(20);
        }

        public Forbiddance AddForbiddance(Entity entity, string desc = "")
        {
            var forbidComponent = World.GetComponent<ForbidComponent>(entity);
            return AddForbiddance(forbidComponent, desc);
        }
        public Forbiddance AddForbiddance(ForbidComponent forbidComponent, string desc = "")
        {
            if (forbidComponent == null) return null;
            Forbiddance forbid = ObjectPool<Forbiddance>.Instance.GetObject();
            forbid.Index = forbidComponent.forbidIndex++;
            forbid.desc = desc;
            forbidComponent.lstForbids.Add(forbid);
            return forbid;
        }

        public bool RemoveForbiddance(Entity entity, Forbiddance forbid)
        {
            var forbidComponent = World.GetComponent<ForbidComponent>(entity);
            return RemoveForbiddance(forbidComponent, forbid);
        }
        public bool RemoveForbiddance(ForbidComponent forbidComponent, Forbiddance forbid)
        {
            if (forbidComponent == null) return false;
            if (forbidComponent.lstForbids.Remove(forbid))
            {
                ObjectPool<Forbiddance>.Instance.SaveObject(forbid);
            }

            return false;
        }

        public bool IsForbid(Entity entity, ForbidType forbidType)
        {
            var forbidComponent = World.GetComponent<ForbidComponent>(entity);
            return IsForbid(forbidComponent, forbidType);
        }

        public bool IsForbid(ForbidComponent forbidComponent, ForbidType forbidType)
        {
            if (forbidComponent == null) return false;
            foreach (var forbiddance in forbidComponent.lstForbids)
            {
                if (forbiddance.IsForbid(forbidType))
                {
#if UNITY_EDITOR
                    if (DebugForbid)
                    {
                        CLog.LogColorArgs(CLogColor.Red, "Forbidance:", forbiddance.ToString(), "IsForbid", forbidType);
                    }
#endif
                    return true;
                }
            }
            return false;
        }

        public void Forbid(Entity entity, ForbidType forbidType)
        {
            var forbidComponent = World.GetComponent<ForbidComponent>(entity);
            Forbid(forbidComponent, forbidType);
        }

        public void Forbid(ForbidComponent forbidComponent, ForbidType forbidType)
        {
            if (forbidComponent == null) return;
            forbidComponent.forbiddance.Forbid(forbidType);
        }

        public void ResumeForbid(Entity entity, ForbidType resumeType)
        {
            var forbidComponent = World.GetComponent<ForbidComponent>(entity);
            ResumeForbid(forbidComponent, resumeType);
        }

        public void ResumeForbid(ForbidComponent forbidComponent, ForbidType resumeType)
        {
            if (forbidComponent == null) return;
            forbidComponent.forbiddance.ResumeForbid(resumeType);
        }

        protected override void OnUpdate()
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F))
            {
                DebugForbid = !DebugForbid;
                CLog.LogArgs("DebugForbid", DebugForbid);
            }
#endif
        }
    }
}