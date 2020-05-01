using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Framework;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    public class NECanvas
    {
        private Vector2 centerPos = Vector2.zero;
        private static Vector2 space = new Vector2(20, 30);
        public Vector2 scrollPos { get; set; }
        private Rect m_sPosition;
        public Rect scrollViewRect = new Rect(0, 0, 10000, 10000);
        private List<NENode> m_lstNode = new List<NENode>();
        private NENode m_cRoot;
        private List<NEConnection> m_lstConnection = new List<NEConnection>();
        public List<NEConnection> lstConnection { get { return m_lstConnection; } }
        private NENode m_cSelectedNode;
        public NENode selectNode { get { return m_cSelectedNode; } }
        private NENode m_cDragNode;
        private NENodePoint m_cInNodePoint;
        private NENodePoint m_cOutNodePoint;
        //种类名称对于节点类型列表
        private Dictionary<string, List<Type>> m_dicNodeDataType;
        public Dictionary<string,List<Type>> dicNodeDataType
        {
            get { return m_dicNodeDataType; }
        }
        private Func<Type, object> m_fCreateNodeDataFunc;
        private NENode m_cCopyObj;
        private Func<NENode, NEData> m_cCopyFunc;

        public NECanvas(List<Type> lstNodeDataType, Dictionary<string, List<Type>> extCategoryType, Func<Type,object> createNodeDataDataFunc, Func<NENode, NEData> copyFunc = null)
        {
            m_fCreateNodeDataFunc = createNodeDataDataFunc;
            m_dicNodeDataType = new Dictionary<string, List<Type>>();
            foreach (var keyValue in extCategoryType)
            {
                m_dicNodeDataType.Add(keyValue.Key,keyValue.Value);
            }
            if (lstNodeDataType != null)
            {
                for (int i = 0; i < lstNodeDataType.Count; i++)
                {
                    List<Type> lst = null;
                    var attributes = lstNodeDataType[i].GetCustomAttributes(typeof(NENodeCategoryAttribute), true);
                    if(attributes.Length > 0)
                    {
                        string category = ((NENodeCategoryAttribute)attributes[0]).category;
                        if(!m_dicNodeDataType.TryGetValue(category, out lst))
                        {
                            lst = new List<Type>();
                            m_dicNodeDataType.Add(category, lst);
                        }
                    }
                    else
                    {
//                        if(!m_dicNodeDataType.TryGetValue("", out lst))
//                        {
//                            lst = new List<Type>();
//                            m_dicNodeDataType.Add("", lst);
//                        }
                    }

                    if (lst != null)
                    {
                        lst.Add(lstNodeDataType[i]);
                    }
                }
            }
            m_cCopyFunc = copyFunc;
            scrollPos = new Vector2(scrollViewRect.width / 2f, scrollViewRect.height / 2f);
            m_cSelectedNode = null;
            m_cDragNode = null;
            m_cInNodePoint = null;
            m_cOutNodePoint = null;
            m_cCopyObj = null;
        }

        public void SetCenter(Vector2 center)
        {
            this.centerPos = center;
        }

        public void RefreshPosition()
        {
            if (m_cRoot == null) return;
            RefreshPlaceHolderWidth(m_cRoot);
            RefreshStartPlaceHolderX(m_cRoot, centerPos.x - m_cRoot.placeHolderWidth / 2);
            RefreshNodePosition(m_cRoot);
        }

        private float RefreshPlaceHolderWidth(NENode node)
        {
            float width = 0;
            if (node.children.Count > 0)
            {
                for (int i = 0; i < node.children.Count; i++)
                {
                    width = width + RefreshPlaceHolderWidth(node.children[i]);
                }
            }
            else
            {
                width = node.rect.width + space.x;
                if (node.parent != null)
                {
                    if (width < node.parent.rect.width)
                    {
                        width = node.parent.rect.width + space.x;
                    }
                }
            }

            node.placeHolderWidth = width;
            return width;
        }

        private void RefreshStartPlaceHolderX(NENode node, float startPlaceHolderX)
        {
            if (node.parent == null)
            {
                node.startPlaceHolderX = startPlaceHolderX;
            }
            else
            {
                var index = node.parent.children.IndexOf(node);
                float x = startPlaceHolderX;
                if (index > 0)
                {
                    var preNode = node.parent.children[index - 1];
                    x = preNode.startPlaceHolderX + preNode.placeHolderWidth;
                }

                node.startPlaceHolderX = x;
            }
            for (int i = 0; i < node.children.Count; i++)
            {
                RefreshStartPlaceHolderX(node.children[i], node.startPlaceHolderX);
            }
        }

        private void RefreshNodePosition(NENode node)
        {
            Vector2 pos;
            float startX;
            if (node.parent == null)
            {
                pos = centerPos;
                startX = centerPos.x - node.placeHolderWidth / 2;
            }
            else
            {
                NENode parent = node.parent;
                startX = parent.startPlaceHolderX;
                float y = parent.rect.center.y + parent.rect.height / 2 + node.rect.height / 2 + space.y;
                float x = startX;
                int index = parent.children.IndexOf(node);
                for (int i = 0; i < index; i++)
                {
                    x += parent.children[i].placeHolderWidth;
                }


                x += node.placeHolderWidth / 2;
                pos = new Vector2(x, y);
            }

            node.rect.center = pos;

            for (int i = 0; i < node.children.Count; i++)
            {
                RefreshNodePosition(node.children[i]);
            }
        }

        public void Draw(Rect position)
        {
            m_sPosition = position;
            Rect rect = new Rect(0, 0, position.width, position.height);
            scrollPos = GUI.BeginScrollView(rect, scrollPos, scrollViewRect, true, true);
            DrawGrid();
            DrawNodes();
            DrawConnections();
            DrawNodePoint(Event.current);
            HandleEvent(Event.current);
            GUI.EndScrollView();
        }

        public NENode CreateNENode(NEData neData, NENode parent)
        {
            NENode parentNode = CreateNode(parent, neData.data);
            if (neData.lstChild != null)
            {
                for (int i = 0; i < neData.lstChild.Count; i++)
                {
                    CreateNENode(neData.lstChild[i], parentNode);
                }
            }
            return parentNode;
        }

        public NENode CreateNode(NENode parent, object data)
        {
            if (parent != null && parent.node is BTTimeLineData && !(data is IBTTimeLineData))
            {
                BTTimeDecoratorData decoratorData = new BTTimeDecoratorData();
                parent = CreateNode(parent, decoratorData);
            }
            NENode node = new NENode(centerPos, data);
            if (parent != null)
            {
                parent.children.Add(node);
                node.SetParent(parent);
                CreateConnect(parent, node);
            }
            else
            {
                m_cRoot = node;
            }
            m_lstNode.Add(node);
            return node;
        }

        public NEConnection CreateConnect(NENode beginNode,NENode endNode)
        {
            if (beginNode.outPoint != null && endNode.inPoint != null)
            {
                NEConnection connection = new NEConnection(endNode.inPoint,beginNode.outPoint);
                m_lstConnection.Add(connection);
                return connection;
            }
            return null;
        }

        private void DrawNodes()
        {
            for (int i = m_lstNode.Count - 1; i > -1; i--)
            {
                m_lstNode[i].Draw(OnClickNodeRemove, OnClickNodePoint);
            }
        }

        private void DrawConnections()
        {
            for (int i = m_lstConnection.Count - 1; i > -1; i--)
            {
                m_lstConnection[i].Draw();
            }
        }

        private void DrawNodePoint(Event e)
        {
            if (m_cInNodePoint != null && m_cOutNodePoint == null)
            {
                Rect rect = new Rect(scrollPos.x, scrollPos.y, m_sPosition.width, m_sPosition.height);
                bool isInWindow = rect.Contains(e.mousePosition);
                if (isInWindow)
                {
                    Handles.DrawBezier(m_cInNodePoint.rect.center, e.mousePosition,
                        m_cInNodePoint.rect.center + Vector2.down * 50f, e.mousePosition + Vector2.up * 50f,
                        Color.white, null, 2f);
                }
                GUI.changed = true;
            }
            if (m_cOutNodePoint != null && m_cInNodePoint == null)
            {
                Rect rect = new Rect(scrollPos.x, scrollPos.y, m_sPosition.width, m_sPosition.height);
                bool isInWindow = rect.Contains(e.mousePosition);
                if (isInWindow)
                {
                    Handles.DrawBezier(m_cOutNodePoint.rect.center, e.mousePosition,
                        m_cOutNodePoint.rect.center + Vector2.up * 50f, e.mousePosition + Vector2.down * 50f,
                        Color.white, null, 2f);
                }
                GUI.changed = true;
            }
        }

        //        private void OnClickConnectRemove(NEConnection connect)
        //        {
        //            m_lstConnection.Remove(connect);
        //            GUI.changed = true;
        //        }

        private void OnClickNodeRemove(NENode node)
        {
            if (m_cDragNode == node) m_cDragNode = null;
            if (m_cSelectedNode == node) m_cSelectedNode = null;
            RemoveNode(node);
            GUI.changed = true;
        }

        public void RemoveNode(NENode node)
        {
            if (node.parent != null)
            {
                node.parent.children.Remove(node);
            }
            m_lstNode.Remove(node);
            for (int i = m_lstConnection.Count - 1; i > -1; i--)
            {
                if (m_lstConnection[i].inPoint.node == node || m_lstConnection[i].outPoint.node == node)
                {
                    m_lstConnection.RemoveAt(i);
                }
            }

            for (int i = node.children.Count - 1; i >= 0 ; i--)
            {
                RemoveNode(node.children[i]);
            }
            RefreshPosition();
        }

        private void OnClickNodePoint(NENodePoint nodePoint)
        {
            if (m_cInNodePoint != null)
            {
                if (m_cInNodePoint.node != nodePoint.node && nodePoint.pointType == NENodePointType.Out)
                {
                    m_cOutNodePoint = nodePoint;
                    CreateConnection(m_cInNodePoint, m_cOutNodePoint);
                    ClearNodePoints();
                }
            }
            else if (m_cOutNodePoint != null)
            {
                if (m_cOutNodePoint.node != nodePoint.node && nodePoint.pointType == NENodePointType.In)
                {
                    m_cInNodePoint = nodePoint;
                    CreateConnection(m_cInNodePoint, m_cOutNodePoint);
                    ClearNodePoints();
                }
            }
            else
            {
                if (nodePoint.pointType == NENodePointType.In)
                {
                    m_cInNodePoint = nodePoint;
                }
                else
                {
                    m_cOutNodePoint = nodePoint;
                }
            }
        }

        private void ClearNodePoints()
        {
            m_cOutNodePoint = null;
            m_cInNodePoint = null;
        }

        private void CreateConnection(NENodePoint inPoint, NENodePoint outPoint)
        {
            NEConnection connection = new NEConnection(inPoint, outPoint);
            m_lstConnection.Add(connection);
        }

        private void HandleEvent(Event e)
        {
            Rect rect = new Rect(scrollPos.x, scrollPos.y, m_sPosition.width, m_sPosition.height);
            bool isInWindow = rect.Contains(e.mousePosition);
            //左键按下
            if (e.button == 0)
            {
                switch (e.type)
                {
                    case EventType.MouseDown:
                        if (isInWindow)
                        {
                            NENode selectNode = GetNodeByPosition(e.mousePosition);
                            if (selectNode != m_cSelectedNode)
                            {
                                SelectNode(selectNode);
                                GUI.changed = true;
                            }

                            if (selectNode != null && selectNode.parent != null)
                            {
                                m_cDragNode = selectNode;
                            }

                            GUI.FocusControl(null);
                            e.Use();
                        }
                        break;
                    case EventType.MouseUp:
                        if (m_cDragNode != null)
                        {
                            var parent = m_cDragNode.parent;
                            if (parent != null)
                            {
                                parent.children.Remove(m_cDragNode);
                                float x = m_cDragNode.rect.center.x;
                                if (parent.children.Count > 0)
                                {
                                    if (x <= parent.children[0].rect.center.x)
                                    {
                                        parent.children.Insert(0, m_cDragNode);
                                    }else if (x >= parent.children[parent.children.Count - 1].rect.center.x)
                                    {
                                        parent.children.Add(m_cDragNode);
                                    }
                                    else
                                    {
                                        int index = 0;
                                        for (int i = 0; i < parent.children.Count - 1; i++)
                                        {
                                            var child = parent.children[i];
                                            var child1 = parent.children[i + 1];
                                            if (x >= child.rect.center.x && x <= child1.rect.center.x)
                                            {
                                                index = i;
                                                break;
                                            }
                                        }
                                        parent.children.Insert(index + 1, m_cDragNode);
                                    }
                                }
                                else
                                {
                                    parent.children.Add(m_cDragNode);
                                }
                            }
                            RefreshPosition();
                            GUI.changed = true;
                        }
                        m_cDragNode = null;
                        break;
                    case EventType.MouseDrag:
                        if (isInWindow && null != m_cDragNode)
                        {
                            //m_cDragNode.MovePosition(e.delta);
                            m_cDragNode.SetPosition(e.mousePosition);
                            e.Use();
                            GUI.changed = true;
                        }
                        break;
                }
            }
            //右键按下
            else if (e.button == 1 && e.type == EventType.ContextClick)
            {
                if (m_cInNodePoint != null || m_cOutNodePoint != null)
                {
                    ClearNodePoints();
                    GUI.changed = true;
                    e.Use();
                }
                else if (isInWindow)
                {
                    NENode selectNode = GetNodeByPosition(e.mousePosition);
                    if (selectNode != null)
                    {
                        HandleNodeMenu(selectNode, e.mousePosition);
                        //e.Use();
                    }
                    else
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("刷新"), false, () => {
                            RefreshPosition();
                        });
                        menu.ShowAsContext();
                        GUI.changed = true;
                        //e.Use();
                    }
                    GUI.changed = true;
                    return;
                }
            }
            else if(e.button == 2)
            {
                switch (e.type)
                {
                    case EventType.MouseDrag:
                        if (isInWindow)
                        {
                            scrollPos -= e.delta;
                            e.Use();
                            GUI.changed = true;
                        }
                        break;
                }
            }
        }

        private NENode GetNodeByPosition(Vector2 pos)
        {
            NENode selectNode = null;
            for (int i = 0; i < m_lstNode.Count; i++)
            {
                if (m_lstNode[i].rect.Contains(pos))
                {
                    selectNode = m_lstNode[i];
                    break;
                }
            }
            return selectNode;
        }

        private void SelectNode(NENode node)
        {
            for (int i = 0; i < m_lstNode.Count; i++)
            {
                if (m_lstNode[i] == node)
                {
                    m_lstNode[i].SetSelected(true);
                }
                else
                {
                    if (m_lstNode[i].isSelected) m_lstNode[i].SetSelected(false);
                }
            }
            m_cSelectedNode = node;
        }

