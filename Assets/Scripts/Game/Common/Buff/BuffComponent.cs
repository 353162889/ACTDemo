﻿using System;
using System.Collections.Generic;
using Framework;

namespace Game
{

    public enum BuffEventType
    {
        NONE = 0,
        HOST_BUFF_ATTACH = 1,
        HOST_BUFF_DETACH = 2,
    }

    public class BuffConfig
    {
        private static Dictionary<string, BuffEventType> InitEventTypes()
        {
            Type t = typeof(BuffEventType);
            Array array = Enum.GetValues(t);
            Dictionary<string, BuffEventType> dic = new Dictionary<string, BuffEventType>();
            foreach (var value in array)
            {
                var name = Enum.GetName(t, value);
                dic.Add(name, (BuffEventType)value);
            }

            return dic;
        }

        public static Dictionary<string, BuffEventType> m_dicStrToStateType = InitEventTypes();


        public static BuffEventType GetEventTypeByString(string str)
        {
            BuffEventType t;
            if (m_dicStrToStateType.TryGetValue(str, out t))
            {
                return t;
            }
            CLog.LogError("can not find str=" + str + " BuffEventType");
            return BuffEventType.NONE;
        }
    }

    public class BuffComponent : DataComponent
    {
        public int startIndex = 1;
        //buffId对于buff数据列表
        public Dictionary<int, List<BuffData>> dicIdToBuffLst = new Dictionary<int, List<BuffData>>();
        //buff的index对于buff数据
        public Dictionary<int, BuffData> dicIndexToBuffData = new Dictionary<int, BuffData>();
        //当前帧添加的buff数据（buff的真正附加时机是在update中）
        public List<BuffData> lstAdd = new List<BuffData>();
    }
}