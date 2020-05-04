using System.Diagnostics;
using System.IO;
using Framework;
using NodeEditor;
using UnityEditor;
using UnityEditor.Timeline;

public static class NETreeWindowBtnExtension
{
    public static void OnEditorBTTimeLine(object instance)
    {
        NENode node = (NENode) instance;
        if (node == null) return;
        if (!NETreeWindow.IsOpen()) return;
        var window = EditorWindow.GetWindow<NETreeWindow>();
        window.SetTimelineEditorNode(node);
    }
}
