using System.Collections.Generic;
using Framework;
using Game;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NodeEditor
{
    public class NESphereColliderAsset : PlayableAsset, INEPlayableAsset
    {
        public ScriptPlayable<ColliderBehaviour> playableBehaviour { get; private set; }
        public SphereCollider sphereCollider { get; set; }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            InitInspector();
            playableBehaviour = ColliderBehaviour.Create(graph, sphereCollider == null ? null : sphereCollider.gameObject);
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
                neData.data = new BTSphereColliderActionData();
            }

            if (sphereCollider != null)
            {
                BTSphereColliderActionData data = (BTSphereColliderActionData)neData.data;
                data.localPos = sphereCollider.transform.localPosition;
                data.localRot = sphereCollider.transform.localRotation;
                var scale = sphereCollider.transform.localScale;
                float radius = sphereCollider.radius;
                data.radius = Mathf.Max(scale.x, scale.y, scale.z) * radius;
                data.duration = (float)curTimelineClip.duration;
            }
            return neData;
        }

        public TimelineClip curTimelineClip { get; set; }
        public void InitInspector()
        {
            PlayableAssetInspectorUtility.InitSphereColliderInspector(this);
        }
    }
}