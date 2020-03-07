﻿using System;
using System.Collections.Generic;
using System.IO;
using Framework;
using Game;
using Unity.Transforms;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Timeline;

namespace NodeEditor
{
    public class PlayableAssetInspectorUtility
    {
        public static void DrawBaseInspectorGUI(INEPlayableAsset asset, NEDataProperty[] properties,
            Action<INEPlayableAsset> onUpdateAssetData = null)
        {
            object data = asset.neData != null ? asset.neData.data : null;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUILayout.LabelField(data != null ? data.GetType().Name : "null");
            if (GUILayout.Button("更换"))
            {
                var dic = NETreeWindow.GetCurrentCanvasEditorTypes();
                if (dic == null)
                {
                    EditorUtility.DisplayDialog("提示", "当前未打开NETreeWindow窗口", "确认");
                }
                else
                {
                    GenericMenu menu = new GenericMenu();
                    int count = dic.Count;
                    foreach (var item in dic)
                    {
                        string pre = "";
                        if (!string.IsNullOrEmpty(item.Key))
                        {
                            pre += item.Key + "/";
                        }

                        for (int i = 0; i < item.Value.Count; i++)
                        {
                            Type nodeDataType = item.Value[i];
                            var dataName = nodeDataType.Name;
                            if (dataName.EndsWith("Data")) dataName = dataName.Substring(0, dataName.Length - 4);
                            string name = pre + dataName;
                            menu.AddItem(new GUIContent(name), false, () =>
                            {
                                asset.neData = new NEData();
                                asset.neData.lstChild = new List<NEData>();
                                asset.neData.data = Activator.CreateInstance(nodeDataType);
                                properties = NEDataProperties.GetProperties(asset.neData.data);
                                if (asset.curTimelineClip != null)
                                {
                                    TimeLineUtility.UpdateNEPlayableClipName(asset.curTimelineClip);
                                }

                                if (onUpdateAssetData != null)
                                {
                                    onUpdateAssetData.Invoke(asset);
                                }

                                EditorUtility.SetDirty((UnityEngine.Object) asset);
                            });
                        }

                        if (count > 1)
                        {
                            menu.AddSeparator("");
                        }

                        count--;
                    }

                    menu.ShowAsContext();
                }
            }

            EditorGUILayout.EndHorizontal();
            if (data != null)
            {
                NEDataProperties.Draw(properties);
            }
        }

        private static void Display(string des, bool showDialog = false)
        {
            if (showDialog)
            {
                EditorUtility.DisplayDialog("提示", des, "确认");
            }
            else
            {
                Debug.Log($"NETimelineEditor {des}");
            }
        }

        private static AnimatorState GetState(AnimatorController animatorController, string animName)
        {
            foreach (var animatorControllerLayer in animatorController.layers)
            {
                var states = animatorControllerLayer.stateMachine.states;
                foreach (var childAnimatorState in states)
                {
                    if (childAnimatorState.state.name == animName)
                    {
                        return childAnimatorState.state;
                    }
                }
            }

            return null;
        }

        private static AnimatorState GetStateByClip(AnimatorController animatorController, AnimationClip clip)
        {
            foreach (var animatorControllerLayer in animatorController.layers)
            {
                var states = animatorControllerLayer.stateMachine.states;
                foreach (var childAnimatorState in states)
                {
                    if (childAnimatorState.state.motion == clip)
                    {
                        return childAnimatorState.state;
                    }
                }
            }

            return null;
        }

