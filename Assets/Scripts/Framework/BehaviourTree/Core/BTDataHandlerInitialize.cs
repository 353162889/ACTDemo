using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework
{
    public class BTDataHandlerInitialize
    {
        //类型对实例索引，即类型对arrHandler中的index
        private static Dictionary<Type, int> dicTypeToIndex;
        //IBTDataHandler的实例对象列表
        private static IBTDataHandler[] arrHandler;
        //所有的IBTDataHandler类型列表
        private static Type[] arrBTDataHandlerDataType;
        //key为IBTContext类型,value为IBTDataHandler列表的字典
        private static Dictionary<Type, Type[]> dicContextTypeToTypeArray;
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
            inited = true;
            dicTypeToIndex = new Dictionary<Type, int>();
           
            List<IBTDataHandler> lstDataHandlers = new List<IBTDataHandler>();
            List<Type> lstDataHandlerDatas = new List<Type>();
            Dictionary<Type,List<Type>> dicContextTypeToDataTypeList = new Dictionary<Type, List<Type>>();
            var types = Assembly.GetAssembly(typeof(BTDataHandlerInitialize)).GetTypes();
            var handlerType = typeof(IBTDataHandler);
            var contextType = typeof(IBTContext);
            int index = -1;
            foreach (var type in types)
            {
                if (handlerType.IsAssignableFrom(type) && !type.IsAbstract)
                {
                    if (type.BaseType != null && type.BaseType.IsAbstract)
                    {
                        var baseType = type.BaseType;
                        var attrs = baseType.GetGenericArguments();
                        if (attrs.Length == 2 && contextType.IsAssignableFrom(attrs[0]))
                        {
                            var instance = Activator.CreateInstance(type);
                            index++;
                            lstDataHandlers.Add((IBTDataHandler)instance);
                            var curContextType = attrs[0];
                            var dataType = attrs[1];
                            List<Type> lst;
                            dicContextTypeToDataTypeList.TryGetValue(curContextType, out lst);
                            if (lst == null)
                            {
                                lst = new List<Type>();
                                dicContextTypeToDataTypeList.Add(curContextType, lst);
                            }
                            lst.Add(dataType);
                            lstDataHandlerDatas.Add(dataType);
                            dicTypeToIndex.Add(dataType, lstDataHandlers.Count - 1);
                        }
                    }
                    

                }
            }

            arrHandler = lstDataHandlers.ToArray();
            arrBTDataHandlerDataType = lstDataHandlerDatas.ToArray();
            dicContextTypeToTypeArray = new Dictionary<Type, Type[]>();
            foreach (var pair in dicContextTypeToDataTypeList)
            {
                dicContextTypeToTypeArray.Add(pair.Key, pair.Value.ToArray());   
            }
            CLog.Log("BTDataHandlerInitialize.Initialize");
        }

        public static IBTDataHandler GetHandler(int index)
        {
            if (index < 0 || index >= arrHandler.Length) return null;
            return arrHandler[index];
        }

        public static IBTDataHandler GetHandler(Type dataType)
        {
            int index = GetHandlerIndex(dataType);
            return GetHandler(index);
        }

        //通过数据类型获取处理类的索引
        public static int GetHandlerIndex(Type dataType)
        {
            int index = -1;
            dicTypeToIndex.TryGetValue(dataType, out index);
            return index;
        }

        public static Type[] GetAllDataType()
        {
            return arrBTDataHandlerDataType;
        }

        public static Type[] GetDataTypeByContextType(Type contextType)
        {
            Type[] arr;
            dicContextTypeToTypeArray.TryGetValue(contextType, out arr);
            return arr;
        }
    }
}