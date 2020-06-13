using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using Game;
using NodeEditor;
using UnityEditor;
using UnityEngine;

public static class CustomMenuItem
{
    static NETreeComposeType CreateNETreeComposeType(Type rootDataType, Type contextType,string typeCategory, string fileDir, string filePre, string fileExt, string desc)
    {
        //获取基类的handler（IBTContext）,core的基础扩展
        var arrCommonType = BTDataHandlerInitialize.GetDataTypeByContextType(typeof(IBTContext));
        //当前类型，例如skill下SkillBTContext扩展的类
        var arrType = BTDataHandlerInitialize.GetDataTypeByContextType(contextType);
        List<Type> lst = new List<Type>();
        if (rootDataType != null)
        {
            lst.Add(rootDataType);
        }
        lst.AddRange(arrCommonType);
        lst.AddRange(arrType);
        Dictionary<string, List<Type>> dicCategory = new Dictionary<string, List<Type>>();
        if (arrCommonType != null)
        {
            for (int i = 0; i < arrCommonType.Length; i++)
            {
                var curType = arrCommonType[i];
                var handler = BTDataHandlerInitialize.GetHandler(curType);
                var handlerBaseType = handler.GetType().BaseType;
                var friendName = handlerBaseType.Name;
                if (handlerBaseType.IsGenericType)
                {
                    friendName = friendName.Remove(friendName.IndexOf('`'));
                }
                var category = friendName;
                List<Type> curLst;
                dicCategory.TryGetValue(category, out curLst);
                if (curLst == null)
                {
                    curLst = new List<Type>();
                    dicCategory.Add(category, curLst);
                }

                curLst.Add(curType);
            }
        }

        if (arrType != null)
        {
            for (int i = 0; i < arrType.Length; i++)
            {
                var curType = arrType[i];
                if(curType == rootDataType)continue;
                var handler = BTDataHandlerInitialize.GetHandler(curType);
                var handlerBaseType = handler.GetType().BaseType;
                var friendName = handlerBaseType.Name;
                if (handlerBaseType.IsGenericType)
                {
                    friendName = friendName.Remove(friendName.IndexOf('`'));
                }
                var category = friendName;
                category = typeCategory + "/" + category;
                List<Type> curLst;
                dicCategory.TryGetValue(category, out curLst);
                if (curLst == null)
                {
                    curLst = new List<Type>();
                    dicCategory.Add(category, curLst);
                }

                curLst.Add(curType);
            }
        }

        return new NETreeComposeType(rootDataType, lst,dicCategory, fileDir, filePre, fileExt, desc);
    }

    private static NETreeComposeType[] GetConfig()
    {
        NETreeComposeType[] staticConfig = new NETreeComposeType[]
        {
            CreateNETreeComposeType(typeof(BTSkillRootData), typeof(SkillBTContext),"Skill", "Assets/ResourceEx/Config/SkillScript", "skill", "bytes", "技能脚本"),
            CreateNETreeComposeType(null, typeof(BuffBTContext),"Buff", "Assets/ResourceEx/Config/BuffScript", "buff", "bytes", "Buff脚本"),
            CreateNETreeComposeType(null, typeof(AIBTContext),"AI", "Assets/ResourceEx/Config/AIScript", "ai", "bytes", "AI脚本"),
        };
        return staticConfig;
    }

    [MenuItem("Tools/OpenSkillNETreeWindow &F1")]
    public static void OpenSkillNETreeWindow()
    {
        BTDataHandlerInitialize.InitializeInEditor(true);
        NETreeWindow.OpenWindow(GetConfig(), 0);
    }

    [MenuItem("Tools/OpenBuffNETreeWindow &F2")]
    public static void OpenBuffNETreeWindow()
    {
        BTDataHandlerInitialize.InitializeInEditor(true);
        NETreeWindow.OpenWindow(GetConfig(), 1);
    }

    [MenuItem("Tools/OpenAINETreeWindow &F3")]
    public static void OpenAINETreeWindow()
    {
        BTDataHandlerInitialize.InitializeInEditor(true);
        NETreeWindow.OpenWindow(GetConfig(), 2);
    }

}