        public static void InitPlayAnimationInspector(NEPlayAnimationAsset asset, bool showDialog = false)
        {
            if (asset == null) return;
            NETimelineAsset timelineAsset = (NETimelineAsset) (asset.curTimelineClip.parentTrack.timelineAsset);
            var data = (BTPlayAnimationActionData) asset.neData.data;
            if (timelineAsset.modelAnimTrack == null || timelineAsset.modelAnimAnimator == null)
            {
                Display("需要动画轨道与绑定动画状态机", showDialog);
                return;
            }

            var animator = timelineAsset.modelAnimAnimator;
            var animatorController = (AnimatorController) animator.runtimeAnimatorController;
            if (animatorController == null)
            {
                Display("animator中找不到AnimatorController", showDialog);
                return;
            }

            if (asset.relatedAnimationClip != null)
            {
                bool contain = false;
                foreach (var clip in timelineAsset.modelAnimTrack.GetClips())
                {
                    if (clip == asset.relatedAnimationClip)
                    {
                        contain = true;
                        break;
                    }
                }

                if (!contain)
                {
                    asset.relatedAnimationClip = null;
                }
            }

            if (asset.relatedAnimationClip == null)
            {
                if (asset.clip == null && string.IsNullOrEmpty(data.animName))
                {
                    Display("需要输入animName或clip", showDialog);
                    return;
                }

                if (asset.clip == null)
                {
                    var state = GetState(animatorController, data.animName);
                    if (state != null && state.motion != null)
                    {
                        asset.clip = (AnimationClip) state.motion;
                        var timelineClip = timelineAsset.modelAnimTrack.CreateClip(asset.clip);
                        timelineClip.start = asset.curTimelineClip.start;
                        float speed = state.speed;
                        if (state.speedParameterActive) speed *= animator.GetFloat(state.speedParameter);
                        float duration = speed != 0 ? asset.clip.length / speed : asset.clip.length;

                        data.duration = duration;

                        timelineClip.duration = asset.clip.length;
                        timelineClip.timeScale = speed;
                        timelineClip.displayName = data.animName;

                        asset.curTimelineClip.start = timelineClip.start;
                        asset.curTimelineClip.duration = timelineClip.duration;

                        asset.relatedAnimationClip = timelineClip;
                    }
                    else
                    {

                        Display("在AnimatorController找不到名字为:" + data.animName + "的AnimationClip", showDialog);
                        return;
                    }
                }
                else
                {
                    var state = GetStateByClip(animatorController, asset.clip);
                    if (state != null)
                    {
                        var timelineClip = timelineAsset.modelAnimTrack.CreateClip(asset.clip);
                        timelineClip.start = asset.curTimelineClip.start;
                        float speed = state.speed;
                        if (state.speedParameterActive) speed *= animator.GetFloat(state.speedParameter);
                        float duration = speed != 0 ? asset.clip.length / speed : asset.clip.length;

                        data.animName = state.name;
                        data.duration = duration;

                        timelineClip.duration = asset.clip.length;
                        timelineClip.timeScale = speed;
                        timelineClip.displayName = data.animName;

                        asset.curTimelineClip.start = timelineClip.start;
                        asset.curTimelineClip.duration = timelineClip.duration;

                        asset.relatedAnimationClip = timelineClip;
                    }
                }

            }
            else
            {
                var timelineClip = asset.curTimelineClip;
                asset.curTimelineClip.start = timelineClip.start;
                asset.curTimelineClip.duration = timelineClip.duration;
                var state = GetStateByClip(animatorController, asset.clip);
                if (state != null && state.name != data.animName)
                {
                    data.animName = state.name;
                }
            }

        }

        public static void InitAnimationMoveInspector(NEAnimationMoveAsset asset, bool showDialog = false)
        {
            if (asset == null) return;
            NETimelineAsset timelineAsset = (NETimelineAsset) (asset.curTimelineClip.parentTrack.timelineAsset);
            if (timelineAsset.modelMoveTrack == null)
            {
                Display("需要动画移动轨道", showDialog);
                return;
            }

            if (asset.relatedAnimationClip != null)
            {
                bool contain = false;
                foreach (var clip in timelineAsset.modelMoveTrack.GetClips())
                {
                    if (clip == asset.relatedAnimationClip)
                    {
                        contain = true;
                        break;
                    }
                }

                if (!contain)
                {
                    asset.relatedAnimationClip = null;
                }
            }

            var data = (BTAnimationMoveActionData) asset.neData.data;
            if (asset.relatedAnimationClip == null)
            {
                var timelineClip = timelineAsset.modelMoveTrack.CreateRecordableClip("AnimationMove");
                timelineClip.start = asset.curTimelineClip.start;
                float duration = 1;
                if (data.movePoints != null && data.movePoints.Length > 0)
                {
                    int len = data.movePoints.Length / 4;
                    if (len > 0)
                    {
                        int index = (len - 1) * 4;
                        duration = data.movePoints[index];
                    }

                    AnimationCurve curveX = new AnimationCurve();
                    AnimationCurve curveY = new AnimationCurve();
                    AnimationCurve curveZ = new AnimationCurve();
                    for (int i = 0; i < len; i++)
                    {
                        int startIndex = i * 4;
                        float time = data.movePoints[startIndex];
                        float x = data.movePoints[startIndex + 1];
                        float y = data.movePoints[startIndex + 2];
                        float z = data.movePoints[startIndex + 3];
                        curveX.AddKey(time, x);
                        curveY.AddKey(time, y);
                        curveZ.AddKey(time, z);
                    }

                    var moveClip = timelineClip.animationClip;
                    moveClip.SetCurve("", typeof(Transform), "m_LocalPosition.x", curveX);
                    moveClip.SetCurve("", typeof(Transform), "m_LocalPosition.y", curveY);
                    moveClip.SetCurve("", typeof(Transform), "m_LocalPosition.z", curveZ);
                }

                timelineClip.duration = duration;

                asset.curTimelineClip.start = timelineClip.start;
                asset.curTimelineClip.duration = timelineClip.duration;

                asset.relatedAnimationClip = timelineClip;
            }
            else
            {
                var timelineClip = asset.curTimelineClip;
                asset.curTimelineClip.start = timelineClip.start;
                asset.curTimelineClip.duration = timelineClip.duration;
            }
        }

