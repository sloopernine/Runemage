using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugManager))]
public class DebugManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DebugManager dm = (DebugManager)target;

        if (!dm.IsDebugMode)
        {
            return;
        }

        if (GUILayout.Button("Kill All Enemies"))
        {
            dm.KillAllEnemies();
        }

        if (GUILayout.Button("Force next round"))
        {
            dm.ForceNextRound();
        }

        if (GUILayout.Button("Force first round"))
        {
            dm.ForceFirstRound();
        }

    }

    
    


}
