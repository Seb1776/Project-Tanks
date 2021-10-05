using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Enemy))]
public class EnemyFOV : Editor
{
    void OnSceneGUI()
    {
        Enemy e = (Enemy)target;
        Handles.color = Color.white; 
        Handles.DrawWireArc(e.transform.position, Vector3.forward, Vector3.right, 360f, e.attackDetectionDistance);
        Vector3 viewAngleA = e.DirFromAngle(-e.fieldOfView / 2f, false);
        Vector3 viewAngleB = e.DirFromAngle(e.fieldOfView / 2f, false);
        Handles.DrawLine(e.transform.position, e.transform.position + viewAngleA * e.attackDetectionDistance);
        Handles.DrawLine(e.transform.position, e.transform.position + viewAngleB * e.attackDetectionDistance);
    }
}
