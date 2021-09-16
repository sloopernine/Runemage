using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(RuneCloud), true)]
public class RuneCloudEditor : Editor
{
    private RuneCloud runeCloud;
    private bool hasComponent;
    
    public void OnEnable()
    {
        if (Selection.activeGameObject.HasComponent<RuneCloud>())
        {
            runeCloud = Selection.activeGameObject.GetComponent<RuneCloud>();
            hasComponent = true;
        }
        else
        {
            hasComponent = false;
        }
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (!hasComponent)
            return;
        
        EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

        EditorGUILayout.Space(20);
        
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Save Gesture"))
        {
            runeCloud.SaveGestureToXML();
        }
        
        GUILayout.EndHorizontal();
        
        EditorGUI.EndDisabledGroup();
    }
}
