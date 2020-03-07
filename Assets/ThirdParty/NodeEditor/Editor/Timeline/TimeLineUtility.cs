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

        public static bool ConvertNEDataToTimeLine(PlayableDirector director, NEData neData)
        {
            var timelineAsset = (NETimelineAsset)director.playableAsset;
          
            var go = new GameObject();
            director.gameObject.AddChildToParent(go, "ModelMove");
            timelineAsset.director = director;
            var moveAnimator = go.AddComponent<Animator>();
            var viewGroupTrack = timelineAsset.CreateTrack<GroupTrack>("ViewGroupTrack");
            timelineAsset.viewGroupTrack = viewGroupTrack;
            //添加一个AnimationTrack用于放置动画
            var animTrack = timelineAsset.CreateTrack<AnimationTrack>(viewGroupTrack,"ModelAnimationTrack");
            timelineAsset.modelAnimTrack = animTrack;
            string playerPath = "Assets/ResourceEx/Prefab/Player.prefab";
            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(playerPath);
            if (playerPrefab != null)
            {
                var player = GameObject.Instantiate(playerPrefab);
                go.AddChildToParent(player.gameObject, "Player");
                var animator = player.transform.GetComponentInChildren<Animator>();
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
                if (asset != null && asset.neData != null && asset.neData.data != null)
                {
                    if (asset.neData.data is IBTTimeLineData)
                    {
                        ((IBTTimeLineData)asset.neData.data).time = (float)clip.start;
                        lst.Add(asset.neData);
                    }
                    else
                    {
                        NEData neData = new NEData();
                        var timeDecoratorData = new BTTimeDecoratorData();
                        timeDecoratorData.time = (float)clip.start;
                        neData.data = timeDecoratorData;
                        neData.lstChild = new List<NEData>();
                        neData.lstChild.Add(asset.neData);
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