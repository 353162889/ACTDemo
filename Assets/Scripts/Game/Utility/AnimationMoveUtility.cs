using System;
using UnityEngine;

namespace Game
{
    public static class AnimationMoveUtility
    {
        //通过时间与距离重建路径点
        public static float[] GetPoints(float[] originPoints, float duration, float distance)
        {
            if(originPoints == null || originPoints.Length == 0)return new float[0];
            float[] resultPoints = new float[originPoints.Length];
            int len = originPoints.Length / 4;
            float totalTime = 0;
            float totalDistance = 0;
            if (len > 0)
            {
                int index = (len - 1) * 4;
                totalTime = originPoints[index];
            }
            for (int i = 0; i < len - 1; i++)
            {
                int startIndex = i * 4;
                int nextIndex = (i + 1) * 4;
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
            }

            for (int i = 1; i < len; i++)
            {
                int index = i * 4;
                resultPoints[index] = originPoints[index] * timeScale;
                resultPoints[index + 1] = resultPoints[index - 3] + (originPoints[index + 1] - originPoints[index - 3]) * distanceScale;
                resultPoints[index + 2] = resultPoints[index - 2] + (originPoints[index + 2] - originPoints[index - 2]) * distanceScale;
                resultPoints[index + 3] = resultPoints[index - 1] + (originPoints[index + 3] - originPoints[index - 1]) * distanceScale;
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
        public static Vector3 GetOffset(float[] points, Quaternion baseRotation, float startTime, float endTime)
        {
            if (points == null || points.Length <= 4)
            {
                return Vector3.zero;
            }
            int dataLen = points.Length >> 2;

            

            int leftStartSearchIndex = 0;
            for (int i = dataLen - 1; i > -1; i--)
            {
                if (startTime >= points[i << 2])
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
                if (endTime >= points[i << 2])
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

            int leftStart = leftStartSearchIndex * 4;
            int rightStart = rightStartSearchIndex * 4;

            Vector3 leftStartPoint = new Vector3(points[leftStart + 1], points[leftStart + 2], points[leftStart + 3]);
            Vector3 rightStartPoint = new Vector3(points[rightStart + 1], points[rightStart + 2], points[rightStart + 3]);

            int leftEnd = leftEndSearchIndex * 4;
            int rightEnd = rightEndSearchIndex * 4;
            Vector3 leftEndPoint = new Vector3(points[leftEnd + 1], points[leftEnd + 2], points[leftEnd + 3]);
            Vector3 rightEndPoint = new Vector3(points[rightEnd + 1], points[rightEnd + 2], points[rightEnd + 3]);


            float startDuration = points[rightStart] - points[leftStart];
            float startLerp = 1;//如果时间一样，取后一个点的坐标
            if (startDuration > 0)
            {
                startLerp = (startTime - points[leftStart]) / startDuration;
            }
            Vector3 startPoint = Vector3.Lerp(leftStartPoint, rightStartPoint, startLerp);

            float endDuration = points[rightEnd] - points[leftEnd];
            float endLerp = 1;
            if (endDuration > 0)
            {
                endLerp = (endTime - points[leftEnd]) / endDuration;
            }

            Vector3 endPoint = Vector3.Lerp(leftEndPoint, rightEndPoint, endLerp);

            //局部位移
            Vector3 localOffset = endPoint - startPoint;
            //计算出世界位移
            Vector3 worldOffset = baseRotation * localOffset;
            return worldOffset;
        }

    }
}