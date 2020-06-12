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
                neData.data = new BTAnimationMoveActionData();
            }

            if (relatedAnimationClip != null && relatedAnimationClip.animationClip != null)
            {
                var clip = relatedAnimationClip.animationClip;
                BTAnimationMoveActionData data = (BTAnimationMoveActionData)neData.data;
                data.movePoints = AnimationMoveUtility.ConvertClipToPoints(clip, (float)relatedAnimationClip.duration);
            }
            return neData;
        }

        public TimelineClip curTimelineClip { get; set; }
        public void InitInspector()
        {
            PlayableAssetInspectorUtility.InitAnimationMoveInspector(this, false);
        }

        public TimelineClip relatedAnimationClip { get; set; }
    }
}