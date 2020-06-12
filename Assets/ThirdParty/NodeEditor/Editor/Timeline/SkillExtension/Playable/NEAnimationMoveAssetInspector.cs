using Game;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    [CustomEditor(typeof(NEAnimationMoveAsset))]
    public class NEAnimationMoveAssetInspector : NEPlayableAssetInspector
    {
        private AnimationClip extClip;
        private string rootNode = "root";
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

            EditorGUILayout.Space();
            extClip = (AnimationClip)EditorGUILayout.ObjectField("外部Clip", extClip, typeof(AnimationClip), false);
            rootNode = EditorGUILayout.TextField("导出根节点", rootNode);
            if (GUILayout.Button("从外部Clip导入"))
            {
                if (extClip != null)
                {
                    if (asset.relatedAnimationClip == null)
                    {
                        EditorUtility.DisplayDialog("提示", "需要关联timelineClip", "确认");
                        return;
                    }
                    var points = AnimationMoveUtility.ConvertClipToPoints(extClip, -1, rootNode);
                    var timelineClip = asset.relatedAnimationClip;
                    timelineClip.animationClip.ClearCurves();
                    AnimationMoveUtility.ConvertPointsToTimelineClip(points, ref timelineClip);
                    asset.curTimelineClip.duration = timelineClip.duration;
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "需要外部Clip", "确认");
                }
            }
        }
    }
}