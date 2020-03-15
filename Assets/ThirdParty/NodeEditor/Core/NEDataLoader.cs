﻿using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace NodeEditor
{
    public class NEDataLoader
    {
        private Dictionary<string, NEData> m_dicData = new Dictionary<string, NEData>();

        private MultiResourceObjectLoader m_resObjectLoader;
        private Action<string, NEData> m_progressAction;
        private Action m_finishAction;
        private Type[] m_arrParseTypes;
        private bool m_bFinish = true;

        public NEData Get(string key)
        {
            NEData data = null;
            m_dicData.TryGetValue(key, out data);
            return data;
        }

        public bool IsFinish()
        {
            return m_bFinish;
        }

        public void Load(List<string> files,Type[] arrParseTypes,Action<string, NEData> onProgress = null, Action onFinish = null)
        {
            if(files.Count == 0)
            {
                if (onFinish != null) onFinish();
                return;
            }
            m_arrParseTypes = arrParseTypes;
            m_progressAction = onProgress;
            m_finishAction = onFinish;
            m_bFinish = false;
            //如果是播放模式
            if (Application.isPlaying)
            {
                //下载对应的资源
                m_resObjectLoader = new MultiResourceObjectLoader();
                m_resObjectLoader.LoadList(files, true, OnComplete, OnProgress);
            }
#if UNITY_EDITOR
            else
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(files[i]);
                    if (textAsset != null)
                    {
                        var neData = ParseData(textAsset.bytes,files[i]);
                        if (null != m_progressAction)
                        {
                            var action = m_progressAction;
                            m_progressAction = null;
                            action(files[i], neData);
                        }
                    }
                }

                m_bFinish = true;
                if (null != m_finishAction)
                {
                    Action action = m_finishAction;
                    m_finishAction = null;
                    action();
                }
            }
#endif
        }

        public void Clear()
        {
            m_dicData.Clear();
            m_finishAction = null;
            m_bFinish = true;
            if (m_resObjectLoader != null)
            {
                m_resObjectLoader.Clear();
            }
            m_arrParseTypes = null;
        }

        private void OnComplete(MultiResourceObjectLoader loader)
        {
            if (m_resObjectLoader != null)
            {
                m_resObjectLoader.Clear();
                m_resObjectLoader = null;
            }

            m_progressAction = null;
            m_arrParseTypes = null;
            m_bFinish = true;
            if (null != m_finishAction)
            {
                Action action = m_finishAction;
                m_finishAction = null;
                action();
            }
        }

        private void OnProgress(UnityEngine.Object obj,string path)
        {
            byte[] data = ((TextAsset)obj).bytes;
            var neData = ParseData(data,path);
            if (null != m_progressAction)
            {
                var action = m_progressAction;
                action(path, neData);
            }
        }

        private NEData ParseData(byte[] bytesData,string path)
        {
            NEData neData = NEUtil.DeSerializerObjectFromBuff(bytesData, typeof(NEData), m_arrParseTypes) as NEData;
            m_dicData.Add(path, neData);
            return neData;
            //Type type = neData.data.GetType();
            //FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            //bool hasPropertyKey = false;
            //foreach (var item in fields)
            //{
            //    if (item.GetCustomAttributes(typeof(NEPropertyKeyAttribute), true).Length > 0)
            //    {
            //        object key = item.GetValue(neData.data);
            //        m_dicData.Add(key, neData);
            //        hasPropertyKey = true;
            //        break;
            //    }
            //}
            //if(!hasPropertyKey)
            //{
            //    CLog.LogError(neData.data.GetType()+" can not find NEPropertyKeyAttribute!");
            //}
        }

        
    }
}
