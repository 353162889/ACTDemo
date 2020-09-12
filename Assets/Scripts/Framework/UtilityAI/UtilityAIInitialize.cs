using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Framework
{
    public class UtilityAIInitialize
    {
        private static Dictionary<Type, Type> dicDataTypeToIUtilityType;
        private static Type[] dataTypes;
        private static bool inited = false;

#if UNITY_EDITOR
        public static void InitializeInEditor(bool forceInit)
        {
            if (forceInit) inited = false;
            Initialize();
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected static void Initialize()
        {
            if (inited) return;
            dicDataTypeToIUtilityType = new Dictionary<Type, Type>();
            var types = Assembly.GetAssembly(typeof(UtilityAIInitialize)).GetTypes();
            var utilityType = typeof(IUtility);
            var utilityDataType = typeof(IUtilityData);
            List<Type> lst = new List<Type>();
            foreach (var type in types)
            {
                if (utilityType.IsAssignableFrom(type) && !type.IsAbstract)
                {
                    if (type.BaseType != null && type.BaseType.IsAbstract)
                    {
                        var baseType = type.BaseType;
                        var attrs = baseType.GetGenericArguments();
                        foreach (var attr in attrs)
                        {
                            if (utilityDataType.IsAssignableFrom(attr))
                            {
                                dicDataTypeToIUtilityType.Add(attr, type);
                                lst.Add(attr);
                                break;
                            }
                        }
                    }
                }
            }
            
            dataTypes = lst.ToArray();

            inited = true;
        }

        public static bool GetUtilityType(Type dataType, out Type result)
        {
            return dicDataTypeToIUtilityType.TryGetValue(dataType, out result);
        }

        public static List<Type> GetUtilityDataTypesByInterface(Type interfaceType)
        {
            List<Type> lst = new List<Type>();
            foreach (var pair in dicDataTypeToIUtilityType)
            {
                if (interfaceType.IsAssignableFrom(pair.Key))
                {
                    lst.Add(pair.Key);
                }
            }

            return lst;
        }

        public static Type[] GetAllDataType()
        {
            return dataTypes;
        }
    }
}