using System;
using System.Collections.Generic;
using System.Security.Permissions;
using Framework;
using Game;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NodeEditor
{
    public static class TimeLineUtility
    {
        private static Type DefaultTrackType = typeof(NEPlayableTrack);
        private static Dictionary<Type, Type> dicDataTypeToTrackType = new Dictionary<Type, Type>()
        {
            {typeof(BTPlayAnimationActionData), typeof(NEPlayAnimationTrack)},
            {typeof(BTAnimationMoveActionData), typeof(NEAnimationMoveTrack)},
            {typeof(BTPlayEffectActionData), typeof(NEPlayEffectTrack)},
            {typeof(BTBoxColliderActionData), typeof(NEBoxColliderTrack)},
            {typeof(BTSphereColliderActionData), typeof(NESphereColliderTrack)},
        };

        public static Type TryGetDefaultData(Type trackType)
        {
            foreach (var item in dicDataTypeToTrackType)
            {
                if (item.Value == trackType)
                {
                    return item.Key;
                }
            }

            return null;
        }

        private static int TimeLineNodeSort(NEData a, NEData b)
        {
            var aTimeData = a.data as IBTTimeLineData;
            var bTimeData = b.data as IBTTimeLineData;

            var aTime = aTimeData == null ? 0 : aTimeData.time;
            var bTime = bTimeData == null ? 0 : bTimeData.time;
            if (aTime > bTime) return 1;
            else if (aTime < bTime) return -1;
            else
            {
                return 0;
            }
        }

        public static NEData ConvertTimeLineToNEData(PlayableDirector director)
        {
            if (director == null || director.playableAsset == null) return null;
            NEData timelineData = new NEData();
            timelineData.data = new BTTimeLineData();
            timelineData.lstChild = new List<NEData>();
            var timelineAsset = (TimelineAsset)director.playableAsset;
            var tracks = timelineAsset.GetRootTracks();
            foreach (var trackAsset in tracks)
            {
                if (trackAsset is INEPlayableTrack)
                {
                    var neTrack = trackAsset;
                    var datas = TimeLineUtility.GetNEPlayableTrackNEDatas(neTrack);
                    timelineData.lstChild.AddRange(datas);
                }
            }
            timelineData.lstChild.Sort(TimeLineNodeSort);
            return timelineData;
        }

        public static bool ConvertNEDataToTimeLine(PlayableDirector director, NEData neData, GameObject prefab)
        {
            var timelineAsset = TimelineAsset.CreateInstance<NETimelineAsset>();
            director.playableAsset = timelineAsset;
          
            var goModelMove = new GameObject();
            director.gameObject.AddChildToParent(goModelMove, "ModelMove");
            timelineAsset.director = director;
            var moveAnimator = goModelMove.AddComponent<Animator>();
            var viewGroupTrack = timelineAsset.CreateTrack<GroupTrack>("ViewGroupTrack");
            timelineAsset.viewGroupTrack = viewGroupTrack;
            //添加一个AnimationTrack用于放置动画
            var animTrack = timelineAsset.CreateTrack<AnimationTrack>(viewGroupTrack,"ModelAnimationTrack");
            timelineAsset.modelAnimTrack = animTrack;
            if (prefab != null)
            {
                var model = GameObject.Instantiate(prefab);
                goModelMove.AddChildToParent(model.gameObject, "TimeLineModel");
                var animator = model.transform.GetComponentInChildren<Animator>();
                if (animator != null)
                {
                    director.SetGenericBinding(animTrack, animator);
                }
            }

            //添加一个AnimationTrack用于配置角色移动数据
            var modelMoveTrack = timelineAsset.CreateTrack<AnimationTrack>(viewGroupTrack, "AnimationMoveTrack");
            timelineAsset.modelMoveTrack = modelMoveTrack;

            //添加一个controlTrack用于显示特效
            var effectTrack = timelineAsset.CreateTrack<ControlTrack>(viewGroupTrack, "EffectTrack");
            timelineAsset.effectTrack = effectTrack;
            var effectGlobalParent = new GameObject("effectGlobalParent");
            director.gameObject.AddChildToParent(effectGlobalParent);
            timelineAsset.effectGlobalParent = effectGlobalParent.transform;

            var colliderParent = new GameObject("colliderParent");
            goModelMove.AddChildToParent(colliderParent);
            timelineAsset.colliderParent = colliderParent.transform;

            director.SetGenericBinding(modelMoveTrack, moveAnimator);
            var type = typeof(IBTTimeLineData);
            for (int i = 0; i < neData.lstChild.Count; i++)
            {
                var childNEData = neData.lstChild[i];
                if (!type.IsInstanceOfType(childNEData.data))
                {
                    Debug.LogError("timeline node 中 "+ type.Name + "不是IBTTimeLineData对象");
                    return false;
                }

               
                if (childNEData.data is BTTimeDecoratorData)
                {
                    if (childNEData.lstChild.Count > 0)
                    {
                        var curNEData = childNEData.lstChild[0];
                        Type trackType;
                        if (!dicDataTypeToTrackType.TryGetValue(curNEData.data.GetType(), out trackType))
                        {
                            trackType = DefaultTrackType;
                        }
                        var track = timelineAsset.CreateTrack(trackType, null, trackType.Name);
                        var clip = track.CreateDefaultClip();
                        var asset = (INEPlayableAsset) clip.asset;
                        clip.start = ((IBTTimeLineData) (childNEData.data)).time;
                        if (curNEData.data is IBTTimelineDurationData)
                        {
                            float duration = ((IBTTimelineDurationData)(curNEData.data)).duration;
                            if (duration > 0) clip.duration = duration;
                        }
                        asset.neData = curNEData;
                        clip.displayName = GetNEPlayableAssetName(asset);
                        asset.InitInspector();
                    }
                }
                else
                {
                    var curNEData = childNEData;
                    Type trackType;
                    if (!dicDataTypeToTrackType.TryGetValue(curNEData.data.GetType(), out trackType))
                    {
                        trackType = DefaultTrackType;
                    }
                    var track = timelineAsset.CreateTrack(trackType, null, trackType.Name);
                    var clip = track.CreateDefaultClip();
                    var asset = (INEPlayableAsset)clip.asset;
                    clip.start = ((IBTTimeLineData)(childNEData.data)).time;
                    if (curNEData.data is IBTTimelineDurationData)
                    {
                        float duration = ((IBTTimelineDurationData)(curNEData.data)).duration;
                        if (duration > 0) clip.duration = duration;
                    }
                    asset.neData = childNEData;
                    clip.displayName = GetNEPlayableAssetName(asset);
                    asset.InitInspector();
                }
            }
            return true;
        }

        public static string GetNEPlayableAssetName(INEPlayableAsset asset)
        {
            if (asset.neData != null && asset.neData.data != null)
            {
                string name = asset.neData.data.GetType().Name;
                if (name.StartsWith("BT"))
                {
                    name = name.Substring(2);
                }

                if (name.EndsWith("Data"))
                {
                    name = name.Substring(0, name.Length - 4);
                }

                return name;
            }
            else
            {
                return "Null";
            }
        }

        public static List<NEData> GetNEPlayableTrackNEDatas(TrackAsset trackAsset)
        {
            List<NEData> lst = new List<NEData>();
            var clips = trackAsset.GetClips();
            foreach (var clip in clips)
            {
                var asset = clip.asset as INEPlayableAsset;
                NEData convertNEData = null;
                if (asset != null)
                {
                    convertNEData = asset.ConvertNEData();
                }
                if (convertNEData != null && convertNEData.data != null)
                {
                    if (convertNEData.data is IBTTimelineDurationData)
                    {
                        ((IBTTimelineDurationData)convertNEData.data).duration = (float)clip.duration;
                    }
                    if (convertNEData.data is IBTTimeLineData)
                    {
                        ((IBTTimeLineData)convertNEData.data).time = (float)clip.start;
                        lst.Add(convertNEData);
                    }
                    else
                    {
                        NEData neData = new NEData();
                        var timeDecoratorData = new BTTimeDecoratorData();
                        timeDecoratorData.time = (float)clip.start;
                        neData.data = timeDecoratorData;
                        neData.lstChild = new List<NEData>();
                        neData.lstChild.Add(convertNEData);
                        lst.Add(neData);
                    }
                }
            }
            return lst;
        }

        public static void InitNEPlayableClip(TimelineClip clip)
        {
            if (clip.asset is INEPlayableAsset)
            {
                ((INEPlayableAsset) clip.asset).curTimelineClip = clip;
                UpdateNEPlayableClipName(clip);
            }
        }

        public static void UpdateNEPlayableClipName(TimelineClip clip)
        {
            if (clip.asset is INEPlayableAsset)
            {
                var asset = (INEPlayableAsset)(clip.asset);
                if (asset.neData == null || asset.neData.data == null)
                {
                    clip.displayName = "Null";
                }
                else
                {
                    clip.displayName = TimeLineUtility.GetNEPlayableAssetName(asset);
                }
            }
        }
    }
}