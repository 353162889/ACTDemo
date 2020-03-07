using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    public class NENode
    {
        //占位宽度
        public float placeHolderWidth;
        public float startPlaceHolderX;
        public NENode parent { get; private set; }
        public List<NENode> children { get; private set; }
        public Rect rect;
        private Vector2 normalSize = Vector2.zero;
        private Vector2 extendSize = Vector2.zero;
        private float buttonHeight = 0;
        private static float singleBtnHeight = 25;
        public object node { get; private set; }
        public NEDataProperty[] dataProperty { get; private set; }
        public NEPropertyBtnAttribute[] arrBtnAttr { get; private set; }
        private List<NEDataProperty> m_lstShowOnNodeProperty;
        private GUIStyle m_cNormalStyle;
        private GUIStyle m_cSelectStyle;
        private GUIStyle m_cStyle;
        private GUIStyle m_cContentStyle;
        private GUIStyle m_cExtendStyle;
        private GUIStyle m_cCloseStyle;
        private Texture2D m_cImg;
        private float m_fImgWidth = 20;
        private string m_sName;
        public string desc { get; private set; }
        private bool m_bShowInPoint;
        private bool m_bShowOutPoint;
        private bool m_bShowClose;
        public bool isSelected { get { return m_bIsSelected; } }
        private bool m_bIsSelected;

        public NENodePoint inPoint { get { return m_cInPoint; } }
        private NENodePoint m_cInPoint;
        public NENodePoint outPoint { get { return m_cOutPoint; } }
        private NENodePoint m_cOutPoint;

        private NENodeBtnAttribute[] m_cNodeBtnArr;

        public void SetParent(NENode parent)
        {
            this.parent = parent;
        }

        public NENode(Vector2 position,object node)
        {
            this.node = node;
            parent = null;
            children = new List<NENode>();
            m_cNormalStyle = null;
            m_cSelectStyle = null;
            m_cCloseStyle = null;
            m_cImg = null;

            m_sName = "";
            desc = "";
            m_bShowInPoint = this.node != null;
            m_bShowOutPoint = true;
            m_bShowClose = this.node != null;
            if (this.node != null)
            {
                var type = this.node.GetType();
                m_sName = NENodeNameAttribute.GetName(type);
                desc = NENodeDescAttribute.GetDesc(type);
                dataProperty = NEDataProperties.GetProperties(this.node);
                arrBtnAttr = (NEPropertyBtnAttribute[])type.GetCustomAttributes(typeof(NEPropertyBtnAttribute), false);
                var attributes = type.GetCustomAttributes(typeof(NENodeDisplayAttribute), false);
                if(attributes.Length > 0)
                {
                    NENodeDisplayAttribute displayAttribute = attributes[0] as NENodeDisplayAttribute;
                    m_bShowInPoint = displayAttribute.showInPoint;
                    m_bShowOutPoint = displayAttribute.showOutPoint;
                    m_bShowClose = displayAttribute.showClose;
                }
                m_cNodeBtnArr = (NENodeBtnAttribute[])type.GetCustomAttributes(typeof(NENodeBtnAttribute), false);
            }
            else
            {
                m_sName = "Root";
            }
            m_cStyle = m_cNormalStyle;
            m_cContentStyle = new GUIStyle();
            m_cContentStyle.fontSize = 10;
            m_cContentStyle.normal.textColor = Color.white;
            m_cContentStyle.alignment = TextAnchor.MiddleCenter;
            m_cExtendStyle = new GUIStyle();
            m_cExtendStyle.fontSize = 8;
            m_cExtendStyle.normal.textColor = Color.white;
            m_cExtendStyle.alignment = TextAnchor.MiddleCenter;

            float width = 80;
            float height = 40;
            var descSize = m_cContentStyle.CalcSize(new GUIContent(m_sName));
            width = Mathf.Max(descSize.x, width);
            normalSize = new Vector2(width,height);

            extendSize = Vector2.zero;
            m_lstShowOnNodeProperty = new List<NEDataProperty>();
            if (dataProperty != null)
            {
                for (int i = 0; i < dataProperty.Length; i++)
                {
                    if (dataProperty[i].showOnNode)
                    {
                        var extSize = m_cExtendStyle.CalcSize(new GUIContent(dataProperty[i].Name + ":" +dataProperty[i].GetValue().ToString()));
                        if (extSize.x > extendSize.x) extendSize.x = extSize.x;
                        extendSize.y += extSize.y;
                        m_lstShowOnNodeProperty.Add(dataProperty[i]);
                    }
                }
            }
            if (extendSize.y > 0) extendSize.y += 10;
            if (m_cNodeBtnArr != null)
            {
                for (int i = 0; i < m_cNodeBtnArr.Length; i++)
                {
                    buttonHeight += singleBtnHeight;
                }
            }

            if (buttonHeight > 0)
            {
                buttonHeight += 10;
            }

            float rectWidth = Mathf.Max(normalSize.x, extendSize.x) + 10;
            float rectHeight = normalSize.y + extendSize.y + buttonHeight;
            rect = new Rect(position.x - rectWidth / 2, position.y - rectHeight / 2, rectWidth, rectHeight);

            if (m_bShowInPoint)
            {
                m_cInPoint = new NENodePoint(this, NENodePointType.In);
            }
            if (m_bShowOutPoint)
            {
                m_cOutPoint = new NENodePoint(this, NENodePointType.Out);
            }

            placeHolderWidth = rect.width;
        }

        public virtual void Draw(Action<NENode> onClickRemoveNode, Action<NENodePoint> onClickNodePoint)
        {
            if (m_cNormalStyle == null)
            {
                m_cNormalStyle = new GUIStyle((GUIStyle)"flow node 0");
                m_cStyle = m_bIsSelected ? m_cSelectStyle : m_cNormalStyle; 
            }
            if (m_cSelectStyle == null)
            {
                m_cSelectStyle = new GUIStyle((GUIStyle)"flow node 0 on");
                m_cStyle = m_bIsSelected ? m_cSelectStyle : m_cNormalStyle;
            }
            if (m_cCloseStyle == null)
            {
                m_cCloseStyle = new GUIStyle((GUIStyle)"ToolbarSeachCancelButton");
            }
            if (m_cImg == null)
            {
                m_cImg = EditorGUIUtility.FindTexture("Favorite Icon");
            }
            if (m_cInPoint != null)
            {
                m_cInPoint.Draw(onClickNodePoint);
            }
            if (m_cOutPoint != null)
            {
                m_cOutPoint.Draw(onClickNodePoint);
            }
            
            GUILayout.BeginArea(rect, m_cStyle);
            if (m_cImg != null)
            {
                GUI.DrawTexture(new Rect((rect.width - m_fImgWidth) / 2, 4, m_fImgWidth, m_fImgWidth), m_cImg);
            }
            if (!string.IsNullOrEmpty(m_sName))
            {
                GUI.Label(new Rect(0, m_fImgWidth + 4, rect.width, normalSize.y - m_fImgWidth - 4), m_sName, m_cContentStyle);
            }
            if(m_lstShowOnNodeProperty.Count > 0)
            {
                GUILayout.BeginArea(new Rect(0, normalSize.y, rect.width, extendSize.y));
                for (int i = 0; i < m_lstShowOnNodeProperty.Count; i++)
                {
                    string desc = m_lstShowOnNodeProperty[i].Name + ":" + m_lstShowOnNodeProperty[i].GetValue().ToString();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(desc, m_cExtendStyle);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }
            if (m_cNodeBtnArr != null)
            {
                GUILayout.BeginArea(new Rect(5, normalSize.y + extendSize.y, rect.width - 10, buttonHeight));
                var btnStyle = new GUIStyle(GUI.skin.button);
                btnStyle.fontSize = 10;
                for (int i = 0; i < m_cNodeBtnArr.Length; i++)
                {
                    var attr = m_cNodeBtnArr[i];
                    GUILayout.BeginHorizontal();
                    
                    if (GUILayout.Button(attr.name,btnStyle, GUILayout.Height(singleBtnHeight)))
                    {
                        attr.ExecuteBtn(this);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }
            GUILayout.EndArea();
            
            float closeWidth = m_cCloseStyle.fixedWidth;
            float closeHeight = m_cCloseStyle.fixedHeight;
            if (m_bShowClose && GUI.Button(new Rect(rect.x + rect.width, rect.y, closeWidth, closeHeight), "", m_cCloseStyle))
            {
                if (null != onClickRemoveNode)
                {
                    onClickRemoveNode(this);
                }
            }
        }

        public void SetSelected(bool selected)
        {
            m_bIsSelected = selected;
            m_cStyle = m_bIsSelected ? m_cSelectStyle : m_cNormalStyle;
        }

        public void SetPosition(Vector2 pos)
        {
            rect.position = new Vector2(pos.x - rect.width / 2, pos.y - rect.height / 2);
        }

        public void MovePosition(Vector2 pos)
        {
            rect.position += pos;
        }
    }
}