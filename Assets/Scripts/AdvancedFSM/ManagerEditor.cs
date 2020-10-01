using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TankPathingSystem;

[CustomEditor(typeof(WaypointManager))]
public class ManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WaypointManager thisTarget = (WaypointManager)target;

        if (GUILayout.Button("Find All Neighbors"))
        {
            thisTarget.FindAllNeighbors();
        }
    }
}
