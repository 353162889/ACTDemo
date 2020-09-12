using System;
using UnityEngine;


namespace Framework
{
    public interface ITwoPointEvaluator : IDecisionFactorEvaluator
    {
        Vector2 PtA { get; }

        Vector2 PtB { get; }

        float MinX { get; }

        float MaxX { get; }

        float MinY { get; }

        float MaxY { get; }

        Interval<float> XInterval { get; }

        Interval<float> YInterval { get; }
    }

    public abstract class TwoPointEvaluatorBase<T> : EvaluatorBase<T>, ITwoPointEvaluator, IComparable<TwoPointEvaluatorBase<T>> where T : IUtilityData
    {
        /// <summary>
        ///   The first point in terms of x!
        /// </summary>
        public Vector2 PtA
        {
            get { return new Vector2(Xa, Ya); }
        }

        /// <summary>
        ///   The second point in terms of x!
        /// </summary>
        public Vector2 PtB
        {
            get { return new Vector2(Xb, Yb); }
        }

        /// <summary>
        ///   Gets the minimum x.
        /// </summary>
        /// <value>The minimum x.</value>
        public float MinX
        {
            get { return Xa; }
        }

        /// <summary>
        ///   Gets the max x.
        /// </summary>
        /// <value>The max x.</value>
        public float MaxX
        {
            get { return Xb; }
        }

        /// <summary>
        ///   Gets the minimum y.
        /// </summary>
        /// <value>The minimum y.</value>
        public float MinY
        {
            get { return Math.Min(Ya, Yb); }
        }

        /// <summary>
        ///   Gets the max y.
        /// </summary>
        /// <value>The max y.</value>
        public float MaxY
        {
            get { return Math.Max(Ya, Yb); }
        }

        /// <summary>
        ///   Gets the X interval.
        /// </summary>
        /// <value>The X interval.</value>
        public Interval<float> XInterval
        {
            get { return new Interval<float>(MinX, MaxX); }
        }

        /// <summary>
        ///   Gets the Y interval.
        /// </summary>
        /// <value>The Y interval.</value>
        public Interval<float> YInterval
        {
            get { return new Interval<float>(MinY, MaxY); }
        }

        public int CompareTo(TwoPointEvaluatorBase<T> other)
        {
            return XInterval.CompareTo(other.XInterval);
        }

        protected virtual void Initialize(float xA, float yA, float xB, float yB)
        {
            if (Mathf.Approximately(xA, xB))
                throw new EvaluatorDxZeroException();
            if (xA > xB)
                throw new EvaluatorXaGreaterThanXbException();

            Xa = xA;
            Xb = xB;
            Ya = Mathf.Clamp01(yA);
            Yb = Mathf.Clamp01(yB);
        }

        protected float Xa;
        protected float Xb;
        protected float Ya;
        protected float Yb;

        internal class EvaluatorDxZeroException : Exception
        {
        }

        internal class EvaluatorXaGreaterThanXbException : Exception
        {
        }

    }

}