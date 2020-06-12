using System;
using Game;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    [CustomEditor(typeof(NEPlayEffectAsset))]
    public class NEPlayEffectAssetInspector : NEPlayableAssetInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            NEPlayEffectAsset asset = target as NEPlayEffectAsset;
            if (asset.curTimelineClip == null) return;
            if (asset.neData == null || !(asset.neData.data is BTPlayEffectActionData)) return;
            var data = (BTPlayEffectActionData) asset.neData.data;
            NETimelineAsset neTimelineAsset = (NETimelineAsset)asset.curTimelineClip.parentTrack.timelineAsset;
            if (neTimelineAsset.director != null)
            {
                var collector = neTimelineAsset.director.gameObject.GetComponentInChildren<MountPointCollector>();
                if (collector != null)
                {
                    var names = collector.GetMountPointNames();
                    int index = Array.IndexOf(names, data.mountPoint);
                    int newIndex = EditorGUILayout.Popup("选择挂点", index, names);
                    if (newIndex >= 0)
                    {
                        data.mountPoint = names[newIndex];
                    }
                    else
                    {
                        data.mountPoint = "";
                    }
                }
            }
            asset.prefab = (GameObject)EditorGUILayout.ObjectField("prefab", asset.prefab, typeof(GameObject), false);
            asset.parent = (Transform)EditorGUILayout.ObjectField("parent", asset.parent, typeof(Transform), false);
            
            if (GUILayout.Button("关联特效对象"))
            {
                PlayableAssetInspectorUtility.InitPlayEffectInspector(asset, true);
            }
        }
    }
}