        public static void InitPlayEffectInspector(NEPlayEffectAsset asset, bool showDialog = false)
        {
            if (asset == null) return;
            NETimelineAsset timelineAsset = (NETimelineAsset)(asset.curTimelineClip.parentTrack.timelineAsset);
            if (timelineAsset.effectTrack == null || timelineAsset.director == null)
            {
                Display("需要动画特效轨道与director", showDialog);
                return;
            }

            if (asset.relatedAnimationClip != null)
            {
                bool contain = false;
                foreach (var clip in timelineAsset.effectTrack.GetClips())
                {
                    if (clip == asset.relatedAnimationClip)
                    {
                        contain = true;
                        break;
                    }
                }

                if (!contain)
                {
                    asset.relatedAnimationClip = null;
                }
            }

            var data = (BTPlayEffectActionData)asset.neData.data;
            if (asset.relatedAnimationClip == null)
            {
                if (string.IsNullOrEmpty(data.effectName) && asset.prefab == null)
                {
                    Display("需要输入effectName或prefab", showDialog);
                    return;
                }

                if (asset.prefab == null)
                {
                    string path = PathUtility.GetBattleEffectPath(data.effectName);
                    path = "Assets/ResourceEx/" + path;
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab == null)
                    {
                        Display($"找不到名字为:{data.effectName}的特效预制", showDialog);
                        return;
                    }

                    asset.prefab = prefab;
                }
                else
                {
                    var path = AssetDatabase.GetAssetPath(asset.prefab);
                    if (string.IsNullOrEmpty(path))
                    {
                        Display("当前prefab不是一个预制体", showDialog);
                        return;
                    }

                    var name = Path.GetFileNameWithoutExtension(path);
                    data.effectName = name;
                }

                var timelineClip = timelineAsset.effectTrack.CreateDefaultClip();
                timelineClip.start = asset.curTimelineClip.start;
                timelineClip.duration = data.duration > 0 ? data.duration : 1;

                asset.curTimelineClip.start = timelineClip.start;
                asset.curTimelineClip.duration = timelineClip.duration;

                asset.relatedAnimationClip = timelineClip;

                var controllerAsset = timelineClip.asset as ControlPlayableAsset;
                controllerAsset.prefabGameObject = asset.prefab;
                
            }
            else
            {
                var timelineClip = asset.curTimelineClip;
                asset.curTimelineClip.start = timelineClip.start;
                asset.curTimelineClip.duration = timelineClip.duration;
            }
            //关联挂点
            if (timelineAsset.director != null && asset.relatedAnimationClip != null)
            {
                var collector = timelineAsset.director.gameObject.GetComponentInChildren<MountPointCollector>();
                if (collector != null)
                {
                    var mountPoint = collector.GetMountpoint(data.mountPoint);
                    var controllerAsset = asset.relatedAnimationClip.asset as ControlPlayableAsset;
                    bool isFirstCreate = false;
                    if (asset.parent == null)
                    {
                        var go = new GameObject();
                        asset.parent = go.transform;
                        isFirstCreate = true;
                    }
                    if (mountPoint == null)
                    {
                        mountPoint = timelineAsset.effectGlobalParent;
                    }
                    mountPoint.gameObject.AddChildToParent(asset.parent.gameObject, data.effectName + "_position", true);
                    if (isFirstCreate)
                    {
                        asset.parent.localPosition = data.localPos;
                        asset.parent.localRotation = data.localRot;
                    }
                    timelineAsset.director.SetReferenceValue(controllerAsset.sourceGameObject.exposedName, asset.parent.gameObject);
                    
                    
                }
            }
        }
    }
}