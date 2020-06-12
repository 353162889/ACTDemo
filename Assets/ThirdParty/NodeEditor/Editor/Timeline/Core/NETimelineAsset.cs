using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NodeEditor
{
    public class NETimelineAsset : TimelineAsset
    {
        public GroupTrack viewGroupTrack { get; set; }
        public AnimationTrack modelAnimTrack { get; set; }
        public AnimationTrack modelMoveTrack { get; set; }
        public ControlTrack effectTrack { get; set; }
        public Transform effectGlobalParent { get; set; }
        public Transform colliderParent { get; set; }
        public PlayableDirector director { get; set; }

        public Animator modelAnimAnimator
        {
            get
            {
                if (modelAnimTrack == null) return null;
                if (director == null) return null;
                foreach (var playableBinding in this.outputs)
                {
                    if (playableBinding.streamName == modelAnimTrack.name)
                    {
                        return (Animator)director.GetGenericBinding(modelAnimTrack);
                    }
                }

                return null;
            }
        }
    }
}