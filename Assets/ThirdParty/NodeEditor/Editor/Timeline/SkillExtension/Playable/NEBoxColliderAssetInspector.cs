using Game;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    [CustomEditor(typeof(NEBoxColliderAsset))]
    public class NEBoxColliderAssetInspector : NEPlayableAssetInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            NEBoxColliderAsset asset = target as NEBoxColliderAsset;
            if (asset.curTimelineClip == null) return;
            if (asset.neData == null || !(asset.neData.data is BTBoxColliderActionData)) return;
            asset.boxCollider = (BoxCollider)EditorGUILayout.ObjectField("boxCollider", asset.boxCollider, typeof(BoxCollider), false);
            if (GUILayout.Button("重置碰撞参数"))
            {
                PlayableAssetInspectorUtility.InitBoxColliderInspector(asset, true);
            }
        }
    }
}