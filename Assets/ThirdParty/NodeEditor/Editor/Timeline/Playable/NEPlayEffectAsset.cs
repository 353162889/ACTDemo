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

        private NEData m_cNEData;

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
                    m_cNEData.data = new BTPlayEffectActionData();
                }
                BTPlayEffectActionData data = (BTPlayEffectActionData)m_cNEData.data;
                if (this.parent != null)
                {
                    data.localPos = this.parent.localPosition;
                    data.localRot = this.parent.localRotation;
                }
                if (relatedAnimationClip != null)
                {
                    data.duration = (float)relatedAnimationClip.duration;
                }

                return m_cNEData;
            }
            set { m_cNEData = value; }
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