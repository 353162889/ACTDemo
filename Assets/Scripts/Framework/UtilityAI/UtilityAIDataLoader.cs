using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Framework
{

    class FieldOnlyResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var result = new List<MemberInfo>(objectType.GetFields(BindingFlags.Public | BindingFlags.Instance));
            return result;
        }
    }

    public class UtilityAIDataLoader
    {

        public static JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new FieldOnlyResolver()
        };

        private Dictionary<string, string> dicKeyToFilePath = new Dictionary<string, string>();
        private Dictionary<string, UtilityAIData> m_dicData = new Dictionary<string, UtilityAIData>();

        private MultiResourceObjectLoader m_resObjectLoader;
        private Action<string, UtilityAIData> m_progressAction;
        private Action m_finishAction;
        private bool m_bFinish = true;

        public UtilityAIData Get(string key)
        {
            UtilityAIData data = null;
            m_dicData.TryGetValue(key, out data);
            return data;
        }

        public bool IsFinish()
        {
            return m_bFinish;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicKeyToFilePath">{key:path}</param>
        /// <param name="onFinish"></param>
        public void LoadResCfgs(Dictionary<string, string> dicKeyToFilePath, Action onFinish)
        {
            Clear();
            var files = new List<string>();
            this.dicKeyToFilePath = dicKeyToFilePath;
            foreach (var pair in dicKeyToFilePath)
            {
                if (!files.Contains(pair.Value)) files.Add(pair.Value);
            }
            this.Load(files, null, onFinish);
        }

        private void Load(List<string> files, Action<string, UtilityAIData> onProgress = null, Action onFinish = null)
        {
            if (files.Count == 0)
            {
                if (onFinish != null) onFinish();
                return;
            }
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
                        var neData = ParseData(textAsset.text, files[i]);
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
            dicKeyToFilePath.Clear();
            m_dicData.Clear();
            m_finishAction = null;
            m_bFinish = true;
            if (m_resObjectLoader != null)
            {
                m_resObjectLoader.Clear();
            }
        }

        private void OnComplete(MultiResourceObjectLoader loader)
        {
            if (m_resObjectLoader != null)
            {
                m_resObjectLoader.Clear();
                m_resObjectLoader = null;
            }

            m_progressAction = null;
            m_bFinish = true;
            if (null != m_finishAction)
            {
                Action action = m_finishAction;
                m_finishAction = null;
                action();
            }
        }

        private void OnProgress(UnityEngine.Object obj, string path)
        {
            string data = ((TextAsset)obj).text;
            var utilityAiData = ParseData(data, path);
            if (null != m_progressAction)
            {
                var action = m_progressAction;
                action(path, utilityAiData);
            }
        }

       

        private UtilityAIData ParseData(string content, string path)
        {
            try
            {
                var utilityAIData = JsonConvert.DeserializeObject<UtilityAIData>(content, jsonSettings);
                foreach (var pair in dicKeyToFilePath)
                {
                    if (pair.Value == path)
                    {
                        m_dicData.Add(pair.Key, utilityAIData);
                    }
                }
//                m_dicData.Add(path, utilityAIData);
                return utilityAIData;
            }
            catch (Exception e)
            {
                CLog.LogError(e);
            }

            return null;
        }
    }
}