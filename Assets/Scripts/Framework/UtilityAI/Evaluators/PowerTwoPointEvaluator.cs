using System;
using UnityEngine;


namespace Framework
{
    [Serializable]
    public class PowerTwoPointEvaluatorData : IEvaluatorData
    {
        public Vector2 pointA;
        public Vector2 pointB;
        public float power = 2;
    }

    /// <summary>
    ///   The PowerTwoPointEvaluator returns a normalized utility based on a power function that has
    ///   effectively 1 parameter ( 0 leq p le 10000 ) bounded by the box defined by PtA and PtB with
    ///   PtA.x being strictly less than PtB.x!
    ///   <see href="https://www.desmos.com/calculator/jjzwwnn5of">Power</see> for an interactive
    ///   plot.
    /// </summary>
    public class PowerTwoPointEvaluator : TwoPointEvaluatorBase<PowerTwoPointEvaluatorData>
    {
        float _dy;
        float _p;

        /// <summary>
        ///   Returns the utility for the specified value x.
        /// </summary>
        /// <param name="x">The x value.</param>
        public override float OnEvaluate(float x)
        {
            var cx = Mathf.Clamp(x, Xa, Xb);
            cx = _dy * (float) Math.Pow((cx - Xa) / (Xb - Xa), _p) + Ya;
            return cx;
        }

        protected override void Initialize(float xA, float yA, float xB, float yB)
        {
            base.Initialize(xA, yA, xB, yB);
            _dy = Yb - Ya;
        }

        const float MinP = 0.0f;
        const float MaxP = 10000f;
        protected override void OnInit(PowerTwoPointEvaluatorData data)
        {
            _p = Mathf.Clamp(data.power, MinP, MaxP);
            this.Initialize(data.pointA.x, data.pointA.y, data.pointB.x, data.pointB.y);
        }
    }

}