using System;
using Framework;
using Game;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class UtilityNodeItem : VisualElement
{
    public object data
    {
        get { return utilityData.data; }
    }
    private UtilityDataToSerializeProperty utilityData;

    public VisualElement dataContainer { get; private set; }

    public UtilityNodeContainerItem parentContainer
    {
        get { return this.GetElementInParent<UtilityNodeContainerItem>(); }
    }

    private Label labelDebug;
    protected Type dataType;

    public UtilityNodeItem(object data)
    {
        var serializeProperty = UtilityDataToSerializeProperty.GetSerializeProperty(data);
        utilityData = (UtilityDataToSerializeProperty)serializeProperty.serializedObject.targetObject;
        this.dataType = this.data.GetType();

        var xml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(string.Format("Assets/Editor/UtilityAIEditor/{0}.uxml", this.GetType().Name));
        var node = xml.CloneTree();
        this.Add(node);

        var labelName = node.Q<Label>("labelName");
        labelName.text = data.GetType().Name;
        dataContainer = node.Q<VisualElement>("dataContainer");
        while (serializeProperty.Next(true))
        {
            if(serializeProperty.depth != 1)continue;
            var propertyField = new PropertyField();
            propertyField.BindProperty(serializeProperty);
            dataContainer.Add(propertyField);
        }

        labelDebug = node.Q<Label>("labelDebug");

        ContextualMenuManipulator manipulator = new ContextualMenuManipulator(OnMenu);
        manipulator.target = this;

        this.RegisterCallback<MouseUpEvent>(evt =>
        {
            if (evt.button == 0)
            {
                bool contain = this.ClassListContains("select_utility_item");
                var window = EditorWindow.GetWindow<UtilityAIWindow>();
                if (window.rootVisualElement != null)
                {
                    window.rootVisualElement.Query<UtilityNodeItem>().ForEach(item =>
                    {
                        item.RemoveFromClassList("select_utility_item");
                    });
                }

                if (!contain)
                {
                    this.AddToClassList("select_utility_item");
                }

                evt.StopPropagation();
            }
        }, TrickleDown.NoTrickleDown);


        this.RegisterCallback<AttachToPanelEvent>(evt =>
        {
            if (evt.target is UtilityNodeItem)
            {
                var window = EditorWindow.GetWindow<UtilityAIWindow>();
                window.UpdateDataToItem();
                evt.StopPropagation();
            }
        });
        this.RegisterCallback<DetachFromPanelEvent>(evt =>
        {
            if (evt.target is UtilityNodeItem)
            {
                var window = EditorWindow.GetWindow<UtilityAIWindow>();
                window.UpdateDataToItem();
                evt.StopPropagation();
            }
        });
    }

    public virtual object Serializable()
    {
        return this.data;
    }

    public virtual void Deserializable()
    {
        this.Query<UtilityNodeItem>().ForEach(item =>
        {
            if (item != this)
            {
                item.Deserializable();
            }
        });
    }

    public virtual void SetDebugText(string text)
    {
        if (labelDebug != null)
        {
            labelDebug.text = text;
        }
    }

    private void OnMenu(ContextualMenuPopulateEvent evt)
    {
        if (this.parentContainer != null)
        {
            evt.menu.AppendAction("↑",
                action => { this.parentContainer.MoveChildUp(this); });
            evt.menu.AppendAction("↓",
                action => { this.parentContainer.MoveChildDown(this); });
            evt.menu.AppendAction("delete", action => { this.parent.Remove(this); });
        }

        this.OnAddAction(evt.menu);
        evt.StopPropagation();
    }

    protected virtual void OnAddAction(DropdownMenu menu)
    {

    }
}
