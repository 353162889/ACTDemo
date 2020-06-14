using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using Game;
using UnityEngine;

namespace Game
{

    public enum BuffStateType
    {
        //无
        NONE = 0,
        //硬直
        STIFFNESS = 1,
        //眩晕
        STUNNED = 2,
        //冰冻
        FROZEN = 3,
        //无敌
        INVULNERABLE = 4,
        //浮空
        FLOAT = 5,
        //飞行
        FLY = 6,
        //倒地
        GROUND = 7,
        //起身
        GETUP = 8,
    }

    public class BuffStateConfig
    {
        private static BuffStateHandler baseBuffStateHandler = new BuffStateHandler();

        public List<ForbidType> forbids;
        public IBuffStateHandler handler;
        //相冲突的状态，如果状态A配置了冲突列表，表示，如果当前角色有状态A，那么只要添加buff中的状态存在refuseStates中，添加将失败
        //当拥有A状态的buff添加时，将会移除拥有refuseStates中的状态的buff
        public List<BuffStateType> refuseStates;
        //移除的状态，如果状态A配置了移除状态列表，表示，当拥有A状态的buff添加时，将会移除拥有removeStates中的状态的buff
        public List<BuffStateType> removeStates;


        private static Dictionary<BuffStateType, BuffStateConfig> m_dicConfigs = new Dictionary<BuffStateType, BuffStateConfig>()
        {
            { BuffStateType.NONE, new BuffStateConfig(){
                forbids = new List<ForbidType>(){},
                handler = baseBuffStateHandler,
                refuseStates = new List<BuffStateType>(),
                removeStates = new List<BuffStateType>(),
            }},
            { BuffStateType.STIFFNESS, new BuffStateConfig(){
                forbids = new List<ForbidType>(){ForbidType.Jump,ForbidType.InputMove, ForbidType.Ability, ForbidType.InputFace},
                handler = baseBuffStateHandler,
                refuseStates = new List<BuffStateType>(),
                removeStates = new List<BuffStateType>(),
            }},
            { BuffStateType.STUNNED, new BuffStateConfig(){
                forbids = new List<ForbidType>(){ForbidType.Jump,ForbidType.InputMove, ForbidType.Ability,ForbidType.InputFace},
                handler = baseBuffStateHandler,
                refuseStates = new List<BuffStateType>(),
                removeStates = new List<BuffStateType>(),
            }},
            { BuffStateType.FROZEN, new BuffStateConfig(){
                forbids = new List<ForbidType>(){ForbidType.Jump,ForbidType.InputMove, ForbidType.Ability, ForbidType.InputFace},
                handler = baseBuffStateHandler,
                refuseStates = new List<BuffStateType>(),
                removeStates = new List<BuffStateType>(),
            }},
            { BuffStateType.FLY, new BuffStateConfig(){
                forbids = new List<ForbidType>(){ForbidType.Jump,ForbidType.InputMove, ForbidType.Ability},
                handler = baseBuffStateHandler,
                refuseStates = new List<BuffStateType>(),
                removeStates = new List<BuffStateType>(),
            }},
            { BuffStateType.INVULNERABLE, new BuffStateConfig(){
                forbids = new List<ForbidType>(){},
                handler = baseBuffStateHandler,
                refuseStates = new List<BuffStateType>(),
                removeStates = new List<BuffStateType>(),
            }},
            { BuffStateType.FLOAT, new BuffStateConfig(){
                forbids = new List<ForbidType>(){ForbidType.Jump,ForbidType.InputMove, ForbidType.Ability, ForbidType.InputFace},
                handler = baseBuffStateHandler,
                refuseStates = new List<BuffStateType>(){BuffStateType.STIFFNESS, BuffStateType.GETUP},
                removeStates = new List<BuffStateType>(){BuffStateType.GROUND, BuffStateType.GETUP, BuffStateType.FLOAT},
            }},
            { BuffStateType.GROUND, new BuffStateConfig(){
                forbids = new List<ForbidType>(){ForbidType.Jump,ForbidType.InputMove, ForbidType.Ability, ForbidType.InputFace},
                handler = baseBuffStateHandler,
                refuseStates = new List<BuffStateType>(){BuffStateType.STIFFNESS, BuffStateType.GROUND},
                removeStates = new List<BuffStateType>(){BuffStateType.FLOAT},
            }},
            { BuffStateType.GETUP, new BuffStateConfig(){
                forbids = new List<ForbidType>(){ForbidType.Jump,ForbidType.InputMove, ForbidType.Ability, ForbidType.InputFace},
                handler = baseBuffStateHandler,
                refuseStates = new List<BuffStateType>(){BuffStateType.STIFFNESS, BuffStateType.GETUP},
                removeStates = new List<BuffStateType>(){BuffStateType.GROUND},
            }},
        };

