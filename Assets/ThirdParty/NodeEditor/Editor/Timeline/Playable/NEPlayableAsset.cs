using System;
using Game;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NodeEditor
{
    [Serializable]
    public class NEPlayableAsset : PlayableAsset, INEPlayableAsset
    {
        public NEData neData { get; set; }

        public TimelineClip curTimelineClip { get; set; }
        public void InitInspector()
        {
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return Playable.Create(graph);
        }
    }
}