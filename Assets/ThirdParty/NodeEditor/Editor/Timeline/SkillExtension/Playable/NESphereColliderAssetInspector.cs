using Game;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    [CustomEditor(typeof(NESphereColliderAsset))]
    public class NESphereColliderAssetInspector : NEPlayableAssetInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            NESphereColliderAsset asset = target as NESphereColliderAsset;
            if (asset.curTimelineClip == null) return;
            if (asset.neData == null || !(asset.neData.data is BTSphereColliderActionData)) return;
            asset.sphereCollider = (SphereCollider)EditorGUILayout.ObjectField("sphereCollider", asset.sphereCollider, typeof(SphereCollider), false);
            if (GUILayout.Button("重置碰撞参数"))
            {
                PlayableAssetInspectorUtility.InitSphereColliderInspector(asset, true);
            }
        }
    }
}