using System;
using System.Collections.Generic;
using Game;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NodeEditor
{
    public class NEAnimationMoveAsset : PlayableAsset, INEPlayableAsset, INEReleatedPlayableAsset
    {
        private struct MoveInfo
        {
            public float time;
            public float[] posInfo;

            public MoveInfo(float time, float[] pos)
            {
                this.time = time;
                this.posInfo = pos;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return Playable.Create(graph);
        }

        private NEData m_cNEData;
        private static string[] PropertyNames = new[] { "m_LocalPosition.x", "m_LocalPosition.y", "m_LocalPosition.z" };
        public NEData neData
        {
            get
            {
                if (m_cNEData == null)
                {
                    m_cNEData = new NEData();
                    m_cNEData.lstChild = new List<NEData>();
                }

                if (m_cNEData.data == null)
                {
                    m_cNEData.data = new BTAnimationMoveActionData();
                }

                if (relatedAnimationClip != null && relatedAnimationClip.animationClip != null)
                {
                    var clip = relatedAnimationClip.animationClip;
                    var binds = AnimationUtility.GetCurveBindings(clip);
                    Dictionary<float, float[]> map = new Dictionary<float, float[]>();
                    foreach (var bind in binds)
                    {
                        int index = Array.IndexOf(PropertyNames, bind.propertyName);
                        if (index < 0)continue;
                        var animCurve = AnimationUtility.GetEditorCurve(clip, bind);
                        UpdateInfo(map, animCurve, index);
                    }

                    List<MoveInfo> infos = new List<MoveInfo>();
                    foreach (var info in map)
                    {
                        infos.Add(new MoveInfo(info.Key, info.Value));
                    }
                    infos.Sort((MoveInfo a, MoveInfo b) =>
                    {
                        if (a.time < b.time)
                        {
                            return -1;
                        }
                        else if (a.time > b.time)
                        {
                            return 1;
                        }
                        return 0;
                    });
                    if (infos.Count > 0)
                    {
                        //第一帧添加时间为0
                        if (infos[0].time > 0)
                        {
                            infos.Insert(0, new MoveInfo(0,new float[]{0,0,0}));
                        }
                        //添加最后一帧
                        float lastTime = (float) relatedAnimationClip.duration;
                        if (!Mathf.Approximately(infos[infos.Count - 1].time , lastTime) && infos[infos.Count - 1].time < lastTime)
                        {
                            var temp = infos[infos.Count - 1].posInfo;
                            infos.Add(new MoveInfo(lastTime, new float[] { temp[0], temp[1], temp[2] }));
                        }
                    }
                    
                    BTAnimationMoveActionData data = (BTAnimationMoveActionData)m_cNEData.data;
                    data.movePoints = new float[infos.Count * 4];
                    for (int i = 0; i < infos.Count; i++)
                    {
                        int index = i * 4;
                        data.movePoints[index] = infos[i].time;
                        data.movePoints[index + 1] = infos[i].posInfo[0];
                        data.movePoints[index + 2] = infos[i].posInfo[1];
                        data.movePoints[index + 3] = infos[i].posInfo[2];
                    }
                }
                return m_cNEData;

            }
            set { m_cNEData = value; }
        }

        private void UpdateInfo(Dictionary<float, float[]> map, AnimationCurve animCurve, int index)
        {
            for (int i = 0; i < animCurve.length; i++)
            {
                var time = animCurve.keys[i].time;
                var value = animCurve.keys[i].value;
                if (!map.ContainsKey(time)) map.Add(time, new float[3]);
                map[time][index] = value;
            }
        }

        public TimelineClip curTimelineClip { get; set; }
        public void InitInspector()
        {
            PlayableAssetInspectorUtility.InitAnimationMoveInspector(this, false);
        }

        public TimelineClip relatedAnimationClip { get; set; }
    }
}