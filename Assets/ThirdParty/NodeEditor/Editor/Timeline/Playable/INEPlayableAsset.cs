using UnityEngine.Timeline;

namespace NodeEditor
{
    public interface INEPlayableAsset
    {
        NEData neData { get; set; }
        TimelineClip curTimelineClip { get; set; }

        void InitInspector();
    }
}