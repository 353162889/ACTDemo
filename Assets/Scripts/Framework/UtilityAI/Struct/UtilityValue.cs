using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 效用值，记录当前决策因子的值与权重
    /// </summary>
    [Serializable]
    public struct UtilityValue : IEquatable<UtilityValue>, IComparable<UtilityValue>
    {
        float m_fValue;

        float m_fWeight;

        float m_fCombine;

        public float Value
        {
            get { return m_fValue; }
            set
            {
                m_fValue = Mathf.Clamp01(value);
                CalCombine();
            }
        }

        public float Weight
        {
            get { return m_fWeight; }
            set
            {
                m_fWeight = Mathf.Clamp01(value);
                CalCombine();
            }
        }

        public float Combined
        {
            get { return m_fCombine; }
        }

        public bool IsZero
        {
            get { return Mathf.Approximately(Combined, 0); }
        }

        public bool IsOne
        {
            get { return Mathf.Approximately(Combined, 1.0f); }
        }

        void CalCombine()
        {
            m_fCombine = m_fValue * m_fWeight;
        }

        public UtilityValue(float value)
        {
            m_fValue = Mathf.Clamp01(value);
            m_fWeight = 1.0f;
            m_fCombine = m_fValue * m_fWeight;
        }

        public UtilityValue(float value, float weight)
        {
            m_fValue = Mathf.Clamp01(value);
            m_fWeight = Mathf.Clamp01(weight);
            m_fCombine = m_fValue * m_fWeight;
        }

        public bool Equals(UtilityValue other)
        {
            return Mathf.Approximately(Value, other.Value) && Mathf.Approximately(Weight, other.Weight);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var util = (UtilityValue)obj;
            return Equals(util);
        }

        public override int GetHashCode()
        {
            return Combined.GetHashCode();
        }

        public int CompareTo(UtilityValue other)
        {
            return Combined.CompareTo(other.Combined);
        }

//        public static implicit operator UtilityValue(float a)
//        {
//            return new UtilityValue(a);
//        }

        public static bool operator ==(UtilityValue a, UtilityValue b)
        {
            return Mathf.Approximately(a.Value, b.Value) && Mathf.Approximately(a.Weight, b.Weight);
        }

        public static bool operator !=(UtilityValue a, UtilityValue b)
        {
            return Mathf.Approximately(a.Value, b.Value) || Mathf.Approximately(a.Weight, b.Weight);
        }

        public static bool operator >(UtilityValue a, UtilityValue b)
        {
            return a.Combined > b.Combined;
        }

        public static bool operator <(UtilityValue a, UtilityValue b)
        {
            return a.Combined < b.Combined;
        }

        public static bool operator >=(UtilityValue a, UtilityValue b)
        {
            return a.Combined >= b.Combined;
        }

        public static bool operator <=(UtilityValue a, UtilityValue b)
        {
            return a.Combined <= b.Combined;
        }

        public override string ToString()
        {
            return string.Format("[UtilityValue: Value={0}, Weight={1}, Combined={2}]", Value, Weight, Combined);
        }
    }

}