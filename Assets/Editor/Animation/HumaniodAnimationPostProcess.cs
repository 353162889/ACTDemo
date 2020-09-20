using System;
using Framework;
using Game;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using Asset = Unity.Entities.Asset;

public class HumaniodAnimationPostProcess
{
    [MenuItem("Assets/PostprocessTwinbladesExpansion")]
    static void PostprocessTwinbladesExpansion()
    {
        var setting = AssetDatabase.LoadAssetAtPath<HumaniodAnimationPostProcessSetting>("Assets/Settings/TwinbladesExpansionPostProcessSetting.asset");
        if (setting == null) return;
        ProcessModel(setting);
    }

    [MenuItem("Assets/PostprocessTwinblades")]
    static void PostprocessTwinblades()
    {
        var setting = AssetDatabase.LoadAssetAtPath<HumaniodAnimationPostProcessSetting>("Assets/Settings/TwinbladesPostProcessSetting.asset");
        if (setting == null) return;
        ProcessModel(setting);
    }

    static void ProcessModel(HumaniodAnimationPostProcessSetting setting)
    {
        var assetList = Provider.GetAssetListFromSelection();
        for (int i = 0; i < assetList.Count; i++)
        {
            if (assetList[i].isFolder)
            {
                var folderDir = assetList[i].assetPath;
                if (folderDir.EndsWith("/")) folderDir = folderDir.Substring(0, folderDir.Length - 1);
                var guids = AssetDatabase.FindAssets("t:GameObject", new[] { folderDir });
                foreach (var guid in guids)
                {
                    var asset = Provider.GetAssetByGUID(guid);
                    PostProcessAnimation(setting, asset.assetPath, asset.name);
                }
            }
            else
            {
                PostProcessAnimation(setting, assetList[i].assetPath, assetList[i].name);
            }
        }
    }

    static void PostProcessAnimation(HumaniodAnimationPostProcessSetting setting, string path, string name)
    {
        if (setting.arrDir == null) return;
        bool contain = false;
        foreach (var obj in setting.arrDir)
        {
            var objPath = AssetDatabase.GetAssetPath(obj);
            var asset = Provider.GetAssetByPath(objPath);
            if (asset != null)
            {
                if (asset.isFolder)
                {
                    if (path.IndexOf(asset.assetPath) > -1)
                    {
                        contain = true;
                        break;
                    }
                }
                else
                {
                    if (path == asset.assetPath)
                    {
                        contain = true;
                        break;
                    }
                }
            }
        }

        if (!contain) return;
        
        var importer = AssetImporter.GetAtPath(path);
        if (!(importer is ModelImporter)) return;
        var modelImporter = (ModelImporter) importer;
        if (modelImporter.defaultClipAnimations.Length == 0) return;
        modelImporter.animationType = ModelImporterAnimationType.Human;
        modelImporter.sourceAvatar = setting.referenceAvatar;
        var anims = modelImporter.defaultClipAnimations;
        bool isLoop = false;
        if (setting.loopAnims != null)
        {
            foreach (var loopAnim in setting.loopAnims)
            {
                if (name.IndexOf(loopAnim) > -1)
                {
                    isLoop = true;
                    break;
                }
            }
        }

        var newName = name;
        if (setting.removePrev != null)
        {
            foreach (var prev in setting.removePrev)
            {
                if (newName.StartsWith(prev))
                {
                    newName = newName.Substring(prev.Length);
                    break;
                }
            }
        }
        for (int i = 0; i < anims.Length; i++)
        {
            anims[i].name = newName;
            anims[i].loopTime = isLoop;
            anims[i].loopPose = isLoop;
            anims[i].maskType = ClipAnimationMaskType.CopyFromOther;
            anims[i].maskSource = setting.animMask;
            anims[i].lockRootRotation = true;
            anims[i].keepOriginalOrientation = true;
            anims[i].lockRootPositionXZ = true;
            anims[i].keepOriginalPositionXZ = true;
            anims[i].lockRootHeightY = true;
            anims[i].keepOriginalPositionY = true;
        }

        modelImporter.clipAnimations = anims;
        modelImporter.SaveAndReimport();
        Debug.Log("process:"+path);
    }
}
