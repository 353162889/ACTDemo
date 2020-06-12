using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using Framework;
using UnityEditor.Compilation;
using UnityEditor.Timeline;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NodeEditor
{
    public class NETreeWindow : EditorWindow
    {
        
        private static float titleHeight = 20;
        private static float leftAreaWidth = 200;
        private static float rightAreaWidth = 200;

        public NECanvas canvas { get { return m_cCanvas; } }
        private NECanvas m_cCanvas;

        protected NETreeComposeType[] arrTreeComposeData;
        protected int m_nTreeComposeIndex = 0;

        private GUIStyle m_cToolBarBtnStyle;
        private GUIStyle m_cToolBarPopupStyle;

        private string[] m_arrComposeDesc;

        private NENode m_cRoot;
        private string m_sLoadPath;
        private NEData initNEData = null;

        private NENode m_cTimeLineEditorNode;
        public NENode timelineEditorNode
        {
            get { return m_cTimeLineEditorNode; }
        }

        private PlayableDirector m_cDirector;
        public PlayableDirector director
        {
            get { return m_cDirector; }
        }

        private GameObject timelinePrefab;

        public void SetTimelineEditorNode(NENode node)
        {
            if (node == null) return;
            if (m_cTimeLineEditorNode != null)
            {
                if (!EditorUtility.DisplayDialog("提示", "当前正在编辑Timeline节点，是否覆盖该节点？", "确定", "取消"))
                {
                    return;
                }
            }

            if (m_cDirector != null)
            {
                GameObject.DestroyImmediate(m_cDirector.gameObject);
                m_cDirector = null;
            }
            m_cTimeLineEditorNode = node;
            //创建playableDirector实例与timeline资源
            var go = new GameObject("NETimelineGameObject");
            m_cDirector = go.AddComponent<PlayableDirector>();
            var neData = GetNEDataByNENode(m_cTimeLineEditorNode);
            if (timelinePrefab == null)
            {
                string playerPath = "Assets/ResourceEx/Prefab/MainPlayer.prefab";
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(playerPath);
                timelinePrefab = prefab;
            }
            if (!TimeLineUtility.ConvertNEDataToTimeLine(m_cDirector, neData, timelinePrefab))
            {
                GameObject.DestroyImmediate(m_cDirector.gameObject);
                m_cDirector = null;
                EditorUtility.DisplayDialog("提示", "创建timeline资源失败!详细请看Debug", "确定");
                return;
            }
            AssetDatabase.CreateAsset(m_cDirector.playableAsset, "Assets/Temp/Timeline_temp.playable");
            AssetDatabase.SaveAssets();
            Selection.activeGameObject = go;
        }

//        [InitializeOnLoadMethod]
//        static public void Init()
//        {
//            CompilationPipeline.compilationStarted += OnStartCompile;
//            CompilationPipeline.compilationFinished += OnEndCompile;
//        }
//
//        private static NEData cacheData = null;
//        private static string cachePath = null;
//
//        static void OnStartCompile(object obj)
//        {
//            UnityEngine.Object[] objectsOfTypeAll = Resources.FindObjectsOfTypeAll(typeof(NETreeWindow));
//            EditorWindow editorWindow = objectsOfTypeAll.Length != 0 ? objectsOfTypeAll[0] as EditorWindow : (EditorWindow)null;
//            if (!(bool)((UnityEngine.Object)editorWindow))
//                return;
//            var window = EditorWindow.GetWindow<NETreeWindow>();
//            cacheData = window.GetCurrentTreeNEData();
//            cachePath = window.m_sLoadPath;
//            window.Close();
//            Debug.Log("start compile:"+obj.ToString());
//        }
//
//        static void OnEndCompile(object obj)
//        {
//            if (cacheData != null)
//            {
//                Debug.Log("end compile1:" + obj.ToString());
//                EditorWindow.GetWindow<NETreeWindow>().m_sLoadPath = cachePath;
//                EditorWindow.GetWindow<NETreeWindow>().initNEData = cacheData;
//                CustomMenuItem.OpenTestNETreeWindow();
//                cacheData = null;
//                cachePath = null;
//            }
//            Debug.Log("end compile:" + obj.ToString());  
//        }

        static public bool IsOpen()
        {
            UnityEngine.Object[] objectsOfTypeAll = Resources.FindObjectsOfTypeAll(typeof(NETreeWindow));
            EditorWindow editorWindow = objectsOfTypeAll.Length != 0 ? objectsOfTypeAll[0] as EditorWindow : (EditorWindow)null;
            if (!(bool)((UnityEngine.Object)editorWindow))
                return false;
            return true;
        }

        static public Dictionary<string, List<Type>> GetCurrentCanvasEditorTypes()
        {
            if (!IsOpen()) return null;
            var window = EditorWindow.GetWindow<NETreeWindow>();
            if (window.canvas == null) return null;
            return window.canvas.dicNodeDataType;
        }

        static public void OpenWindow(NETreeComposeType[] arrTreeComposeData, int index)
        {
            Debug.Log("OpenWindow");
            var window = EditorWindow.GetWindow<NETreeWindow>(); 
            window.arrTreeComposeData = arrTreeComposeData;
            window.titleContent = new GUIContent("NETreeWindow");
            var position = window.position;
            position.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height).center;
            window.position = position;
            window.Show();
            window.FocusCanvasCenterPosition();
            if (index > -1 && index < window.arrTreeComposeData.Length)
            {
                window.m_nTreeComposeIndex = index;
            }
            window.OnEnable();
            window.Focus();
        }

        void OnEnable()
        {
            m_cToolBarBtnStyle = null;
            m_cToolBarPopupStyle = null;
            timelinePrefab = null;
            if (arrTreeComposeData != null)
            {
                m_arrComposeDesc = new string[arrTreeComposeData.Length];
                for (int i = 0; i < arrTreeComposeData.Length; i++)
                {
                    m_arrComposeDesc[i] = arrTreeComposeData[i].desc + "编辑";
                }

                Load(arrTreeComposeData[m_nTreeComposeIndex]);
            }

            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (director != null && TimelineEditor.inspectedDirector == director)
            {
                var timelineAsset = (TimelineAsset) director.playableAsset;
                if (timelineAsset != null)
                {
                    foreach (var track in timelineAsset.GetRootTracks())
                    {
                        if (track is NEAbstractPlayableTrack)
                        {
                            ((NEAbstractPlayableTrack)track).Update();
                        }
                    }
                }
            }
        }

        private void Load(NETreeComposeType composeType)
        {
            m_sLoadPath = null;
            //移除根节点
            if (m_cCanvas != null) m_cCanvas.Dispose();
            m_cCanvas = new NECanvas(composeType.lstNodeDataType, composeType.dicCategory, CreateNENodeDataByDataType, CopyNENodeData);
            CreateTreeByTreeData(initNEData);
            initNEData = null;
        }

        public void FocusCanvasCenterPosition()
        {
            if (m_cCanvas != null)
            {
                float canvasWidth = position.width - leftAreaWidth - rightAreaWidth;
                float canvasHeight = position.height - titleHeight;
                Vector2 firstScrollPos = new Vector2((m_cCanvas.scrollViewRect.width - canvasWidth) / 2, (m_cCanvas.scrollViewRect.height - 100) / 2);
                m_cCanvas.scrollPos = firstScrollPos;
            }
        }

        public void SetCanvasCenter()
        {
            if (m_cCanvas != null)
            {
                float canvasWidth = position.width - leftAreaWidth - rightAreaWidth;
                float canvasHeight = position.height - titleHeight;
                Vector2 firstScrollPos = new Vector2((m_cCanvas.scrollViewRect.width) / 2, (m_cCanvas.scrollViewRect.height) / 2);
               m_cCanvas.SetCenter(firstScrollPos);
            }
        }

        private NENode CreateTreeByTreeData(NEData neData)
        {
            if (m_cCanvas != null) m_cCanvas.Clear();
            NENode node = null;
            if (neData == null)
            {
                var composeData = arrTreeComposeData[m_nTreeComposeIndex];
                Vector2 center = m_cCanvas.scrollViewRect.center;
                node = m_cCanvas.CreateNode(null, null);
            }
            else
            {
                node = m_cCanvas.CreateNENode(neData, null);
            }
            m_cRoot = node;
            SetCanvasCenter();
            m_cCanvas.RefreshPosition();
            FocusCanvasCenterPosition();
            return node;
        }

        private object CreateNENodeDataByDataType(Type neNodeDataType)
        {
            return Activator.CreateInstance(neNodeDataType);
        }
       
        private NEData CopyNENodeData(NENode neNode)
        {
            NEData neData = GetNEDataByNENode(neNode);
            byte[] buff = NEUtil.SerializerObjectToBuff(neData, arrTreeComposeData[m_nTreeComposeIndex].lstNodeDataType.ToArray());
            if (buff != null)
            {
                neData = NEUtil.DeSerializerObjectFromBuff(buff, typeof(NEData), arrTreeComposeData[m_nTreeComposeIndex].lstNodeDataType.ToArray()) as NEData;
                return neData;
            }
            return null;
        }


        void OnDisable()
        {
            if (m_cCanvas != null)
            {
                m_cCanvas.Dispose();
            }

            m_cCanvas = null;
            m_cToolBarBtnStyle = null;
            m_cToolBarPopupStyle = null;
            initNEData = null;
            m_cTimeLineEditorNode = null;
            if (m_cDirector != null)
            {
                GameObject.DestroyImmediate(m_cDirector.gameObject);
            }
            m_cDirector = null;
            EditorApplication.update -= OnEditorUpdate;
        }

        private int toolBarIndex = 0;
        private Vector3 leftScrollPos;
        private Vector3 rightScrollPos;
        void OnGUI()
        {
            if (m_cToolBarBtnStyle == null)
            {
                m_cToolBarBtnStyle = new GUIStyle((GUIStyle)"toolbarbutton");
            }

            if (m_cToolBarPopupStyle == null)
            {
                m_cToolBarPopupStyle = new GUIStyle((GUIStyle)"ToolbarPopup");
            }

            if (m_cCanvas == null) return;

            float centerAreaWidth = position.width - leftAreaWidth - rightAreaWidth;
            if (centerAreaWidth < 0) centerAreaWidth = 0;

            float oldWidth;
            //画布整体描述区域
            Rect leftArea = new Rect(0, titleHeight, leftAreaWidth, position.height - titleHeight);
            GUILayout.BeginArea(leftArea);
            GUILayout.Label("总描述", m_cToolBarBtnStyle, GUILayout.Width(leftArea.width));
            leftScrollPos = GUILayout.BeginScrollView(leftScrollPos, false, true);
            if(m_cRoot != null)
            {
                oldWidth= EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 50;
                NEDataProperties.Draw(m_cRoot.dataProperty, GUILayout.Width(leftArea.width - 50));
                EditorGUIUtility.labelWidth = oldWidth;
            }

            oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 90;
            timelinePrefab = (GameObject)EditorGUILayout.ObjectField("TimelinePrefab", timelinePrefab, typeof(GameObject), true);
            EditorGUIUtility.labelWidth = oldWidth;

            if (m_cTimeLineEditorNode != null)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.Space(5);
                m_cDirector = (PlayableDirector)EditorGUILayout.ObjectField(m_cDirector, typeof(PlayableDirector), true);
                if (m_cDirector != null && m_cTimeLineEditorNode != null)
                {
                    if (GUILayout.Button("保存"))
                    {
                        var neData = TimeLineUtility.ConvertTimeLineToNEData(m_cDirector);
                        if (neData != null)
                        {
                            var parent = m_cTimeLineEditorNode.parent;
                            m_cCanvas.RemoveNode(m_cTimeLineEditorNode);
                            m_cTimeLineEditorNode = m_cCanvas.CreateNENode(neData, parent);
                            m_cCanvas.RefreshPosition();
                        }
                        Debug.Log("保存");
                    }
                    if (GUILayout.Button("保存并删除"))
                    {
                        var neData = TimeLineUtility.ConvertTimeLineToNEData(m_cDirector);
                        if (neData != null)
                        {
                            var parent = m_cTimeLineEditorNode.parent;
                            m_cCanvas.RemoveNode(m_cTimeLineEditorNode);
                            m_cTimeLineEditorNode = m_cCanvas.CreateNENode(neData, parent);
                            m_cCanvas.RefreshPosition();
                        }
                        GameObject.DestroyImmediate(m_cDirector.gameObject);
                        m_cDirector = null;
                        m_cTimeLineEditorNode = null;
                        Debug.Log("保存并删除");
                    }
                }

                EditorGUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            //画布区域
            Rect centerArea = new Rect(leftArea.width, titleHeight, centerAreaWidth, position.height - titleHeight);
            GUILayout.BeginArea(centerArea);
            m_cCanvas.Draw(centerArea);
            GUILayout.EndArea();

            //单个节点描述区域
            Rect rightArea = new Rect(leftArea.width + centerAreaWidth, titleHeight, rightAreaWidth, position.height - titleHeight);
            GUILayout.BeginArea(rightArea);
            GUILayout.Label("节点描述", m_cToolBarBtnStyle, GUILayout.Width(rightArea.width));
            rightScrollPos = GUILayout.BeginScrollView(rightScrollPos, false, true);
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            if (m_cCanvas.selectNode != null && m_cCanvas.selectNode.dataProperty != null)
            {
                if (!string.IsNullOrEmpty(m_cCanvas.selectNode.desc))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("节点描述:");
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    bool oldWordWrap = EditorStyles.textArea.wordWrap;
                    EditorStyles.textArea.wordWrap = true;
                    GUILayout.TextArea(m_cCanvas.selectNode.desc, GUILayout.Width(rightArea.width - 40), GUILayout.Height(60));
                    EditorStyles.textArea.wordWrap = oldWordWrap;
                    EditorGUILayout.EndHorizontal();
                }
                for (int i = 0; i < m_cCanvas.selectNode.arrBtnAttr.Length; i++)
                {
                    NEPropertyBtnAttribute attr = m_cCanvas.selectNode.arrBtnAttr[i];
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(attr.name))
                    {
                        attr.ExecuteBtn(null, m_cCanvas.selectNode.node);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUIUtility.labelWidth = 50;
                NEDataProperties.Draw(m_cCanvas.selectNode.dataProperty, GUILayout.Width(rightArea.width - 50));
            }
            EditorGUIUtility.labelWidth = oldLabelWidth;
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            //标题区域
            Rect titleRect = new Rect(0, 0, position.width, titleHeight);
            m_cToolBarBtnStyle.fixedHeight = titleRect.height;
            m_cToolBarPopupStyle.fixedHeight = titleRect.height;
            GUILayout.BeginArea(titleRect);
            //GUILayout.Label("", tt,GUILayout.Width(50),GUILayout.Height(20));
            GUILayout.BeginHorizontal();
            GUILayout.Label("", m_cToolBarBtnStyle, GUILayout.Width(10));
            int oldTreeComposeIndex = m_nTreeComposeIndex;
            m_nTreeComposeIndex = EditorGUILayout.Popup(m_nTreeComposeIndex, m_arrComposeDesc, m_cToolBarPopupStyle, GUILayout.Width(100));
            if (oldTreeComposeIndex != m_nTreeComposeIndex)
            {
                Load(arrTreeComposeData[m_nTreeComposeIndex]);
            }
            GUILayout.Label("", m_cToolBarBtnStyle, GUILayout.Width(position.width - 10 - 100 - 50 - 50 - 50 - 10));
            if (GUILayout.Button("创建", m_cToolBarBtnStyle, GUILayout.Width(50))) { CreateTreeByTreeData(null); }
            if (GUILayout.Button("加载", m_cToolBarBtnStyle, GUILayout.Width(50))) { LoadTreeByTreeData(); }
            if (GUILayout.Button("保存", m_cToolBarBtnStyle, GUILayout.Width(50))) { SaveTreeToTreeData(); }
            GUILayout.Label("", m_cToolBarBtnStyle, GUILayout.Width(10));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            if (GUI.changed) { Repaint(); }
        }

        private void LoadTreeByTreeData()
        {
            if (m_nTreeComposeIndex < 0 || m_nTreeComposeIndex > arrTreeComposeData.Length)
            {
                Debug.Log("需要选择树的类型");
                return;
            }
            var composeData = arrTreeComposeData[m_nTreeComposeIndex];
            string dir = composeData.fileDir;
            if (dir.StartsWith("Assets")) dir = dir.Replace("Assets", "");
            string path = EditorUtility.OpenFilePanel("加载数据", Application.dataPath + dir, composeData.fileExt);
            if (path.Length != 0)
            {
                path = path.Replace("\\", "/");
                for (int i = 0; i < arrTreeComposeData.Length; i++)
                {
                    if (path.Contains(arrTreeComposeData[i].fileDir))
                    {
                        m_nTreeComposeIndex = i;
                        composeData = arrTreeComposeData[m_nTreeComposeIndex];
                        break;
                    }
                }
                //通过前后缀确定当前数据是哪种类型,需要先切换到当前类型，在加载数据，否则数据有可能不对
                NEData neData = NEUtil.DeSerializerObject(path, typeof(NEData), composeData.lstNodeDataType.ToArray()) as NEData;
                m_sLoadPath = path;
                CreateTreeByTreeData(neData);
            }
        }

        private void SaveTreeToTreeData()
        {
            if (m_nTreeComposeIndex < 0 || m_nTreeComposeIndex > arrTreeComposeData.Length)
            {
                Debug.Log("需要选择树的类型");
                return;
            }
            var composeData = arrTreeComposeData[m_nTreeComposeIndex];
            NEData data = GetNEDataByNENode(m_cRoot);
            if (data == null)
            {
                Debug.Log("没有树数据");
                return;
            }
            string dir = composeData.fileDir;
            if (dir.StartsWith("Assets")) dir = dir.Replace("Assets", "");
            string defaultName = "";
            if (!string.IsNullOrEmpty(m_sLoadPath))
            {
                m_sLoadPath = m_sLoadPath.Replace("\\", "/");
                int index = m_sLoadPath.LastIndexOf("/");
                defaultName = m_sLoadPath.Substring(index + 1);
                defaultName = defaultName.Substring(0,defaultName.LastIndexOf("."));
            }
            //else
            //{
            //    Type type = data.data.GetType();
            //    FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            //    foreach (var item in fields)
            //    {
            //        if (item.GetCustomAttributes(typeof(NEPropertyKeyAttribute), true).Length > 0)
            //        {
            //            object key = item.GetValue(data.data);
            //            defaultName = key.ToString();
            //            break;
            //        }
            //    }
            //}
            string path = EditorUtility.SaveFilePanel("保存数据", Application.dataPath + dir, defaultName, composeData.fileExt);
            if (path.Length != 0)
            {
                NEUtil.SerializerObject(path, data, composeData.lstNodeDataType.ToArray());
            }
            AssetDatabase.Refresh();
        }

        private void DebugNEData(NEData neData)
        {
            if (neData.lstChild != null)
            {
                for (int i = 0; i < neData.lstChild.Count; i++)
                {
                    DebugNEData(neData.lstChild[i]);
                }
            }
        }

        private NEData GetNEDataByNENode(NENode neNode)
        {
            if (neNode == null) return null;
            var lstConnection = m_cCanvas.lstConnection;
            List<NENode> handNodes = new List<NENode>();
            NEData neData = GetNodeNEData(neNode, lstConnection, handNodes);
            return neData;
        }

        private NEData GetNodeNEData(NENode node, List<NEConnection> lst, List<NENode> handNodes)
        {
            if (handNodes.Contains(node))
            {
                Debug.LogError("树的连线进入死循环，节点=" + node.node.GetType());
                return null;
            }
            handNodes.Add(node);

            NEData neData = new NEData();
            neData.data = node.node;

            List<NENode> lstSubNode = new List<NENode>();
            for (int i = 0; i < lst.Count; i++)
            {
                NEConnection connection = lst[i];
                if (connection.outPoint.node == node)
                {
                    NENode childNode = connection.inPoint.node;
                    lstSubNode.Add(childNode);
                }
            }
            lstSubNode.Sort(NodeSort);
            for (int i = 0; i < lstSubNode.Count; i++)
            {
                NENode childNode = lstSubNode[i];
                NEData childNEData = GetNodeNEData(childNode, lst, handNodes);
                if (childNEData == null) continue;
                if (neData.lstChild == null) neData.lstChild = new List<NEData>();
                neData.lstChild.Add(childNEData);
            }

            if (neData.lstChild == null) neData.lstChild = new List<NEData>();

            if (neData.data is BTTimeLineData)
            {
                neData.lstChild.Sort(TimeLineNodeSort);
            }

            return neData;
        }

        //按照位置排序
        private int NodeSort(NENode a, NENode b)
        {
            int res = 0;
            if (a.rect.center.x - b.rect.center.x > 0) res = 1;
            else if (a.rect.center.x - b.rect.center.x < 0) res = -1;
            return res;
        }

        private int TimeLineNodeSort(NEData a, NEData b)
        {
            var aTimeData = a.data as IBTTimeLineData;
            var bTimeData = b.data as IBTTimeLineData;

            var aTime = aTimeData == null ? 0 : aTimeData.time;
            var bTime = bTimeData == null ? 0 : bTimeData.time;
            if (aTime > bTime) return 1;
            else if (aTime < bTime) return -1;
            else
            {
                return 0;
            }
        }
    }

}