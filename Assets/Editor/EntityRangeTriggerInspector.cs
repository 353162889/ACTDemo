using Game;
using UnityEditor;

[CustomEditor(typeof(EntityRangeTrigger))]
public class EntityRangeTriggerInspector : Editor
{
    private EntityRangeTrigger rangeTrigger;
    private bool foldout = false;
    void OnEnable()
    {
        rangeTrigger = target as EntityRangeTrigger;
    }

    public override void OnInspectorGUI()
    {
        if (rangeTrigger == null) return;
        foldout = EditorGUILayout.Foldout(foldout, "entities");
        if (foldout)
        {
            for (int i = 0; i < rangeTrigger.lstEntity.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(rangeTrigger.lstEntity[i].ToString());
                EditorGUILayout.EndHorizontal();
            }
            EditorUtility.SetDirty(target);
        }
    }
}
