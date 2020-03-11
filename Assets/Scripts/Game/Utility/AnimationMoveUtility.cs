using System;
using UnityEngine;

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
            //            Quaternion startQuat = Quaternion.Euler(startRot);
            //            Quaternion endQuat = Quaternion.Euler(endRot);
            //            Quaternion localRotOffset = Quaternion.Inverse(startQuat) * endQuat;
            Quaternion localRotOffset = Quaternion.Euler(endRot - startRot);
            //计算出世界位移
            Vector3 worldOffset = baseRotation * localOffset;
            return new OffsetMoveInfo() { offsetPos = worldOffset, offsetRot = localRotOffset};
        }

    }
}