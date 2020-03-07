using System;
using System.Collections.Generic;
using Game;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NodeEditor
{
    public class NEPlayAnimationAsset : PlayableAsset, INEPlayableAsset, INEReleatedPlayableAsset
    {
        public AnimationClip clip { get; set; }
        public TimelineClip relatedAnimationClip { get; set; }
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return Playable.Create(graph);
        }

        private NEData m_cNEData;
        public NEData neData {
            get
            {
                if (m_cNEData == null)
                {
                    m_cNEData = new NEData();
                    m_cNEData.lstChild = new List<NEData>();
                }

                if (m_cNEData.data == null)
                {
                    m_cNEData.data = new BTPlayAnimationActionData();
                }
                if (relatedAnimationClip != null)
                {
                    BTPlayAnimationActionData data = (BTPlayAnimationActionData) m_cNEData.data;
                    if (relatedAnimationClip.timeScale == 0)
                    {
                        data.duration = (float)relatedAnimationClip.duration;
                    }
                    else
                    {
                        data.duration = (float)(relatedAnimationClip.duration / relatedAnimationClip.timeScale);
                    }
                }

                return m_cNEData;
            }
            set { m_cNEData = value; }
        }
        public TimelineClip curTimelineClip { get; set; }
        public void InitInspector()
        {
            PlayableAssetInspectorUtility.InitPlayAnimationInspector(this);
        }
    }
}