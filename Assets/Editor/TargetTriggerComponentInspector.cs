using Game;
using UnityEditor;

[CustomEditor(typeof(TargetTriggerComponent))]
public class TargetTriggerComponentInspector : Editor
{
    private TargetTriggerComponent component;
    private bool foldout = false;
    void OnEnable()
    {
        component = target as TargetTriggerComponent;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (component == null) return;
        foldout = EditorGUILayout.Foldout(foldout, "entities");
        if (foldout)
        {
            for (int i = 0; i < component.lstEntity.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(component.lstEntity[i].ToString());
                EditorGUILayout.EndHorizontal();
            }
            EditorUtility.SetDirty(target);
        }
    }
}