using System.Collections.Generic;
using Framework;
using Game;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NodeEditor
{
    public class NEBoxColliderAsset : PlayableAsset, INEPlayableAsset
    {
        public ScriptPlayable<BoxColliderBehaviour> playableBehaviour { get; private set; }
        public BoxCollider boxCollider { get; set; }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            InitInspector();
            playableBehaviour = BoxColliderBehaviour.Create(graph, boxCollider == null ?null :boxCollider.gameObject);
            return playableBehaviour;
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
                neData.data = new BTBoxColliderActionData();
            }

            if (boxCollider != null)
            {
                BTBoxColliderActionData data = (BTBoxColliderActionData)neData.data;
                data.localPos = boxCollider.transform.localPosition;
                data.localRot = boxCollider.transform.localRotation;
                data.size = boxCollider.size;
                data.duration = (float)curTimelineClip.duration;
            }
            return neData;
        }

        public TimelineClip curTimelineClip { get; set; }
        public void InitInspector()
        {
            PlayableAssetInspectorUtility.InitBoxColliderInspector(this);
        }
    }
}