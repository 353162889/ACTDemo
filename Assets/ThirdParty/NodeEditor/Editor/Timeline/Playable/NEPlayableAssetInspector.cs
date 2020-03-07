using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    [CustomEditor(typeof(NEPlayableAsset))]
    public class NEPlayableAssetInspector : Editor
    {
        private NEDataProperty[] properties;

        void OnDisable()
        {
            properties = null;
        }

        public override void OnInspectorGUI()
        {
            INEPlayableAsset asset = target as INEPlayableAsset;
            
            object data = asset.neData != null ? asset.neData.data : null;

            if (properties == null && data != null)
            {
                properties = NEDataProperties.GetProperties(data); 
            }
            PlayableAssetInspectorUtility.DrawBaseInspectorGUI(asset, properties, null);
        }
    }
}