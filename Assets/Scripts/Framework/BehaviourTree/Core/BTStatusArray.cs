using System.Data.SqlTypes;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    //状态数组，最大长度64
    public struct BTStatusArray
    {
        private static readonly int StatusBits = 2;
        private static readonly ulong StatusMask = 3;//1<<2 + 1<<1
        private static readonly int MaxLen = Marshal.SizeOf(typeof(BTStatusArray)) * 8;

        public static BTStatusArray DefaultRunning = GetDefaultRunning(BTStatus.Running);
        private ulong d1;
        private ulong d2;

        public static BTStatusArray GetDefaultRunning(BTStatus status)
        {
            BTStatusArray array = new BTStatusArray();
            array.Clear(status);
            return array;
        }

        private bool CheckIndex(int bits, int index)
        {
            if (bits > MaxLen - StatusBits)
            {
                CLog.LogError("BTStatusArray 最大支持长度为" + (MaxLen / StatusBits) + ",index=" + index);
                return false;
            }
            return true;
        }

        public BTStatus GetState(int index)
        {
            int bits = index * StatusBits;
            if (!CheckIndex(bits, index)) return BTStatus.Success;

            int statusValue = 0;
            ulong v = 0;
            if (bits >= 0 && bits < 64)
            {
                v = d1;
            }
            else if (bits >= 64 && bits < 128)
            {
                bits = bits - 64;
                v = d2;
            }
            statusValue = (int)((v >> bits) & StatusMask);
            return (BTStatus)statusValue;
        }

        public void SetState(int index, BTStatus status)
        {
            int bits = index * StatusBits;
            if (!CheckIndex(bits, index)) return;

            ulong statusValue = (ulong) status;
            if (bits >= 0 && bits < 64)
            {
                d1 &= ~(StatusMask << bits);
                d1 |= (statusValue << bits);
            }
            else if (bits >= 64 && bits < 128)
            {
                bits = bits - 64;
                d2 &= ~(StatusMask << bits);
                d2 |= (statusValue << bits);
            }
        }

        public void Clear(BTStatus status)
        {
            int len = MaxLen / StatusBits;
            for (int i = 0; i < len; i++)
            {
                SetState(i, status);
            }
        }
    }
}