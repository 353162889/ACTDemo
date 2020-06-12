using UnityEngine.Timeline;

namespace NodeEditor
{
    public interface INEPlayableAsset
    {
        NEData neData { get; set; }
        NEData ConvertNEData();
        TimelineClip curTimelineClip { get; set; }

        void InitInspector();
    }
}