//        private void HandleBlankMenu(Vector2 mousePosition)
//        {
//            GenericMenu menu = new GenericMenu();
//            int count = m_dicNodeDataType.Count;
//            foreach (var item in m_dicNodeDataType)
//            {
//                string pre = "";
//                if(!string.IsNullOrEmpty(item.Key))
//                {
//                    pre += item.Key + "/";
//                }
//                for (int i = 0; i < item.Value.Count; i++)
//                {
//                    Type nodeDataType = item.Value[i];
//                    var dataName = nodeDataType.Name;
//                    if (dataName.EndsWith("Data")) dataName = dataName.Substring(0, dataName.Length - 4);
//                    string name = pre + dataName;
//                    menu.AddItem(new GUIContent(name), false, () => {
//                        if (m_fCreateNodeDataFunc != null)
//                        {
//                            object data = m_fCreateNodeDataFunc.Invoke(nodeDataType);
//                            CreateNode(mousePosition, data);
//                        }
//                    });
//                }
//                if(count > 1)
//                {
//                    menu.AddSeparator("");
//                }
//                count--;
//            }
//            if(m_cCopyObj != null)
//            {
//                menu.AddItem(new GUIContent("粘贴"), false, () => { CreateNode(mousePosition, m_cCopyObj); });
//            }
//            menu.ShowAsContext();
//            GUI.changed = true;
//        }

        private void HandleNodeMenu(NENode node, Vector2 mousePosition)
        {
            GUI.changed = true;
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("拷贝节点"), false, () => {
                if (m_cCopyFunc != null)
                {
                    m_cCopyObj = node;
//                    m_cCopyObj = m_cCopyFunc.Invoke(node);
                }
            });
            int count = m_dicNodeDataType.Count;
            foreach (var item in m_dicNodeDataType)
            {
                string pre = "";
                if (!string.IsNullOrEmpty(item.Key))
                {
                    pre += item.Key + "/";
                }
                for (int i = 0; i < item.Value.Count; i++)
                {
                    Type nodeDataType = item.Value[i];
                    var dataName = nodeDataType.Name;
                    if (dataName.EndsWith("Data")) dataName = dataName.Substring(0, dataName.Length - 4);
                    string name = pre + dataName;
                    menu.AddItem(new GUIContent(name), false, () => {
                        if (m_fCreateNodeDataFunc != null)
                        {
                            object data = m_fCreateNodeDataFunc.Invoke(nodeDataType);
                            CreateNode(node, data);
                            RefreshPosition();
                            GUI.changed = true;
                        }
                    });
                }
                if (count > 1)
                {
                    menu.AddSeparator("");
                }
                count--;
            }
            if (m_cCopyObj != null)
            {
                menu.AddItem(new GUIContent("粘贴"), false, () =>
                {
                    if (m_cCopyFunc != null)
                    {
                        CreateNENode(m_cCopyFunc.Invoke(m_cCopyObj), node);
                        RefreshPosition();
                    }
                });
            }
            menu.ShowAsContext();
            GUI.changed = true;
        }

        private void DrawGrid()
        {
            DrawGrid(10, new Color(0.5f, 0.5f, 0.5f, 0.2f));
            DrawGrid(50, new Color(0.5f, 0.5f, 0.5f, 0.4f));
        }

        private void DrawGrid(float gridSpacing, Color gridColor)
        {
            int column = Mathf.CeilToInt(scrollViewRect.height / gridSpacing);
            int row = Mathf.CeilToInt(scrollViewRect.width / gridSpacing);
            Handles.BeginGUI();
            Color oldColor = Handles.color;
            Handles.color = gridColor;
            for (int i = 0; i < column; i++)
            {
                Handles.DrawLine(new Vector3(0, i * gridSpacing, 0), new Vector3(scrollViewRect.width, i * gridSpacing, 0));
            }
            for (int i = 0; i < row; i++)
            {
                Handles.DrawLine(new Vector3(i * gridSpacing, 0, 0), new Vector3(i * gridSpacing, scrollViewRect.height, 0));
            }
            Handles.color = oldColor;
            Handles.EndGUI();
        }

        public void Clear()
        {
            m_lstNode.Clear();
            m_cRoot = null;
            m_lstConnection.Clear();
        }

        public void Dispose()
        {
            m_lstNode.Clear();
            m_cRoot = null;
            m_lstConnection.Clear();
            m_fCreateNodeDataFunc = null;
            m_cCopyFunc = null;
            m_cCopyObj = null;
        }
    }
}