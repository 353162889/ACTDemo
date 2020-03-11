using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NodeEditor
{
    public class NEPlayEffectAsset : PlayableAsset, INEPlayableAsset, INEReleatedPlayableAsset
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
                neData.data = new BTPlayEffectActionData();
            }
            BTPlayEffectActionData data = (BTPlayEffectActionData)neData.data;
            if (this.parent != null)
            {
                data.localPos = this.parent.localPosition;
                data.localRot = this.parent.localRotation;
            }
            if (relatedAnimationClip != null)
            {
                data.duration = (float)relatedAnimationClip.duration;
            }

            return neData;
        }

        public TimelineClip curTimelineClip { get; set; }
        public void InitInspector()
        {
            PlayableAssetInspectorUtility.InitPlayEffectInspector(this);
        }

        public TimelineClip relatedAnimationClip { get; set; }

        public GameObject prefab { get; set; }
        public Transform parent { get; set; }
    }
}