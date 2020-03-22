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
    }

    public class BuffStateConfig
    {
        private static BuffStateHandler baseBuffStateHandler = new BuffStateHandler();

        public List<ForbidType> forbids;
        public IBuffStateHandler handler;


        private static Dictionary<BuffStateType, BuffStateConfig> m_dicConfigs = new Dictionary<BuffStateType, BuffStateConfig>()
        {
            { BuffStateType.NONE, new BuffStateConfig(){
                forbids = new List<ForbidType>(){},
                handler = baseBuffStateHandler}},
            { BuffStateType.STIFFNESS, new BuffStateConfig(){
                forbids = new List<ForbidType>(){ForbidType.Jump,ForbidType.InputMove, ForbidType.Ability},
                handler = baseBuffStateHandler}},
            { BuffStateType.STUNNED, new BuffStateConfig(){
                forbids = new List<ForbidType>(){ForbidType.Jump,ForbidType.InputMove, ForbidType.Ability},
                handler = baseBuffStateHandler}},
            { BuffStateType.FROZEN, new BuffStateConfig(){
                forbids = new List<ForbidType>(){ForbidType.Jump,ForbidType.InputMove, ForbidType.Ability},
                handler = baseBuffStateHandler}},
            { BuffStateType.FLY, new BuffStateConfig(){
                forbids = new List<ForbidType>(){ForbidType.Jump,ForbidType.InputMove, ForbidType.Ability},
                handler = baseBuffStateHandler}},
            { BuffStateType.INVULNERABLE, new BuffStateConfig(){
                forbids = new List<ForbidType>(){},
                handler = baseBuffStateHandler}},
            { BuffStateType.FLOAT, new BuffStateConfig(){
                forbids = new List<ForbidType>(){},
                handler = baseBuffStateHandler}},
            { BuffStateType.FLY, new BuffStateConfig(){
                forbids = new List<ForbidType>(){},
                handler = baseBuffStateHandler}},
            
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
    }

    public class BuffStateComponent : DataComponent
    {
        public int stateIndex = 1;
        public Dictionary<int, BuffStateData> dicStates = new Dictionary<int, BuffStateData>();
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(BuffStateComponent))]
    public class BuffStateComponentInspector : UnityEditor.Editor
    {
        private UnityEditor.Editor cacheEditor = null;
        private static List<BuffStateData> m_lstTemp = new List<BuffStateData>();

        void OnEnable()
        {
            cacheEditor = UnityEditor.Editor.CreateEditor((UnityEngine.Object)(object)m_lstTemp);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var component = target as BuffStateComponent;
            if (component != null)
            {
                foreach (var pair in component.dicStates)
                {
                    m_lstTemp.Add(pair.Value);
                }
            }
            if(cacheEditor != null)
                cacheEditor.OnInspectorGUI();
            m_lstTemp.Clear();

        }
    }
#endif
}

