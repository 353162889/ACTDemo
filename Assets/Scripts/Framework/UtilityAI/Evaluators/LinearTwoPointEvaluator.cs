using System;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class LinearTwoPointEvaluatorData : IEvaluatorData
    {
        public Vector2 pointA;
        public Vector2 pointB;
    }
    /// <summary>
    ///   The LinearTwoPointEvaluator returns a normalized utility value based on a linear function.
    /// </summary>
    public class LinearTwoPointEvaluator : TwoPointEvaluatorBase<LinearTwoPointEvaluatorData>
    {
        float _dyOverDx;

        /// <summary>
        ///   Returns the utility value for the specified x.
        /// </summary>
        /// <param name="x">The x value.</param>
        public override float OnEvaluate(float x)
        {
            return Mathf.Clamp01(Ya + _dyOverDx * (x - Xa));
        }

        protected override void Initialize(float xA, float yA, float xB, float yB)
        {
            base.Initialize(xA, yA, xB, yB);
            _dyOverDx = (Yb - Ya) / (Xb - Xa);
        }

        protected override void OnInit(LinearTwoPointEvaluatorData data)
        {
            this.Initialize(data.pointA.x, data.pointA.y, data.pointB.x, data.pointB.y);

        }
    }

}