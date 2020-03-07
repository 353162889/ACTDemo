using System;
using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class MountPointInfo
    {
        public string name;
        public Transform trans;
    }
    public class MountPointCollector : MonoBehaviour
    {
        public List<MountPointInfo> infos;
        [NonSerialized]
        private Dictionary<string, Transform> m_cRuntimeData;

        private string[] names;

        void Awake()
        {
            m_cRuntimeData = new Dictionary<string, Transform>();
            if (infos != null)
            {
                for (int i = 0; i < infos.Count; i++)
                {
                    var info = infos[i];
                    if (m_cRuntimeData.ContainsKey(info.name))
                    {
                        CLog.LogError("hangpoint name="+info.name+"已存在");
                        continue;
                    }
                    m_cRuntimeData.Add(info.name, info.trans);
                }
            }
        }

        public string[] GetMountPointNames()
        {
            if (names == null)
            {
                int count = 0;
                if (infos != null) count = infos.Count;
                names = new string[count];
                for (int i = 0; i < count; i++)
                {
                    names[i] = infos[i].name;
                }
            }

            return names;
        }

        public Transform GetMountpoint(string name)
        {
            if (m_cRuntimeData == null)
            {
                Awake();
            }
            Transform trans = null;
            m_cRuntimeData.TryGetValue(name, out trans);
            return trans;
        }
#if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/MountPointCollector/收集挂点")]
        public static void FindMountPoints(UnityEditor.MenuCommand command)
        {
            var collector = command.context as MountPointCollector;
            collector.infos = new List<MountPointInfo>();
            FindMountPoint(collector.transform, collector.infos);
            //检测命名重复
            HashSet<string> set = new HashSet<string>();
            for (int i = collector.infos.Count - 1; i >= 0; i--)
            {
                var info = collector.infos[i];
                if (set.Contains(info.name))
                {
                    CLog.LogColorArgs(CLogColor.Red, "检测到挂点命名重复，名称为", info.name);
                    collector.infos.RemoveAt(i);
                }
                else
                {
                    set.Add(info.name);
                }
            }
            CLog.LogArgs("FindHangPoints");
        }

        private static void FindMountPoint(Transform trans, List<MountPointInfo> lst)
        {
            if (trans.gameObject.tag == "MountPoint")
            {
                var mountPointInfo = new MountPointInfo();
                mountPointInfo.name = trans.name;
                mountPointInfo.trans = trans;
                lst.Add(mountPointInfo);
            }

            for (int i = 0; i < trans.childCount; i++)
            {
                var child = trans.GetChild(i);
                FindMountPoint(child, lst);
            }
        }
#endif
    }
}