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

        public NEData neData { get; set; }

        public NEData ConvertNEData()
        {
            if (neData == null)
            {
                neData = new NEData();
                neData.lstChild = new List<NEData>();
            }

            if (neData.data == null)
            {
                neData.data = new BTPlayAnimationActionData();
            }
            if (relatedAnimationClip != null && curTimelineClip != null)
            {
                if (relatedAnimationClip.timeScale == 0)
                {
                    curTimelineClip.duration = (float)relatedAnimationClip.duration;
                }
                else
                {
                    curTimelineClip.duration = (float)(relatedAnimationClip.duration / relatedAnimationClip.timeScale);
                }
            }

            return neData;
        }

        public TimelineClip curTimelineClip { get; set; }
        public void InitInspector()
        {
            PlayableAssetInspectorUtility.InitPlayAnimationInspector(this);
        }
    }
}