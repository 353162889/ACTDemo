using System;
using System.Collections.Generic;
using Framework;
using Game;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class UtilityNodeContainerItem : UtilityNodeItem
{
    public static string ChildNodeStyle = "utility-node";
    public Foldout foldout { get; private set; }

    public UtilityNodeContainerItem(object data) : base(data)
    {
        foldout = this.Q<Foldout>("foldout");
    }

    public void MoveChildUp(UtilityNodeItem child)
    {
        var index = child.parent.IndexOf(child);
        index--;
        if (index < 0) index = 0;
        child.parent.Insert(index, child);
    }

    public void MoveChildDown(UtilityNodeItem child)
    {
        var index = child.parent.IndexOf(child);
        index++;
        if (index >= this.parent.childCount) index = child.parent.childCount - 1;
        child.parent.Insert(index, child);
    }

    public void RemoveChildItem(UtilityNodeItem child)
    {
        this.foldout.contentContainer.Remove(child);
    }

    public void AddChildItem(VisualElement child)
    {
        child.AddToClassList(ChildNodeStyle);
        this.foldout.contentContainer.Add(child);
        this.foldout.value = true;
    }
}
