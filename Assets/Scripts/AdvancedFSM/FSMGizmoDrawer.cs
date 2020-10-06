using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

//FSMState does not inherit from monobehavior, so I could not use Gizmos to debug without a class that can draw the gizmos for the states
//public class FSMGizmoDrawer : MonoBehaviour
//{
//    private List<KeyValuePair<Vector3[], Color>> lines = new List<KeyValuePair<Vector3[], Color>>();
//    public void DrawLine(Vector3 p1, Vector3 p2, Color color)
//    {
//        lines.Add(new KeyValuePair<Vector3[], Color>(new Vector3[]{ p1, p2 },color));
//    }

//    private void OnDrawGizmos()
//    {

//    }
//}

//public class FSMGizmoDrawer
//{
//    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
//    public static void DrawLineToNextWaypoint(FSMState state, GizmoType gizmoType)
//    {
//        Vector3 pos = state.tank.transform.position;
//        Vector3 dest = state.destWaypoint.transform.position;
//        switch (state.ID)
//        {
//            case FSMStateID.OffDuty:
//                Gizmos.color = Color.green;
//                break;
//            case FSMStateID.Patrolling:
//                Gizmos.color = Color.blue;
//                break;
//        }
//        Gizmos.DrawLine(pos, dest);
//    }
//}
