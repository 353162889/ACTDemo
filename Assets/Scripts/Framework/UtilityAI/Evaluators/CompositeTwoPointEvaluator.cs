using System;
using System.Collections.Generic;


namespace Framework
{
    [Serializable]
    public class CompositeTwoPointEvaluatorData : IEvaluatorData
    {
        public List<IEvaluatorData> mLstEvaluatorDatas;
    }

    public class CompositeTwoPointEvaluator : TwoPointEvaluatorBase<CompositeTwoPointEvaluatorData>
    {
        public override float OnEvaluate(float x)
        {
            var ev = FindEvaluator(x);
            // if ev is null then there is a "hole" in the XInterval.
            return ev != null ? ev.Evaluate(x) : LinearHoleInterpolator(x);
        }

        /// <summary>
        ///   Add the specified Evaluator.
        /// </summary>
        /// <param name="ev">Ev.</param>
        public void Add(ITwoPointEvaluator ev)
        {
            if (DoesNotOverlapWithAnyEvaluator(ev))
                Evaluators.Add(ev);
            //_evaluators.Sort();
            Evaluators.Sort((e1, e2) => e1.XInterval.CompareTo(e2.XInterval));
            UpdateXyPoints();
        }

        public CompositeTwoPointEvaluator()
        {
            Evaluators = new List<ITwoPointEvaluator>();
        }

        bool DoesNotOverlapWithAnyEvaluator(ITwoPointEvaluator ev)
        {
            foreach (var cev in Evaluators)
                if (ev.XInterval.Overlaps(cev.XInterval))
                    if (ev.XInterval.Adjacent(cev.XInterval))
                        continue;
                    else
                        return false;

            return true;
        }

        void UpdateXyPoints()
        {
            var count = Evaluators.Count;
            if (count == 1)
                SingleEvaluatorXyPointsUpdate();
            else
                MultiEvaluatorXyPointsUpdate();
        }

        void SingleEvaluatorXyPointsUpdate()
        {
            Xa = Evaluators[0].PtA.x;
            Ya = Evaluators[0].PtA.y;
            Xb = Evaluators[0].PtB.x;
            Yb = Evaluators[0].PtB.y;
        }

        void MultiEvaluatorXyPointsUpdate()
        {
            foreach (var ev in Evaluators)
            {
                if (Xa >= ev.MinX)
                {
                    Xa = ev.MinX;
                    Ya = ev.PtA.y;
                }

                if (Xb <= ev.MaxX)
                {
                    Xb = ev.MaxX;
                    Yb = ev.PtB.y;
                }
            }
        }

        /// <summary>
        ///   Attempts to find an evaluator. The only Time this will return null is when
        ///   the value x is within the XInterval of the composite evaluator but there is
        ///   no evaluator within the interval that contains x.
        /// </summary>
        /// <returns>The evaluator.</returns>
        /// <param name="x">X.</param>
        ITwoPointEvaluator FindEvaluator(float x)
        {
            int evCount = Evaluators.Count;
            if (XInterval.Contains(x))
                return FindInternalEvaluator(x);

            if (x > XInterval.UpperBound)
                return Evaluators[evCount - 1];

            return x < XInterval.LowerBound ? Evaluators[0] : null;
        }

        ITwoPointEvaluator FindInternalEvaluator(float x)
        {
            int evCount = Evaluators.Count;
            for (int i = 0; i < evCount; i++)
                if (Evaluators[i].XInterval.Contains(x))
                    return Evaluators[i];
            // x is in a "hole"
            return null;
        }

        float LinearHoleInterpolator(float x)
        {
            var lrev = FindLeftAndRightInterpolators(x);
            var xl = lrev.Key.MaxX;
            var yl = lrev.Key.Evaluate(xl);
            var xr = lrev.Value.MinX;
            var yr = lrev.Value.Evaluate(xr);
            var alpha = (x - xl) / (xr - xl);
            return yl + alpha * (yr - yl);
        }

        KeyValuePair<ITwoPointEvaluator, ITwoPointEvaluator> FindLeftAndRightInterpolators(float x)
        {
            int evCount = Evaluators.Count;
            ITwoPointEvaluator lev = null;
            ITwoPointEvaluator rev = null;
            for (int i = 0; i < evCount - 1; i++)
            {
                lev = Evaluators[i];
                rev = Evaluators[i + 1];
                if (x > lev.XInterval.UpperBound &&
                    x < rev.XInterval.LowerBound)
                    break;
            }

            return new KeyValuePair<ITwoPointEvaluator, ITwoPointEvaluator>(lev, rev);
        }

        internal List<ITwoPointEvaluator> Evaluators;
        protected override void OnInit(CompositeTwoPointEvaluatorData data)
        {
            if (data.mLstEvaluatorDatas != null)
            {
                for (int i = 0; i < data.mLstEvaluatorDatas.Count; i++)
                {
                    var childData = data.mLstEvaluatorDatas[i];
                    var child = (ITwoPointEvaluator)this.utilityAI.CreateUtility(childData);
                    if (child != null)
                    {
                        Add(child);
                    }
                }
            }

        }
    }

}