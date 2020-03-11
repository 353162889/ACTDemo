using System;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace NodeEditor
{
    [Serializable]
    public class NEAbstractPlayableTrack : TrackAsset,INEPlayableTrack
    {
        public NETimelineAsset neTimelineAsset
        {
            get
            {
                return (NETimelineAsset) this.timelineAsset;
            }
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);
            var type = TimeLineUtility.TryGetDefaultData(this.GetType());
            if (type != null)
            {
                var asset = clip.asset as INEPlayableAsset;
                if (asset != null)
                {
                    asset.neData = new NEData();
                    asset.neData.lstChild = new List<NEData>();
                    asset.neData.data = Activator.CreateInstance(type);
                }
            }

            clip.duration = 0.5f;
            TimeLineUtility.InitNEPlayableClip(clip);
        }

        public virtual void Update()
        {
            var selectedClip = TimelineEditor.selectedClip;
            if (selectedClip == null) return;
            foreach (var timelineClip in GetClips())
            {
                if (timelineClip.asset is INEReleatedPlayableAsset)
                {
                    var asset = (INEReleatedPlayableAsset)timelineClip.asset;
                    var relatedClip = asset.relatedAnimationClip;
                    if (relatedClip != null)
                    {
                        if (selectedClip == timelineClip)
                        {
                            relatedClip.start = timelineClip.start;
                            relatedClip.duration = timelineClip.duration;
                        }
                        else if (selectedClip == relatedClip)
                        {
                            timelineClip.start = relatedClip.start;
                            timelineClip.duration = relatedClip.duration;
                        }
                    }
                }
            }
        }
    }
}