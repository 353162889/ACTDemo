using UnityEngine;
using UnityEngine.Timeline;

namespace NodeEditor
{
    public interface INEReleatedPlayableAsset
    {
        TimelineClip relatedAnimationClip { get; set; }
    }
}