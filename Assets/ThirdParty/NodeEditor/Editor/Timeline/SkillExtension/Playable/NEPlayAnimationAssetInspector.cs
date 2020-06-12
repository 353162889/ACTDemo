using Game;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NodeEditor
{
    [CustomEditor(typeof(NEPlayAnimationAsset))]
    public class NEPlayAnimationAssetInspector : NEPlayableAssetInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            NEPlayAnimationAsset asset = target as NEPlayAnimationAsset;
            asset.clip = (AnimationClip) EditorGUILayout.ObjectField("clip", asset.clip, typeof(AnimationClip), false);
            if (asset.curTimelineClip == null) return;
            if (asset.neData == null || !(asset.neData.data is BTPlayAnimationActionData)) return;
            if (GUILayout.Button("关联动画Clip"))
            {
                PlayableAssetInspectorUtility.InitPlayAnimationInspector(asset, true);
            }
        }
            
    }
}