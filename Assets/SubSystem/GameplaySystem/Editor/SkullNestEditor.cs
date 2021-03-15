using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameSystem;
using GameSystem.Setting;

[CustomEditor(typeof(SkullNest))]
public class SkullNestEditor : Editor
{
    static GameplaySystemSetting Setting => GameplaySystem.Setting;
    static TheMatrixEditorSetting EditorSetting => TheMatrix.EditorSetting;


    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    static void DrawNestGizmos(SkullNest nest, GizmoType gizmoType)
    {
        var c = nest.Center;
        Gizmos.color = EditorSetting.nestCenterColor;
        Gizmos.DrawSphere(c, 0.1f);
        Gizmos.color = EditorSetting.nestPatrolAreaColor;
        Gizmos.DrawWireSphere(c, nest.patrolRadius);
        if (gizmoType.HasFlag(GizmoType.Selected))
        {
            Gizmos.color = EditorSetting.nestChaseAreaColor;
            Gizmos.DrawWireSphere(c, nest.chaseRadius);
        }
    }
}
