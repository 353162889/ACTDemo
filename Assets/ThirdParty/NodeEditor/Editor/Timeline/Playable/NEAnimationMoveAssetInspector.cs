using Game;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    [CustomEditor(typeof(NEAnimationMoveAsset))]
    public class NEAnimationMoveAssetInspector : NEPlayableAssetInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            NEAnimationMoveAsset asset = target as NEAnimationMoveAsset;
            if (asset.curTimelineClip == null) return;
            if (asset.neData == null || !(asset.neData.data is BTAnimationMoveActionData)) return;
            if (GUILayout.Button("关联动画Clip"))
            {
                PlayableAssetInspectorUtility.InitAnimationMoveInspector(asset, true);
            }
        }
    }
}