        private static Dictionary<string, BuffStateType> InitStateTypes()
        {
            Type t = typeof(BuffStateType);
            Array array = Enum.GetValues(t);
            Dictionary<string, BuffStateType> dic = new Dictionary<string, BuffStateType>();
            foreach (var value in array)
            {
                var name = Enum.GetName(t, value);
                dic.Add(name, (BuffStateType)value);
            }

            return dic;
        }

        public static Dictionary<string, BuffStateType> m_dicStrToStateType = InitStateTypes();

        private static Dictionary<BuffStateType, string> InitStateToStrTypes()
        {
            Type t = typeof(BuffStateType);
            Array array = Enum.GetValues(t);
            Dictionary<BuffStateType, string> dic = new Dictionary<BuffStateType, string>();
            foreach (var value in array)
            {
                var name = Enum.GetName(t, value);
                dic.Add((BuffStateType)value, name);
            }

            return dic;
        }

        public static Dictionary<BuffStateType, string> m_dicStateToStrType = InitStateToStrTypes();


        private static Dictionary<BuffStateType, List<BuffStateType>> InitStateRefuseByList()
        {
            Dictionary<BuffStateType, List<BuffStateType>> dic = new Dictionary<BuffStateType, List<BuffStateType>>();
            foreach (var pair in m_dicConfigs)
            {
                var state = pair.Key;
                var refuseStates = pair.Value.refuseStates;
                foreach (var refuseState in refuseStates)
                {
                    List<BuffStateType> lst = null;
                    if (!dic.TryGetValue(refuseState, out lst))
                    {
                        lst = new List<BuffStateType>();
                        dic.Add(refuseState, lst);
                    }

                    if (!lst.Contains(state))
                    {
                        lst.Add(state);
                    }
                }
            }

            return dic;
        }
        private static Dictionary<BuffStateType, List<BuffStateType>> m_dicStateRefuseByList = InitStateRefuseByList();


        public static BuffStateConfig GetConfig(BuffStateType stateType)
        {
            return m_dicConfigs[stateType];
        }

        public static BuffStateType GetStateTypeByString(string str)
        {
            BuffStateType t;
            if (m_dicStrToStateType.TryGetValue(str, out t))
            {
                return t;
            }
            CLog.LogError("can not find str="+str+" BuffStateType");
            return BuffStateType.NONE;
        }

        public static string GetStringByStateType(BuffStateType type)
        {
            return m_dicStateToStrType[type];
        }

        public static List<BuffStateType> GetRefuseStateList(BuffStateType state)
        {
            List<BuffStateType> lst;
            if (m_dicStateRefuseByList.TryGetValue(state, out lst))
            {
                return lst;
            }

            return null;
        }
    }

    public class BuffStateComponent : DataComponent
    {
        public int stateIndex = 1;
        public Dictionary<int, BuffStateData> dicStates = new Dictionary<int, BuffStateData>();
        public HashSet<BuffStateType> stateSet = new HashSet<BuffStateType>();
    }

#if UNITY_EDITOR
//    [UnityEditor.CustomEditor(typeof(BuffStateComponent))]
//    public class BuffStateComponentInspector : UnityEditor.Editor
//    {
//        private UnityEditor.Editor cacheEditor = null;
//        private static List<BuffStateData> m_lstTemp = new List<BuffStateData>();
//
//        void OnEnable()
//        {
//            cacheEditor = UnityEditor.Editor.CreateEditor((UnityEngine.Object)(object)m_lstTemp);
//        }
//
//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();
//            var component = target as BuffStateComponent;
//            if (component != null)
//            {
//                foreach (var pair in component.dicStates)
//                {
//                    m_lstTemp.Add(pair.Value);
//                }
//            }
//            if(cacheEditor != null)
//                cacheEditor.OnInspectorGUI();
//            m_lstTemp.Clear();
//
//        }
//    }
#endif
}

