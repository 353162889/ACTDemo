using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Framework;
using Game;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class UtilityDataToSerializeProperty : ScriptableObject
{
    [SerializeReference]
    public object data;

    public SerializedObject serializedObject;

    public static SerializedProperty GetSerializeProperty(object data)
    {
        var propertyObj = ScriptableObject.CreateInstance<UtilityDataToSerializeProperty>();
        propertyObj.data = data;
        propertyObj.serializedObject = new SerializedObject(propertyObj);
        return propertyObj.serializedObject.FindProperty("data");
    }
}

public static class VisualElementExtension
{
    public static T GetElementInParent<T>(this VisualElement element) where T : VisualElement
    {
        VisualElement t = element.parent;
        while (t != null)
        {
            if (t is T)
            {
                return (T)t;
            }
            t = t.parent;
        }
        return null;
    }

    public static T GetFirstLayerChildElement<T>(this VisualElement element) where T : VisualElement
    {
        for (int i = 0; i < element.childCount; i++)
        {
            if (element[i] is T)
            {
                return (T)element[i];
            }
        }

        return null;
    }
}

public class UtilityAIWindow : EditorWindow
{
    private string saveDir = "Assets/ResourceEx/Config/UtilityAIScript";
    private VisualElement rootContainer;
    private UtilityNodeItem rootItem;
    private string loadPath = "";
    private List<List<UtilityDebugInfo>> lstDebugInfos = new List<List<UtilityDebugInfo>>();

    private Dictionary<object, UtilityNodeItem> mDataToItem = new Dictionary<object, UtilityNodeItem>();
    private bool isEditor;
    public void OnEnable()
    {
        UtilityAIInitialize.InitializeInEditor(true);
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UtilityAIEditor/UtilityAIWindow.uxml");
        VisualElement windowXML = visualTree.CloneTree();
        root.Add(windowXML);

        var btnCreate = windowXML.Q<Button>("btnCreate");
        btnCreate.clicked += OnCreate;
        var btnSave = windowXML.Q<Button>("btnSave");
        btnSave.clicked += OnSave;
        var btnLoad = windowXML.Q<Button>("btnLoad");
        btnLoad.clicked += OnLoad;
        var btnApply = windowXML.Q<Button>("btnApply");
        btnApply.clicked += BtnApplyOnClicked;

        var scrollContent = windowXML.Q<ScrollView>("scrollContent");
        rootContainer = scrollContent.contentContainer;
        var scrollDetail = windowXML.Q<ScrollView>("scrollDetail");

        UtilityDebugEventDispatcher.Instance.RemoveEvent(UtilityDebugEvent.SendDebugInfo, OnDebug);
        UtilityDebugEventDispatcher.Instance.AddEvent(UtilityDebugEvent.SendDebugInfo, OnDebug);
        UtilityDebugEventDispatcher.Instance.RemoveEvent(UtilityDebugEvent.SendUtilityAIData, OnStartView);
        UtilityDebugEventDispatcher.Instance.AddEvent(UtilityDebugEvent.SendUtilityAIData, OnStartView);
        lstDebugInfos.Clear();

        UpdateDataToItem();
        isEditor = false;
        EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
    }

    private void BtnApplyOnClicked()
    {
        var data = GetSerializeData();
        if (data == null) return;
        UtilityDebugEventDispatcher.Instance.Dispatch(UtilityDebugEvent.UpdateUtilityAIData, data);
    }

    void OnDisable()
    {
        UtilityDebugEventDispatcher.Instance.RemoveEvent(UtilityDebugEvent.SendDebugInfo, OnDebug);
        UtilityDebugEventDispatcher.Instance.RemoveEvent(UtilityDebugEvent.SendUtilityAIData, OnStartView);
        EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
    }

