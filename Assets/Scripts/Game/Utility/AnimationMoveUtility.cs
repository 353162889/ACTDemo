using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public static class AnimationMoveUtility
    {
        public static int DataSpace = 7;

        //通过时间与距离重建路径点
        public static float[] GetPoints(float[] originPoints, float duration, float distance)
        {
            if(originPoints == null || originPoints.Length == 0)return new float[0];
            float[] resultPoints = new float[originPoints.Length];
            int len = originPoints.Length / DataSpace;
            float totalTime = 0;
            float totalDistance = 0;
            if (len > 0)
            {
                int index = (len - 1) * DataSpace;
                totalTime = originPoints[index];
            }
            for (int i = 0; i < len - 1; i++)
            {
                int startIndex = i * DataSpace;
                int nextIndex = (i + 1) * DataSpace;
                float x = originPoints[startIndex + 1];
                float y = originPoints[startIndex + 2];
                float z = originPoints[startIndex + 3];
                float nextX = originPoints[nextIndex + 1];
                float nextY = originPoints[nextIndex + 2];
                float nextZ = originPoints[nextIndex + 3];
                float num1 = nextX - x;
                float num2 = nextY - y;
                float num3 = nextZ - z;
                totalDistance += (float)Math.Sqrt((double)num1 * (double)num1 + (double)num2 * (double)num2 + (double)num3 * (double)num3);
            }

            float timeScale = 1;
            float distanceScale = 1;
            if (duration > 0 && totalTime > 0)
            {
                timeScale = duration / totalTime;
            }

            if (distance > 0 && totalDistance > 0)
            {
                distanceScale = distance / totalDistance;
            }

            if (len > 0)
            {
                resultPoints[0] = originPoints[0];
                resultPoints[1] = originPoints[1];
                resultPoints[2] = originPoints[2];
                resultPoints[3] = originPoints[3];
                resultPoints[4] = originPoints[4];
                resultPoints[5] = originPoints[5];
                resultPoints[6] = originPoints[6];
            }

            for (int i = 1; i < len; i++)
            {
                Quaternion a;
                int index = i * DataSpace;
                resultPoints[index] = originPoints[index] * timeScale;
                if (distanceScale != 1)
                {
                    //x
                    resultPoints[index + 1] = resultPoints[index - DataSpace + 1] +
                                              (originPoints[index + 1] - originPoints[index - DataSpace + 1]) *
                                              distanceScale;
                    //y
                    resultPoints[index + 2] = resultPoints[index - DataSpace + 2] +
                                              (originPoints[index + 2] - originPoints[index - DataSpace + 2]) *
                                              distanceScale;
                    //z
                    resultPoints[index + 3] = resultPoints[index - DataSpace + 3] +
                                              (originPoints[index + 3] - originPoints[index - DataSpace + 3]) *
                                              distanceScale;
                }
                else
                {
                    resultPoints[index + 1] = originPoints[index + 1];
                    resultPoints[index + 2] = originPoints[index + 2];
                    resultPoints[index + 3] = originPoints[index + 3];
                }

                resultPoints[index + 4] = originPoints[index + 4];
                resultPoints[index + 5] = originPoints[index + 5];
                resultPoints[index + 6] = originPoints[index + 6];
            }
            return resultPoints;
        }

        /// <summary>
        /// 根据时间获取到移动的偏移量
        /// </summary>
        /// <param name="points"></param>
        /// <param name="baseFace"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public struct OffsetMoveInfo
        {
            public static OffsetMoveInfo Empty = new OffsetMoveInfo(){offsetPos = Vector3.zero, offsetRot = Quaternion.identity};
            public Vector3 offsetPos;
            public Quaternion offsetRot;
        }
        public static OffsetMoveInfo GetOffset(float[] points, Quaternion baseRotation, float startTime, float endTime)
        {
            if (points == null || points.Length <= DataSpace)
            {
                return OffsetMoveInfo.Empty;
            }
            int dataLen = points.Length / DataSpace;

            int leftStartSearchIndex = 0;
            for (int i = dataLen - 1; i > -1; i--)
            {
                if (startTime >= points[i * DataSpace])
                {
                    leftStartSearchIndex = i;
                    break;
                }
            }

            int rightStartSearchIndex = leftStartSearchIndex + 1;
            if (rightStartSearchIndex >= dataLen) rightStartSearchIndex = dataLen - 1;

            int leftEndSearchIndex = leftStartSearchIndex;
            for (int i = leftStartSearchIndex; i < dataLen; i++)
            {
                if (endTime >= points[i * DataSpace])
                {
                    leftEndSearchIndex = i;
                }
                else
                {
                    break;
                }
            }

            int rightEndSearchIndex = leftEndSearchIndex + 1;
            if (rightEndSearchIndex >= dataLen) rightEndSearchIndex = dataLen - 1;

            int leftStart = leftStartSearchIndex * DataSpace;
            int rightStart = rightStartSearchIndex * DataSpace;

            Vector3 leftStartPoint = new Vector3(points[leftStart + 1], points[leftStart + 2], points[leftStart + 3]);
            Vector3 rightStartPoint = new Vector3(points[rightStart + 1], points[rightStart + 2], points[rightStart + 3]);
            Vector3 leftStartRot = new Vector3(points[leftStart + 4], points[leftStart + 5], points[leftStart + 6]);
            Vector3 rightStartRot = new Vector3(points[rightStart + 4], points[rightStart + 5], points[rightStart + 6]);

            int leftEnd = leftEndSearchIndex * DataSpace;
            int rightEnd = rightEndSearchIndex * DataSpace;
            Vector3 leftEndPoint = new Vector3(points[leftEnd + 1], points[leftEnd + 2], points[leftEnd + 3]);
            Vector3 rightEndPoint = new Vector3(points[rightEnd + 1], points[rightEnd + 2], points[rightEnd + 3]);
            Vector3 leftEndRot = new Vector3(points[leftEnd + 4], points[leftEnd + 5], points[leftEnd + 6]);
            Vector3 rightEndRot = new Vector3(points[rightEnd + 4], points[rightEnd + 5], points[rightEnd + 6]);


            float startDuration = points[rightStart] - points[leftStart];
            float startLerp = 1;//如果时间一样，取后一个点的坐标
            if (startDuration > 0)
            {
                startLerp = (startTime - points[leftStart]) / startDuration;
            }
            Vector3 startPoint = Vector3.Lerp(leftStartPoint, rightStartPoint, startLerp);
            Vector3 startRot = Vector3.Lerp(leftStartRot, rightStartRot, startLerp);

            float endDuration = points[rightEnd] - points[leftEnd];
            float endLerp = 1;
            if (endDuration > 0)
            {
                endLerp = (endTime - points[leftEnd]) / endDuration;
            }

            Vector3 endPoint = Vector3.Lerp(leftEndPoint, rightEndPoint, endLerp);
            Vector3 endRot = Vector3.Lerp(leftEndRot, rightEndRot, endLerp);

            //局部位移
            Vector3 localOffset = endPoint - startPoint;
            Quaternion localRotOffset = Quaternion.Euler(endRot - startRot);
            //计算出世界位移
            Vector3 worldOffset = baseRotation * localOffset;
            return new OffsetMoveInfo() { offsetPos = worldOffset, offsetRot = localRotOffset};
        }

#if UNITY_EDITOR
        private static string[] PropertyNames = new[] { "m_LocalPosition.x", "m_LocalPosition.y", "m_LocalPosition.z", "localEulerAnglesRaw.x", "localEulerAnglesRaw.y", "localEulerAnglesRaw.z" };
        private static void UpdateInfo(Dictionary<float, float[]> map, AnimationCurve animCurve, int index)
        {
            for (int i = 0; i < animCurve.length; i++)
            {
                var time = animCurve.keys[i].time;
                var value = animCurve.keys[i].value;
                if (!map.ContainsKey(time)) map.Add(time, new float[] { 0, 0, 0, 0, 0, 0, 0 });
                map[time][index] = value;
            }
        }
        private struct MoveInfo
        {
            public float time;
            public float[] offsetInfo;

            public MoveInfo(float time, float[] offset)
            {
                this.time = time;
                this.offsetInfo = offset;
            }
        }
        public static float[] ConvertClipToPoints(AnimationClip clip, float duration = -1, string path = "")
        {
            var binds = AnimationUtility.GetCurveBindings(clip);
            Dictionary<float, float[]> map = new Dictionary<float, float[]>();
            foreach (var bind in binds)
            {
                if (bind.path == path)
                {
                    int index = Array.IndexOf(PropertyNames, bind.propertyName);
                    if (index < 0) continue;
                    var animCurve = AnimationUtility.GetEditorCurve(clip, bind);
                    UpdateInfo(map, animCurve, index);
                }
            }

            List<MoveInfo> infos = new List<MoveInfo>();
            foreach (var info in map)
            {
                infos.Add(new MoveInfo(info.Key, info.Value));
            }
            infos.Sort((MoveInfo a, MoveInfo b) =>
            {
                if (a.time < b.time)
                {
                    return -1;
                }
                else if (a.time > b.time)
                {
                    return 1;
                }
                return 0;
            });
            if (infos.Count > 0)
            {
                //第一帧添加时间为0
                if (infos[0].time > 0)
                {

                    infos.Insert(0, new MoveInfo(0, new float[] { 0, 0, 0, 0, 0, 0, 0 }));

                }
                float lastTime = clip.length;
                //添加最后一帧
                if (duration > 0)
                    lastTime = duration;
                
                if (!Mathf.Approximately(infos[infos.Count - 1].time, lastTime) && infos[infos.Count - 1].time < lastTime)
                {
                    var temp = infos[infos.Count - 1].offsetInfo;
                    infos.Add(new MoveInfo(lastTime, new float[] { temp[0], temp[1], temp[2], temp[3], temp[4], temp[5], temp[6] }));
                }
            }

            float[] points = new float[infos.Count * AnimationMoveUtility.DataSpace];
            for (int i = 0; i < infos.Count; i++)
            {
                int index = i * AnimationMoveUtility.DataSpace;
                points[index] = infos[i].time;
                points[index + 1] = infos[i].offsetInfo[0];
                points[index + 2] = infos[i].offsetInfo[1];
                points[index + 3] = infos[i].offsetInfo[2];
                points[index + 4] = infos[i].offsetInfo[3];
                points[index + 5] = infos[i].offsetInfo[4];
                points[index + 6] = infos[i].offsetInfo[5];
            }

            return points;
        }

        public static void ConvertPointsToTimelineClip(float[] points, ref TimelineClip timelineClip)
        {
            if (points != null && points.Length > 0)
            {
                int len = points.Length / AnimationMoveUtility.DataSpace;
                AnimationCurve[] curves = new AnimationCurve[PropertyNames.Length];
                for (int i = 0; i < curves.Length; i++)
                {
                    curves[i] = new AnimationCurve();
                }
                for (int i = 0; i < len; i++)
                {
                    int startIndex = i * AnimationMoveUtility.DataSpace;
                    float time = points[startIndex];
                    for (int j = 0; j < curves.Length; j++)
                    {
                        curves[j].AddKey(time, points[startIndex + j + 1]);
                    }
                }

                var moveClip = timelineClip.animationClip;
                for (int i = 0; i < curves.Length; i++)
                {
                    moveClip.SetCurve("", typeof(Transform), PropertyNames[i], curves[i]);
                }

                timelineClip.duration = moveClip.length;
            }
        }
#endif

    }
}