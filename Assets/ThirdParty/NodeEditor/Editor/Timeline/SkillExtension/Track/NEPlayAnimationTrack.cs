using System.Collections.Generic;
using Game;
using Pathfinding;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace NodeEditor
{
    [TrackClipType(typeof(NEPlayAnimationAsset))]
    public class NEPlayAnimationTrack : NEAbstractPlayableTrack
    {
    }
}