    private void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (isEditor)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                isEditor = false;
                rootContainer.Clear();
            }
        }
    }

    public void UpdateDataToItem()
    {
        Debug.Log("UpdateDataToItem");
        mDataToItem.Clear();
        rootContainer.Query<UtilityNodeItem>().ForEach(item =>
        {
            mDataToItem.Add(item.data, item);
        });
    }

    private void OnStartView(int key, object args)
    {
        var utilityData = (UtilityAIData)args;
        if (utilityData != null && (this.rootItem == null || utilityData != this.rootItem.data))
        {
            isEditor = true;
            var item = Create(utilityData);
            item.Deserializable();
        }
    }

    private void OnDebug(int key, object args)
    {
        var lst = (List<UtilityDebugInfo>) args;
        rootContainer.Query<UtilityNodeItem>().ForEach(item =>
        {
            item.dataContainer.parent.RemoveFromClassList("select_utility_item");
            item.dataContainer.parent.RemoveFromClassList("select_utility_action");
            item.SetDebugText("");
        });
        for (int i = 0; i < lst.Count; i++)
        {
            var debugInfo = lst[i];
            if (debugInfo.notifyType == UtilityNotifyType.Selector && debugInfo.msg == null)
            {
                continue;
            }
            UtilityNodeItem item;
            if (mDataToItem.TryGetValue(debugInfo.utilityData, out item))
            {
                if (debugInfo.notifyType == UtilityNotifyType.BeSelected)
                {
                    item.dataContainer.parent.AddToClassList("select_utility_action");
                }
                else
                {
                    item.dataContainer.parent.AddToClassList("select_utility_item");
                }
                
                if (debugInfo.msg != null)
                {
                    string debugText = "";
                    if (debugInfo.notifyType == UtilityNotifyType.Evaluator)
                    {
                        debugText = "[Debug]Score:" + debugInfo.msg.ToString();
                    }
                    else if (debugInfo.notifyType == UtilityNotifyType.Decision)
                    {
                        debugText = "[Debug]UtilityValue:" + debugInfo.msg.ToString();
                    }
                    item.SetDebugText(debugText);
                }
            }
        }
    }

    private void OnCreate()
    {
        Create(new UtilityAIData());
    }

    private UtilityAIDataItem Create(UtilityAIData data)
    {
        rootContainer.Clear();
        var item = new UtilityAIDataItem(data);
        rootItem = item;
        rootContainer.Add(item);
        return item;
    }

    private bool CheckName(string name)
    {
        var builder = rootContainer.Query<VisualElement>(name);
        var lst = builder.ToList();
        foreach (var visualElement in lst)
        {
            if (visualElement.childCount <= 0) return false;
        }

        return true;
    }

    private object GetSerializeData()
    {
        var utilityAIDataItem = rootContainer.Q<UtilityAIDataItem>();
        if (utilityAIDataItem == null)
        {
            EditorUtility.DisplayDialog("提示", "需要创建数据", "确定");
            return null;
        }
        //检测是否有些数据没有填充
        if (!CheckName("selectorContainer") ||
            !CheckName("combineContainer") ||
            !CheckName("decisionFactorContainer") ||
            !CheckName("evaluatorContainer") ||
            !CheckName("actionContainer")
        )
        {
            EditorUtility.DisplayDialog("提示", "部分数据对象未填充，请填充完毕后再保存", "确定");
            return null;
        }

        return utilityAIDataItem.Serializable();
    }

    private void OnSave()
    {
        var data = GetSerializeData();
        if (data == null) return;
        string dir = saveDir;
        if (dir.StartsWith("Assets")) dir = dir.Replace("Assets", "");
        string defaultName = "";
        if (!string.IsNullOrEmpty(loadPath))
        {
            loadPath = loadPath.Replace("\\", "/");
            int index = loadPath.LastIndexOf("/");
            defaultName = loadPath.Substring(index + 1);
            defaultName = defaultName.Substring(0, defaultName.LastIndexOf("."));
        }
        var path = EditorUtility.SaveFilePanel("保存文件", Application.dataPath + dir, defaultName, "bytes");
        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                var content = JsonConvert.SerializeObject(data, UtilityAIDataLoader.jsonSettings);
                Debug.Log(content);
                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    writer.Write(content);
                    writer.Flush();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("SerializerObject Erro:" + ex.ToString());
                return;
            }
            AssetDatabase.Refresh();
        }
    }

    private void OnLoad()
    {
        string dir = saveDir;
        if (dir.StartsWith("Assets")) dir = dir.Replace("Assets", "");
        var path = EditorUtility.OpenFilePanel("打开文件", Application.dataPath + dir, "bytes");
        if (!string.IsNullOrEmpty(path))
        {
            UtilityAIData data = null;
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    var content = reader.ReadToEnd();
                    Debug.Log(content);
                    data = JsonConvert.DeserializeObject<UtilityAIData>(content, UtilityAIDataLoader.jsonSettings);
                }
               
                this.loadPath = path;
                if (data != null)
                {
                    var item = Create(data);
                    item.Deserializable();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("DeSerializerObject Erro:" + e.ToString());
            }
        }

        void OnGUI()
        {
//            if (lstDebugInfos.Count > 0)
//            {
//                rootContainer.Query<UtilityNodeItem>().ForEach((item) =>
//                {
//
//                });
//            }
        }
    }
}