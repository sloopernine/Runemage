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

        if (GUILayout.Button("Rebuild Shield"))
        {
            dm.ResetShield();
        }
        EditorGUILayout.HelpBox("Calls RebuildShield() to restore shield to full health", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("Invulnerable Shield"))
        {
            dm.Invulnerability();
        }
        EditorGUILayout.HelpBox("Sets the shield to invulnerable state", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("Invulnerable Shield Off"))
        {
            dm.Invulnerability_Off();
        }
        EditorGUILayout.HelpBox("Turns invulnerable state off", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("Force next round"))
        {
            dm.ForceNextRound();
        }
        EditorGUILayout.HelpBox("Calls die() on all living enemies and moves to next round", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("Force first round"))
        {
            dm.ForceFirstRound();
        }
        EditorGUILayout.HelpBox("Calls die() on all living enemies and moves the first round", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("Kill All Enemies")) 
        {
            dm.KillAllEnemies();
        }
        EditorGUILayout.HelpBox("Calls die() on all living enemies", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("Destroy All Spells"))
        {
            dm.DestroyAllSpells();
        }
        EditorGUILayout.HelpBox("Calls Destroy(gameObject) on all SpellObjects", MessageType.Info);
        EditorGUILayout.Space();

		    if(GUILayout.Button("Destroy All RuneClouds"))
		    {
			    dm.DestroyAllRuneClouds();
		    }
		    EditorGUILayout.HelpBox("Calls function of RuneDestroyer that destroyes all RuneCloudObjects", MessageType.Info);
		    EditorGUILayout.Space();
	      
        if (GUILayout.Button("Set Paused"))
        {
            dm.SetGameStatePaused();
        }
        EditorGUILayout.HelpBox("Sets GameManager.CurrentGameState to paused", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("Set Play"))
        {
            dm.SetGameStatePlay();
        }
        EditorGUILayout.HelpBox("Sets GameManager.CurrentGameState to Play", MessageType.Info);
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Set Win"))
        {
            dm.SetGameStateWin();
        }
        EditorGUILayout.HelpBox("Sets GameManager.CurrentGameState to Win", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("Set Lose"))
        {
            dm.SetGameStateLose();
        }
        EditorGUILayout.HelpBox("Sets GameManager.CurrentGameState to Lost", MessageType.Info);
        EditorGUILayout.Space();
    }

}
