using System;
using UnityEngine;


namespace Framework
{
    [Serializable]
    public class SigmoidTwoPointEvaluatorData : IEvaluatorData
    {
        public Vector2 pointA;
        public Vector2 pointB;
        public float k = -0.6f;
    }

    /// <summary>
    ///   The SigmoidTwoPointEvaluator returns a normalized utility based on a sigmoid function that has
    ///   effectively 1 parameter ( -0.99999f leq k leq 0.99999f ) bounded by the box defined by PtA and PtB with
    ///   PtA.x being strictly less than PtB.x!
    ///   <see href="https://www.desmos.com/calculator/rf8mrgolws">Parametrised Sigmoid</see> for an interactive
    ///   plot.
    /// </summary>
    public class SigmoidTwoPointEvaluator : TwoPointEvaluatorBase<SigmoidTwoPointEvaluatorData>
    {
        float _dyOverTwo;
        float _k;
        float _oneMinusK;
        float _twoOverDx;
        float _xMean;
        float _yMean;

        /// <summary>
        ///   Returns the utility value for the specified value x.
        /// </summary>
        /// <param name="x">The x value.</param>
        public override float OnEvaluate(float x)
        {
            var cxMinusXMean = Mathf.Clamp(x, Xa, Xb) - _xMean;
            var num = _twoOverDx * cxMinusXMean * _oneMinusK;
            var den = _k * (1 - 2 * Math.Abs(_twoOverDx * cxMinusXMean)) + 1;
            var val = _dyOverTwo * (num / den) + _yMean;
            return val;
        }

        const float MinK = -0.99999f;
        const float MaxK = 0.99999f;

        protected override void Initialize(float xA, float yA, float xB, float yB)
        {
            base.Initialize(xA, yA, xB, yB);
            _twoOverDx = Math.Abs(2.0f / (Xb - Xa));
            _xMean = (Xa + Xb) / 2.0f;
            _yMean = (Ya + Yb) / 2.0f;
            _dyOverTwo = (Yb - Ya) / 2.0f;
            _oneMinusK = 1.0f - _k;
        }

        protected override void OnInit(SigmoidTwoPointEvaluatorData data)
        {
            _k = Mathf.Clamp(data.k, MinK, MaxK);
            this.Initialize(data.pointA.x, data.pointA.y, data.pointB.x, data.pointB.y);
        }
    }

}