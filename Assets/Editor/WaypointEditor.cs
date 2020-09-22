using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TankPathingSystem;

[CustomEditor(typeof(Waypoint))]
[CanEditMultipleObjects]
public class WaypointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Waypoint thisTarget = (Waypoint)target;

        if (GUILayout.Button("Find Neighbors"))
        {
            thisTarget.FindNeighbors();
        }
        if(GUILayout.Button("Recalculate Weights/Distances"))
        {
            thisTarget.RecalculateWeights();
        }
        if(GUILayout.Button("Reset Neighbors"))
        {
            thisTarget.ResetNeighbors();
        }
    }
}
