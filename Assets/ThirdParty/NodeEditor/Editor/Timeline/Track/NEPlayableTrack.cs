using System.Collections.Generic;
using Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NodeEditor
{
    [TrackClipType(typeof(NEPlayableAsset))]
    public class NEPlayableTrack : NEAbstractPlayableTrack
    {
    }
}