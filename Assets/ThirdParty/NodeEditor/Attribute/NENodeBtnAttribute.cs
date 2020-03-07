using System;
using System.Reflection;
using UnityEngine;

namespace NodeEditor
{
    public class NENodeBtnAttribute : Attribute
    {
        public string name { get; private set; }

        public string className { get; private set; }
        /// <summary>
        /// 反射调用方法，参数是当前字段的fieldInfo,以及整个结构的objData
        /// </summary>
        public string classStaticFunc { get; private set; }
        public NENodeBtnAttribute(string name, string className, string classStaticFunc)
        {
            this.name = name;
            this.className = className;
            this.classStaticFunc = classStaticFunc;
        }

        public void ExecuteBtn(System.Object obj)
        {
            //have to traverse all assemblies to get the Type. In Assembly-CSharp-Editor you can not Get the Type
            Type t = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type tt = asm.GetType(className);
                if (tt != null)
                {
                    t = tt;
                    break;
                }
            }
            if (t == null)
            {
                Debug.LogError("在所有程序集中找不到类名=" + className + "的类");
                return;
            }
            var method = t.GetMethod(this.classStaticFunc, BindingFlags.Static | BindingFlags.Public);
            if (method == null)
            {
                Debug.LogError("在类名=" + className + "找不到方法=" + this.classStaticFunc);
                return;
            }
            method.Invoke(null, new object[] { obj });
        }
    }
}