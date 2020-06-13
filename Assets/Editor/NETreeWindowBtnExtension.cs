using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Framework;
using Game;
using NodeEditor;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;

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

    public static void SetSkillEffectiveRange(object instance)
    {
        NENode node = (NENode) instance;
        if (!(node.node is BTSkillRootData))
        {
            CLog.LogArgs("设置技能根节点数据失败");
            return;
        }

        var attackDistance = GetChildNodeAttackDistance(node);
        var moveDistance = GetChildNodeMoveDistance(node);
        var rootData = node.node as BTSkillRootData;
        rootData.effectiveRange = attackDistance + moveDistance;
    }

    private static Dictionary<Type, Func<NENode, float>> m_dicGetAttackDistance = new Dictionary<Type, Func<NENode, float>>()
    {
        { typeof(BTBoxColliderActionData), GetBTBoxColliderActionDataDistance},
        { typeof(BTSphereColliderActionData), GetBTSphereColliderActionDataDistance},
    };

    private static Dictionary<Type, Func<NENode, float>> m_dicMoveGetDistance = new Dictionary<Type, Func<NENode, float>>()
    {
        { typeof(BTAnimationMoveActionData), GetBTAnimationMoveActionDataDistance},
    };


    private static float GetBTBoxColliderActionDataDistance(NENode node)
    {
        float dis = 0;
        if (node.node is BTBoxColliderActionData)
        {
            BTBoxColliderActionData data = node.node as BTBoxColliderActionData;
            Vector3 max = data.size / 2;
            Vector3 localPos = data.localPos;
            localPos.y = 0;
            Vector3[] positions = new Vector3[]
            {
                new Vector3(-max.x, max.y, -max.z),
                new Vector3(max.x, max.y, -max.z),
                max,
                new Vector3(-max.x, max.y, max.z),
                -max,
                new Vector3(max.x, -max.y, -max.z),
                new Vector3(max.x, -max.y, max.z),
                new Vector3(-max.x, -max.y, max.z),
            };
            foreach (var position in positions)
            {
                var p = data.localRot * position;
                p.y = 0;
                float distance = (p + localPos).magnitude;
                if (distance > dis) dis = distance;
            }
        }
        return dis;
    }

    private static float GetBTSphereColliderActionDataDistance(NENode node)
    {
        float dis = 0;
        if (node.node is BTSphereColliderActionData)
        {
            BTSphereColliderActionData data = node.node as BTSphereColliderActionData;
            dis = data.localPos.magnitude + data.radius;
        }
        return dis;
    }

    private static float GetBTAnimationMoveActionDataDistance(NENode node)
    {
        float dis = 0;
        if (node.node is BTAnimationMoveActionData)
        {
            BTAnimationMoveActionData data = node.node as BTAnimationMoveActionData;
            var points = AnimationMoveUtility.GetAllVector3(data.movePoints);
            for (int i = 0; i < points.Length; i++)
            {
                var position = points[i];
                position.y = 0;
                float distance = position.magnitude;
                if (distance > dis) dis = distance;
            }
        }
        return dis;
    }

    private static float GetChildNodeAttackDistance(NENode node)
    {
        float dis = 0;
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        foreach (var nodeChild in node.children)
        {
            var distance = GetChildNodeAttackDistance(nodeChild);
            if (distance > dis) dis = distance;
        }

        if (node.node != null)
        {
            Func<NENode, float> func;
            if (m_dicGetAttackDistance.TryGetValue(node.node.GetType(), out func))
            {
                var distance = func.Invoke(node);
                if (distance > dis) dis = distance;
            }
        }
        return dis;
    }

    private static float GetChildNodeMoveDistance(NENode node)
    {
        float dis = 0;
        foreach (var nodeChild in node.children)
        {
            var distance = GetChildNodeMoveDistance(nodeChild);
            if (distance > dis) dis = distance;
        }

        if (node.node != null)
        {
            Func<NENode, float> func;
            if (m_dicMoveGetDistance.TryGetValue(node.node.GetType(), out func))
            {
                var distance = func.Invoke(node);
                if (distance > dis) dis = distance;
            }
        }
        return dis;
    }
}
